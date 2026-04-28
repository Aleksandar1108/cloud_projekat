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

    }
}