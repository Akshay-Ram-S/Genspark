using AuctionAPI.Models;
using AuctionAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AuctionAPI.Contexts
{
    public class AuctionContext : DbContext
    {
        public AuctionContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Bidder> Bidders { get; set; }
        public DbSet<Seller> Sellers { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemDetails> ItemDetails { get; set; }
        public DbSet<Bid> Bids { get; set; }
        public DbSet<ItemAllBids> ItemBids{ get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Audit> Audits { get; set; }


        public async Task<List<ItemAllBids>> GetBidsByItem(Guid id)
        {
            return await this.Set<ItemAllBids>()
                        .FromSqlInterpolated($"select * from fn_get_bids_by_item({id})")
                        .ToListAsync();
        }

        // For stored function usage
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasKey(u => u.UserId);

            modelBuilder.Entity<User>()
                .Property(u => u.Name)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.Password)
                .IsRequired();


            modelBuilder.Entity<Seller>()
                .HasKey(s => s.SellerId);

            modelBuilder.Entity<Seller>()
                .HasOne(s => s.User)
                .WithOne(u => u.Seller)
                .HasForeignKey<Seller>(s => s.UserId)
                .HasPrincipalKey<User>(u => u.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Seller>()
                .HasMany(s => s.Items)
                .WithOne(i => i.Seller)
                .HasForeignKey(i => i.SellerID)
                .OnDelete(DeleteBehavior.Cascade);

  
            modelBuilder.Entity<Bidder>()
                .HasKey(b => b.BidderId);

            modelBuilder.Entity<Bidder>()
                .HasOne(b => b.User)
                .WithOne(u => u.Bidder)
                .HasForeignKey<Bidder>(b => b.UserId)
                .HasPrincipalKey<User>(u => u.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Bidder>()
                .HasMany(b => b.Bids)
                .WithOne(bid => bid.Bidder)
                .HasForeignKey(bid => bid.BidderId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Item>()
                .HasKey(i => i.Id);

            modelBuilder.Entity<Item>()
                .Property(i => i.Title)
                .IsRequired();

            modelBuilder.Entity<Item>()
                .HasOne(i => i.Seller)
                .WithMany(s => s.Items)
                .HasForeignKey(i => i.SellerID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Item>()
                .HasMany(i => i.Bids)
                .WithOne(b => b.Item)
                .HasForeignKey(b => b.ItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Item>()
                .HasOne(i => i.ItemDetails)
                .WithOne(d => d.Item)
                .HasForeignKey<ItemDetails>(d => d.ItemId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<ItemDetails>()
                .HasKey(d => d.ItemId);

            modelBuilder.Entity<ItemDetails>()
                .Property(i => i.StartingPrice)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<ItemDetails>()
                .Property(d => d.Description)
                .IsRequired();

            modelBuilder.Entity<ItemDetails>()
                .Property(d => d.ImageMimeType)
                .HasMaxLength(100);

            modelBuilder.Entity<ItemDetails>()
                .Property(d => d.CurrentBid)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<ItemDetails>()
                .HasOne(d => d.Bidder)
                .WithMany()
                .HasForeignKey(d => d.CurrentBidderID)
                .OnDelete(DeleteBehavior.SetNull);

 
            modelBuilder.Entity<Bid>()
                .HasKey(b => b.Id);

            modelBuilder.Entity<Bid>()
                .Property(b => b.Amount)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Bid>()
                .HasOne(b => b.Item)
                .WithMany(i => i.Bids)
                .HasForeignKey(b => b.ItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Bid>()
                .HasOne(b => b.Bidder)
                .WithMany(bd => bd.Bids)
                .HasForeignKey(b => b.BidderId)
                .OnDelete(DeleteBehavior.Cascade);
           
            modelBuilder.Entity<ItemAllBids>().HasNoKey();
        }

    }
}
