using Microsoft.EntityFrameworkCore;
using App.Models;

namespace App.Contexts
{
    class TwitterContext : DbContext
    {
        public TwitterContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Tweet> Tweets { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Hashtag> Hashtags { get; set; }
        public DbSet<TweetHashtag> TweetHashtag { get; set; }
        public DbSet<UserFollow> UserFollow { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserFollow>(entity =>
            {
                entity.HasKey(uf => uf.SerialNumber);

                entity.HasOne(uf => uf.Follower)
                    .WithMany(u => u.Following)
                    .HasForeignKey(uf => uf.FollowerUserName)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(uf => uf.Following)
                    .WithMany(u => u.Followers)
                    .HasForeignKey(uf => uf.FollowingUserName)
                    .OnDelete(DeleteBehavior.Restrict);
            });

        }
    }
}