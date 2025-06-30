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
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();

                var context = scope.ServiceProvider.GetRequiredService<AuctionContext>();
                var bidRepo = scope.ServiceProvider.GetRequiredService<IRepository<Guid, Bid>>();
                var itemRepo = scope.ServiceProvider.GetRequiredService<IRepository<Guid, Item>>();
                var bidderRepo = scope.ServiceProvider.GetRequiredService<IRepository<Guid, Bidder>>();
                var auditRepo = scope.ServiceProvider.GetRequiredService <IRepository<Guid, Audit>>();

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
                        continue;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[MonitorService] Unexpected error while processing item {item.Id}: {ex.Message}");
                        continue; 
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}