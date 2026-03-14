using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Data;

public class AppWriteDbContext : DbContext
{
    public AppWriteDbContext(DbContextOptions<AppWriteDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<RevokedToken> RevokedTokens => Set<RevokedToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(b =>
        {
            b.ToTable("users");
            b.HasKey(u => u.Id);
            b.Property(u => u.Id).HasColumnName("id");
            b.Property(u => u.Name).HasColumnName("name").HasMaxLength(255).IsRequired();
            b.Property(u => u.Password).HasColumnName("password").HasMaxLength(255).IsRequired();
            b.HasIndex(u => u.Name).IsUnique();
        });

        modelBuilder.Entity<RevokedToken>(b =>
        {
            b.ToTable("revoked_tokens");
            b.HasKey(rt => rt.Jti);
            b.Property(rt => rt.Jti).HasColumnName("jti").HasMaxLength(64);
            b.Property(rt => rt.RevokedAt).HasColumnName("revoked_at");
            b.Property(rt => rt.ExpiresAt).HasColumnName("expires_at");
        });
    }
}
