using DocSharingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DocSharingAPI.Contexts
{
    public class DocumentContext : DbContext
    {

        public DocumentContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Document> Documents { get; set; }
        public DbSet<User> Users { get; set; }
        

    }
}