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
    public DbSet<AssetStatusHistory> AssetStatusHistories => Set<AssetStatusHistory>();
    public DbSet<AssetAssignmentHistory> AssetAssignmentHistories => Set<AssetAssignmentHistory>();
    public DbSet<Consumable> Consumables => Set<Consumable>();
    public DbSet<ConsumableAdjustment> ConsumableAdjustments => Set<ConsumableAdjustment>();
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
            entity.Property(asset => asset.AssetTag).IsRequired().HasMaxLength(100);
            entity.Property(asset => asset.SerialNumber).IsRequired().HasMaxLength(100);
            entity.Property(asset => asset.Type).IsRequired().HasMaxLength(100);
            entity.Property(asset => asset.Brand).IsRequired().HasMaxLength(100);
            entity.Property(asset => asset.Model).IsRequired().HasMaxLength(100);
            entity.Property(asset => asset.Description).HasMaxLength(1000);
            entity.Property(asset => asset.Notes).HasMaxLength(2000);
            entity.Property(asset => asset.Status).IsRequired();
            entity.HasIndex(asset => asset.AssetTag).IsUnique();
            entity.HasIndex(asset => asset.SerialNumber);
            entity.HasIndex(asset => asset.Type);
            entity.HasIndex(asset => asset.Brand);
            entity.HasQueryFilter(asset => !asset.IsDeleted);

            entity.HasOne(asset => asset.Location)
                .WithMany()
                .HasForeignKey(asset => asset.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(asset => asset.Supplier)
                .WithMany()
                .HasForeignKey(asset => asset.SupplierId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(asset => asset.AssignedEmployee)
                .WithMany()
                .HasForeignKey(asset => asset.AssignedEmployeeId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<AssetStatusHistory>(entity =>
        {
            entity.HasKey(history => history.Id);
            entity.Property(history => history.OperatorName).IsRequired().HasMaxLength(200);
            entity.Property(history => history.ToStatus).IsRequired();
            entity.Property(history => history.ChangedAtUtc).IsRequired();
            entity.HasIndex(history => history.AssetId);

            entity.HasOne(history => history.Asset)
                .WithMany()
                .HasForeignKey(history => history.AssetId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AssetAssignmentHistory>(entity =>
        {
            entity.HasKey(history => history.Id);
            entity.Property(history => history.OperatorName).IsRequired().HasMaxLength(200);
            entity.Property(history => history.AssignedAtUtc).IsRequired();
            entity.HasIndex(history => history.AssetId);
            entity.HasIndex(history => history.ToEmployeeId);

            entity.HasOne(history => history.Asset)
                .WithMany()
                .HasForeignKey(history => history.AssetId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(history => history.FromEmployee)
                .WithMany()
                .HasForeignKey(history => history.FromEmployeeId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(history => history.ToEmployee)
                .WithMany()
                .HasForeignKey(history => history.ToEmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Consumable>(entity =>
        {
            entity.HasKey(consumable => consumable.Id);
            entity.Property(consumable => consumable.Name).IsRequired().HasMaxLength(200);
            entity.Property(consumable => consumable.Category).IsRequired().HasMaxLength(100);
            entity.Property(consumable => consumable.Unit).IsRequired().HasMaxLength(50);
            entity.Property(consumable => consumable.QuantityOnHand).IsRequired();
            entity.Property(consumable => consumable.ReorderLevel).IsRequired();
            entity.HasQueryFilter(consumable => !consumable.IsDeleted);

            entity.HasOne(consumable => consumable.Location)
                .WithMany()
                .HasForeignKey(consumable => consumable.LocationId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<ConsumableAdjustment>(entity =>
        {
            entity.HasKey(adjustment => adjustment.Id);
            entity.Property(adjustment => adjustment.QuantityChange).IsRequired();
            entity.Property(adjustment => adjustment.Reason).IsRequired().HasMaxLength(500);
            entity.Property(adjustment => adjustment.AdjustedAtUtc).IsRequired();
            entity.Property(adjustment => adjustment.OperatorName).IsRequired().HasMaxLength(200);
            entity.HasIndex(adjustment => adjustment.ConsumableId);

            entity.HasOne(adjustment => adjustment.Consumable)
                .WithMany()
                .HasForeignKey(adjustment => adjustment.ConsumableId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Repair>(entity =>
        {
            entity.HasKey(repair => repair.Id);
            entity.Property(repair => repair.AssetId).IsRequired();
            entity.Property(repair => repair.Vendor).IsRequired().HasMaxLength(200);
            entity.Property(repair => repair.Cost).HasColumnType("decimal(18,2)");
            entity.Property(repair => repair.Notes).HasMaxLength(2000);
            entity.Property(repair => repair.Status).IsRequired();
            entity.Property(repair => repair.LoggedAtUtc).IsRequired();
            entity.HasIndex(repair => repair.AssetId);
            entity.HasQueryFilter(repair => !repair.IsDeleted);

            entity.HasOne(repair => repair.Asset)
                .WithMany()
                .HasForeignKey(repair => repair.AssetId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(audit => audit.Id);
            entity.Property(audit => audit.Action).IsRequired().HasMaxLength(200);
            entity.Property(audit => audit.EntityName).IsRequired().HasMaxLength(200);
            entity.Property(audit => audit.EntityId).IsRequired().HasMaxLength(100);
            entity.Property(audit => audit.OperatorName).IsRequired().HasMaxLength(200);
            entity.Property(audit => audit.Summary).IsRequired().HasMaxLength(500);
            entity.Property(audit => audit.OccurredAtUtc).IsRequired();
            entity.HasQueryFilter(audit => !audit.IsDeleted);
        });
    }
}
