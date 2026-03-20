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
        public DbSet<OrganizationalUnit> OrganizationalUnits => Set<OrganizationalUnit>();
        public DbSet<RepositoryNode> RepositoryNodes => Set<RepositoryNode>();
        public DbSet<NodePermission> NodePermissions => Set<NodePermission>();
        public DbSet<DownloadAudit> DownloadAudits => Set<DownloadAudit>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DocumentItem>()
                .HasOne(x => x.DocumentFile)
                .WithOne(x => x.DocumentItem)
                .HasForeignKey<DocumentFile>(x => x.DocumentItemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrganizationalUnit>()
                .HasOne(x => x.Parent)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RepositoryNode>()
                .HasOne(x => x.Parent)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RepositoryNode>()
                .HasOne(x => x.OrganizationalUnit)
                .WithMany()
                .HasForeignKey(x => x.OrganizationalUnitId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DocumentItem>()
                .HasOne(x => x.RepositoryNode)
                .WithMany()
                .HasForeignKey(x => x.RepositoryNodeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<NodePermission>()
                .HasOne(x => x.RepositoryNode)
                .WithMany()
                .HasForeignKey(x => x.RepositoryNodeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<NodePermission>()
                .HasOne(x => x.OrganizationalUnit)
                .WithMany()
                .HasForeignKey(x => x.OrganizationalUnitId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<NodePermission>()
                .HasIndex(x => new { x.RepositoryNodeId, x.OrganizationalUnitId })
                .IsUnique();

            modelBuilder.Entity<DownloadAudit>()
                .HasOne(x => x.DocumentItem)
                .WithMany(x => x.DownloadAudits)
                .HasForeignKey(x => x.DocumentItemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DownloadAudit>()
                .HasOne(x => x.DocumentFile)
                .WithMany(x => x.DownloadAudits)
                .HasForeignKey(x => x.DocumentFileId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}