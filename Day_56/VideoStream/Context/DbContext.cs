using Microsoft.EntityFrameworkCore;
using VideoStream.Models;

namespace VideoStream.Contexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) {}

        public DbSet<TrainingVideo> TrainingVideos { get; set; }
    }
}
