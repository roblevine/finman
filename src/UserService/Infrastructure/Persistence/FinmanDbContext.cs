using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;
using UserService.Domain.ValueObjects;

namespace UserService.Infrastructure.Persistence;

public class FinmanDbContext : DbContext
{
    public FinmanDbContext(DbContextOptions<FinmanDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("public");

        // Ensure citext extension is enabled when applying migrations
        modelBuilder.HasPostgresExtension("citext");

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            entity.HasKey(u => u.Id);

            entity.Property(u => u.Id)
                .HasColumnName("id")
                .ValueGeneratedNever();

            // Email as citext for case-insensitive comparison
            entity.OwnsOne(u => u.Email, owned =>
            {
                owned.Property(e => e.Value)
                    .HasColumnName("email")
                    .HasColumnType("citext")
                    .IsRequired();

                owned.WithOwner();
            });

            // Username as citext for case-insensitive comparison
            entity.OwnsOne(u => u.Username, owned =>
            {
                owned.Property(e => e.Value)
                    .HasColumnName("username")
                    .HasColumnType("citext")
                    .IsRequired();

                owned.WithOwner();
            });

            entity.Property(u => u.PasswordHash)
                .HasColumnName("password_hash")
                .HasMaxLength(200)
                .IsRequired();

            entity.OwnsOne(u => u.FirstName, owned =>
            {
                owned.Property(p => p.Value)
                    .HasColumnName("first_name")
                    .HasMaxLength(50)
                    .IsRequired();
                owned.WithOwner();
            });

            entity.OwnsOne(u => u.LastName, owned =>
            {
                owned.Property(p => p.Value)
                    .HasColumnName("last_name")
                    .HasMaxLength(50)
                    .IsRequired();
                owned.WithOwner();
            });

            entity.Property(u => u.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            entity.Property(u => u.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            entity.Property(u => u.IsActive)
                .HasColumnName("is_active")
                .IsRequired();

            entity.Property(u => u.IsDeleted)
                .HasColumnName("is_deleted")
                .IsRequired();

            entity.Property(u => u.DeletedAt)
                .HasColumnName("deleted_at")
                .HasColumnType("timestamp with time zone")
                .IsRequired(false);

            entity.HasIndex("email").IsUnique();
            entity.HasIndex("username").IsUnique();
        });
    }
}
