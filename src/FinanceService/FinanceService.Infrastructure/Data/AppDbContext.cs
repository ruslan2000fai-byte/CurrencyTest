using FinanceService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceService.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Currency> Currencies => Set<Currency>();
    public DbSet<UserFavorite> UserFavorites => Set<UserFavorite>();
    public DbSet<RevokedToken> RevokedTokens => Set<RevokedToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Currency>(b =>
        {
            b.ToTable("currency");
            b.HasKey(c => c.Id);
            b.Property(c => c.Id).HasColumnName("id");
            b.Property(c => c.Name).HasColumnName("name").HasMaxLength(255).IsRequired();
            b.Property(c => c.CharCode).HasColumnName("char_code").HasMaxLength(10).IsRequired();
            b.Property(c => c.Nominal).HasColumnName("nominal");
            b.Property(c => c.Rate).HasColumnName("rate").HasPrecision(18, 4);
            b.HasIndex(c => c.CharCode).IsUnique();
        });

        modelBuilder.Entity<UserFavorite>(b =>
        {
            b.ToTable("user_favorite_currencies");
            b.HasKey(f => new { f.UserId, f.CurrencyId });
            b.Property(f => f.UserId).HasColumnName("user_id");
            b.Property(f => f.CurrencyId).HasColumnName("currency_id");
            b.HasOne(f => f.Currency).WithMany().HasForeignKey(f => f.CurrencyId);
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
