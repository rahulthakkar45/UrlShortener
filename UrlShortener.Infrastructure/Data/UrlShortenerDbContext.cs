using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Domain.Entitites;

namespace UrlShortener.Infrastructure.Data
{
    public class UrlShortenerDbContext : DbContext
    {
        public UrlShortenerDbContext(DbContextOptions<UrlShortenerDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<ShortUrl> ShortUrls { get; set; }
        public DbSet<Click> Clicks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.HasIndex(e => e.PasswordHash).IsUnique();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.HasMany(e => e.ShortUrls)
                      .WithOne(u => u.User)
                      .HasForeignKey(su => su.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<ShortUrl>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.OriginalUrl).IsRequired();
                entity.Property(e => e.ShortCode).IsRequired().HasMaxLength(25);
                entity.HasIndex(e => e.ShortCode).IsUnique();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.ExpirationDate).IsRequired();
                entity.Property(e => e.IsActive).IsRequired();
                entity.HasOne(e => e.User)
                      .WithMany(u => u.ShortUrls)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(e => e.Clicks)
                      .WithOne(c => c.ShortUrl)
                      .HasForeignKey(c => c.ShortUrlId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<Click>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ShortUrlId).IsRequired();
                entity.HasIndex(e => e.ShortUrlId);
                entity.Property(e => e.ClickedAt).IsRequired();
                entity.HasOne(e => e.ShortUrl)
                      .WithMany(su => su.Clicks)
                      .HasForeignKey(e => e.ShortUrlId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }

}
