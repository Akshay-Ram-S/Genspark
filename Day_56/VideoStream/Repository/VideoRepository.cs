using Microsoft.EntityFrameworkCore;
using VideoStream.Contexts;
using VideoStream.Models;

namespace VideoStream.Repositories
{
    public class VideoRepository : Repository<Guid, TrainingVideo>
    {
        public VideoRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<TrainingVideo> Get(Guid key)
        {
            var videos = await _auctionContext.TrainingVideos.SingleOrDefaultAsync(p => p.Id == key);

            return videos??throw new Exception("No Bid found with the given ID");
        }

        public override async Task<IEnumerable<TrainingVideo>> GetAll()
        {
            var videos = _auctionContext.TrainingVideos;
            return await videos.ToListAsync();
        }
    }
}