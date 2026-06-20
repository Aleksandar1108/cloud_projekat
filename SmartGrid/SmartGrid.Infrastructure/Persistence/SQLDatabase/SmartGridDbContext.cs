using Microsoft.EntityFrameworkCore;
using SmartGrid.Infrastructure.Persistence.SQLDatabase.Entities;

public class SmartGridDbContext : DbContext
{
    public SmartGridDbContext(DbContextOptions<SmartGridDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserEntity> Users { get; set; } = null!;
    public DbSet<EmailActivationEntity> EmailActivations { get; set; } = null!;
    public DbSet<TariffModelEntity> TariffModels { get; set; } = null!;
    public DbSet<PropertyEntity> Properties { get; set; } = null!;
    public DbSet<SmartMeterEntity> SmartMeters { get; set; } = null!;

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
            entity.Property(e => e.IsActive)
                  .IsRequired()
                  .HasColumnName("isActivated");
        });
        modelBuilder.Entity<EmailActivationEntity>(entity =>
        {
            entity.HasKey(e => e.IdEmailActivation);

            entity.Property(e => e.IdEmailActivation)
                  .HasColumnName("idEmailActivation");

            entity.Property(e => e.UserId)
                  .HasColumnName("idUsers");

            entity.Property(e => e.ActivationToken)
                  .HasColumnName("activationToken");

            entity.Property(e => e.CreatedAt)
                  .HasColumnName("createdAt");

            entity.Property(e => e.ExpirationDate)
                  .HasColumnName("expireAt");

            entity.HasOne<UserEntity>()
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .HasPrincipalKey(u => u.IdUsers);
        });

        modelBuilder.Entity<PropertyEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.City).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Address).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.PropertyType).IsRequired().HasMaxLength(20);
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasOne<UserEntity>()
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .HasPrincipalKey(u => u.IdUsers);
        });

        modelBuilder.Entity<SmartMeterEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PropertyId).IsRequired();
            entity.Property(e => e.Label).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ConnectionType).IsRequired().HasMaxLength(20);
            entity.Property(e => e.MaxApprovedPower).IsRequired();
            entity.Property(e => e.Note).HasMaxLength(500);
            entity.Property(e => e.SerialNumber).HasMaxLength(20);
            entity.Property(e => e.PairingStatus).IsRequired().HasMaxLength(20);
            entity.Property(e => e.DeviceUUID).HasMaxLength(50);
            entity.Property(e => e.AccessToken).HasMaxLength(255);
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasOne<PropertyEntity>()
                  .WithMany()
                  .HasForeignKey(e => e.PropertyId)
                  .HasPrincipalKey(p => p.Id);
        });

        modelBuilder.Entity<TariffModelEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).IsRequired().HasMaxLength(80).HasColumnName("name");
            entity.Property(e => e.IsActive).IsRequired().HasColumnName("isActive");
            entity.Property(e => e.CreatedAt).IsRequired().HasColumnName("createdAt");
            entity.Property(e => e.GreenZoneVtPrice).IsRequired().HasColumnName("greenZoneVtPrice");
            entity.Property(e => e.GreenZoneNtPrice).IsRequired().HasColumnName("greenZoneNtPrice");
            entity.Property(e => e.BlueZoneVtPrice).IsRequired().HasColumnName("blueZoneVtPrice");
            entity.Property(e => e.BlueZoneNtPrice).IsRequired().HasColumnName("blueZoneNtPrice");
            entity.Property(e => e.RedZoneVtPrice).IsRequired().HasColumnName("redZoneVtPrice");
            entity.Property(e => e.RedZoneNtPrice).IsRequired().HasColumnName("redZoneNtPrice");
            entity.Property(e => e.NetworkCostPerKw).IsRequired().HasColumnName("networkCostPerKw");
            entity.Property(e => e.SupplierCost).IsRequired().HasColumnName("supplierCost");
            entity.Property(e => e.ApprovedPowerKw).IsRequired().HasColumnName("approvedPowerKw");
            entity.Property(e => e.GreenZoneMaxKwh).IsRequired().HasColumnName("greenZoneMaxKwh");
            entity.Property(e => e.BlueZoneMaxKwh).IsRequired().HasColumnName("blueZoneMaxKwh");
            entity.Property(e => e.UpdatedAt).IsRequired().HasColumnName("updatedAt");
        });

    }
}