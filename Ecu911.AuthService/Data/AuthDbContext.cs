using Ecu911.AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecu911.AuthService.Data;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<SystemModule> SystemModules => Set<SystemModule>();
    public DbSet<UserSystemRole> UserSystemRoles => Set<UserSystemRole>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<UserSystemScope> UserSystemScopes => Set<UserSystemScope>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(x => x.Username).IsUnique();
            entity.HasIndex(x => x.Email).IsUnique();
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(x => new { x.UserId, x.RoleId });

            entity.HasOne(x => x.User)
                .WithMany(x => x.UserRoles)
                .HasForeignKey(x => x.UserId);

            entity.HasOne(x => x.Role)
                .WithMany(x => x.UserRoles)
                .HasForeignKey(x => x.RoleId);
        });

        modelBuilder.Entity<SystemModule>(entity =>
        {
            entity.HasIndex(x => x.Code).IsUnique();
            entity.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<UserSystemRole>(entity =>
        {
            entity.HasKey(x => new { x.UserId, x.RoleId, x.SystemModuleId });

            entity.HasOne(x => x.User)
                .WithMany(x => x.UserSystemRoles)
                .HasForeignKey(x => x.UserId);

            entity.HasOne(x => x.Role)
                .WithMany(x => x.UserSystemRoles)
                .HasForeignKey(x => x.RoleId);

            entity.HasOne(x => x.SystemModule)
                .WithMany(x => x.UserSystemRoles)
                .HasForeignKey(x => x.SystemModuleId);
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasIndex(x => x.Code).IsUnique();
            entity.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(x => new { x.RoleId, x.PermissionId });

            entity.HasOne(x => x.Role)
                .WithMany(x => x.RolePermissions)
                .HasForeignKey(x => x.RoleId);

            entity.HasOne(x => x.Permission)
                .WithMany(x => x.RolePermissions)
                .HasForeignKey(x => x.PermissionId);
        });

        modelBuilder.Entity<UserSystemScope>(entity =>
        {
            entity.HasOne(x => x.User)
                .WithMany(x => x.UserSystemScopes)
                .HasForeignKey(x => x.UserId);

            entity.HasOne(x => x.SystemModule)
                .WithMany(x => x.UserSystemScopes)
                .HasForeignKey(x => x.SystemModuleId);

            entity.Property(x => x.ScopeLevel)
                .HasMaxLength(50)
                .IsRequired();
        });
    }
}