using Microsoft.EntityFrameworkCore;
using Ecu911.CatalogService.Models;

namespace Ecu911.CatalogService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<DocumentItem> DocumentItems => Set<DocumentItem>();
    public DbSet<DocumentType> DocumentTypes => Set<DocumentType>();
}