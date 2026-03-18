using Microsoft.EntityFrameworkCore;
using Ecu911.CatalogService.Models;

namespace Ecu911.CatalogService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<DocumentItem> DocumentItems => Set<DocumentItem>();
        public DbSet<DocumentType> DocumentTypes => Set<DocumentType>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<DocumentFile> DocumentFiles => Set<DocumentFile>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DocumentItem>()
                .HasOne(x => x.File)
                .WithOne(x => x.DocumentItem)
                .HasForeignKey<DocumentFile>(x => x.DocumentItemId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}