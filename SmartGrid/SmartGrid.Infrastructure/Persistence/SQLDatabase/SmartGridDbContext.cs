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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.IdUsers);

            entity.Property(e => e.IdUsers).HasColumnName("idUsers");
            entity.Property(e => e.Email)
                  .HasColumnName("email")
                  .IsRequired()
                  .HasMaxLength(320);

            entity.Property(e => e.Password)
                  .HasColumnName("password")
                  .IsRequired();

            entity.Property(e => e.Role)
                  .HasColumnName("role")
                  .IsRequired();

            entity.Property(e => e.AccountCreated)
                  .HasColumnName("accountCreated")
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
    }
}
