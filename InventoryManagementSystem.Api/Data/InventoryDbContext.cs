using InventoryManagementSystem.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Api.Data;

public sealed class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
        : base(options)
    {
    }

    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Asset> Assets => Set<Asset>();
    public DbSet<Consumable> Consumables => Set<Consumable>();
    public DbSet<Repair> Repairs => Set<Repair>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(location => location.Id);
            entity.Property(location => location.Name).IsRequired().HasMaxLength(200);
            entity.Property(location => location.Code).IsRequired().HasMaxLength(50);
            entity.Property(location => location.Description).HasMaxLength(500);
            entity.Property(location => location.AddressLine1).HasMaxLength(200);
            entity.Property(location => location.City).HasMaxLength(100);
            entity.Property(location => location.State).HasMaxLength(100);
            entity.Property(location => location.Country).HasMaxLength(100);
            entity.HasIndex(location => location.Code).IsUnique();
            entity.HasQueryFilter(location => !location.IsDeleted);
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(supplier => supplier.Id);
            entity.Property(supplier => supplier.Name).IsRequired().HasMaxLength(200);
            entity.Property(supplier => supplier.Code).IsRequired().HasMaxLength(50);
            entity.Property(supplier => supplier.Email).HasMaxLength(200);
            entity.Property(supplier => supplier.Phone).HasMaxLength(50);
            entity.HasIndex(supplier => supplier.Code).IsUnique();
            entity.HasQueryFilter(supplier => !supplier.IsDeleted);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(employee => employee.Id);
            entity.Property(employee => employee.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(employee => employee.LastName).IsRequired().HasMaxLength(100);
            entity.Property(employee => employee.Email).HasMaxLength(200);
            entity.Property(employee => employee.StaffNumber).HasMaxLength(50);
            entity.HasIndex(employee => employee.Email).IsUnique().HasFilter("[Email] IS NOT NULL");
            entity.HasIndex(employee => employee.StaffNumber).IsUnique().HasFilter("[StaffNumber] IS NOT NULL");
            entity.HasQueryFilter(employee => !employee.IsDeleted);
        });

        modelBuilder.Entity<Asset>(entity =>
        {
            entity.HasKey(asset => asset.Id);
            entity.Property(asset => asset.Name).IsRequired().HasMaxLength(200);
            entity.Property(asset => asset.AssetTag).IsRequired().HasMaxLength(100);
            entity.HasIndex(asset => asset.AssetTag).IsUnique();
            entity.HasQueryFilter(asset => !asset.IsDeleted);
        });

        modelBuilder.Entity<Consumable>(entity =>
        {
            entity.HasKey(consumable => consumable.Id);
            entity.Property(consumable => consumable.Name).IsRequired().HasMaxLength(200);
            entity.Property(consumable => consumable.Sku).IsRequired().HasMaxLength(100);
            entity.HasIndex(consumable => consumable.Sku).IsUnique();
            entity.HasQueryFilter(consumable => !consumable.IsDeleted);
        });

        modelBuilder.Entity<Repair>(entity =>
        {
            entity.HasKey(repair => repair.Id);
            entity.Property(repair => repair.Description).IsRequired().HasMaxLength(500);
            entity.HasQueryFilter(repair => !repair.IsDeleted);
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(audit => audit.Id);
            entity.Property(audit => audit.Action).IsRequired().HasMaxLength(200);
            entity.Property(audit => audit.EntityName).IsRequired().HasMaxLength(200);
            entity.Property(audit => audit.EntityId).IsRequired().HasMaxLength(100);
            entity.HasQueryFilter(audit => !audit.IsDeleted);
        });
    }
}
