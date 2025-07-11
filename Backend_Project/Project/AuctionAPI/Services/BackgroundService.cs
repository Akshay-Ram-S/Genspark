using AuctionAPI.Contexts;
using AuctionAPI.Exceptions;
using AuctionAPI.Interfaces;
using AuctionAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AuctionAPI.Service
{
    public class AuctionMonitorService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public AuctionMonitorService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await WaitForDatabaseAsync(stoppingToken);
            DateTime lastLogUploadTime = DateTime.MinValue;

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    using var scope = _serviceProvider.CreateScope();

                    var context = scope.ServiceProvider.GetRequiredService<AuctionContext>();
                    var bidRepo = scope.ServiceProvider.GetRequiredService<IRepository<Guid, Bid>>();
                    var itemRepo = scope.ServiceProvider.GetRequiredService<IRepository<Guid, Item>>();
                    var bidderRepo = scope.ServiceProvider.GetRequiredService<IRepository<Guid, Bidder>>();
                    var auditRepo = scope.ServiceProvider.GetRequiredService<IRepository<Guid, Audit>>();

                    var itemsToClose = await context.Items
                        .Where(i => i.Status == "Active" && i.EndDate <= DateTime.UtcNow)
                        .ToListAsync(stoppingToken);

                    foreach (var item in itemsToClose)
                    {
                        try
                        {
                            var bids = await bidRepo.GetAll();
                            var highestBid = bids
                                .Where(b => b.ItemId == item.Id && !b.IsDeleted)
                                .OrderByDescending(b => b.Amount)
                                .FirstOrDefault();

                            if (highestBid == null)
                            {
                                item.Status = "Unsold";
                            }
                            else
                            {
                                item.Status = "Sold";
                                item.BidderID = highestBid.BidderId;

                                var bidder = await bidderRepo.Get(highestBid.BidderId);
                                if (bidder?.User != null)
                                {
                                    await auditRepo.Add(new Audit
                                    {
                                        Action = "Bought",
                                        CreatedAt = DateTime.UtcNow,
                                        CreatedBy = bidder.User.Email,
                                        EntityId = bidder.UserId,
                                        EntityType = "Bidder"
                                    });
                                }
                            }

                            await itemRepo.Update(item.Id, item);
                        }
                        catch (IdNotFoundException ex)
                        {
                            Console.WriteLine($"[MonitorService] Item not found during update. Skipping. ID: {item.Id}, Msg: {ex.Message}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[MonitorService] Error processing item {item.Id}: {ex.Message}");
                        }
                    }

                    await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MonitorService] Fatal error: {ex.Message}");
            }
        }

        private async Task WaitForDatabaseAsync(CancellationToken token)
        {
            const int maxRetries = 10;
            int retries = 0;

            while (retries < maxRetries && !token.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<AuctionContext>();

                    await context.Database.ExecuteSqlRawAsync("SELECT 1", token);
                    Console.WriteLine("[MonitorService] Database is ready.");
                    return;
                }
                catch (Exception ex)
                {
                    retries++;
                    Console.WriteLine($"[MonitorService] Waiting for database... attempt {retries}, error: {ex.Message}");
                    await Task.Delay(3000, token);
                }
            }

            throw new Exception("[MonitorService] Database not ready after retries.");
        }
    }
}
