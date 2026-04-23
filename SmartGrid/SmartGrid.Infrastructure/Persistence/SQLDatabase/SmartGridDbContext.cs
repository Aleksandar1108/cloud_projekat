using Microsoft.EntityFrameworkCore;
using SmartGrid.Infrastructure.Persistence.SQLDatabase.Entities;

public class SmartGridDbContext : DbContext
{
    public SmartGridDbContext(DbContextOptions<SmartGridDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserEntity> Users { get; set; } = null!;
    public DbSet<TariffModelEntity> TariffModels { get; set; } = null!;
    public DbSet<MonthlyBillEntity> MonthlyBills { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.HasKey(e => e.IdUsers);

            entity.Property(e => e.Email)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(e => e.Password)
                  .IsRequired();

            entity.Property(e => e.Role)
                  .IsRequired();

            entity.Property(e => e.AccountCreated)
                  .IsRequired();
        });

        modelBuilder.Entity<TariffModelEntity>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                  .IsRequired()
                  .HasMaxLength(80);

            entity.Property(e => e.IsActive)
                  .IsRequired();

            entity.Property(e => e.CreatedAt)
                  .IsRequired();

            entity.Property(e => e.GreenZoneVtPrice).IsRequired();
            entity.Property(e => e.GreenZoneNtPrice).IsRequired();
            entity.Property(e => e.BlueZoneVtPrice).IsRequired();
            entity.Property(e => e.BlueZoneNtPrice).IsRequired();
            entity.Property(e => e.RedZoneVtPrice).IsRequired();
            entity.Property(e => e.RedZoneNtPrice).IsRequired();
            entity.Property(e => e.NetworkCostPerKw).IsRequired();
            entity.Property(e => e.SupplierCost).IsRequired();
            entity.Property(e => e.ApprovedPowerKw).IsRequired();
        });

        modelBuilder.Entity<MonthlyBillEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DeviceId).IsRequired().HasMaxLength(128);
            entity.Property(e => e.Year).IsRequired();
            entity.Property(e => e.Month).IsRequired();
            entity.Property(e => e.TotalKwh).IsRequired();
            entity.Property(e => e.HigherTariffKwh).IsRequired();
            entity.Property(e => e.LowerTariffKwh).IsRequired();
            entity.Property(e => e.GreenZoneKwh).IsRequired();
            entity.Property(e => e.BlueZoneKwh).IsRequired();
            entity.Property(e => e.RedZoneKwh).IsRequired();
            entity.Property(e => e.EnergyCost).IsRequired();
            entity.Property(e => e.FixedCosts).IsRequired();
            entity.Property(e => e.TotalCost).IsRequired();
            entity.Property(e => e.BillText).IsRequired();
            entity.Property(e => e.GeneratedAtUtc).IsRequired();
            entity.HasIndex(e => new { e.DeviceId, e.Year, e.Month }).IsUnique();
        });
    }
}