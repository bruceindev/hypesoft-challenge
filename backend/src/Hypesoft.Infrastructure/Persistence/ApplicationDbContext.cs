using Microsoft.EntityFrameworkCore;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.ValueObjects;
using MongoDB.EntityFrameworkCore.Extensions;
using System.Globalization;

namespace Hypesoft.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToCollection("products");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedNever();

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(1000);

            entity.Property(e => e.CategoryId)
                .IsRequired();

            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500);

            entity.Property(e => e.Price)
                .IsRequired()
                .HasConversion(
                    value => ConvertPriceToStorage(value),
                    stored => ConvertPriceFromStorage(stored));

            entity.Property(e => e.StockQuantity)
                .IsRequired()
                .HasConversion(
                    value => ConvertStockToStorage(value),
                    stored => ConvertStockFromStorage(stored));

            entity.Ignore(e => e.DomainEvents);

            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.CategoryId);
            entity.HasIndex(e => e.IsActive);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToCollection("categories");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedNever();

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(500);

            entity.Ignore(e => e.DomainEvents);

            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.IsActive);
        });
    }

    private static string ConvertPriceToStorage(Price value)
    {
        return $"{value.Currency}|{value.Amount.ToString(CultureInfo.InvariantCulture)}";
    }

    private static Price ConvertPriceFromStorage(string stored)
    {
        var parts = stored.Split('|', 2);
        if (parts.Length == 2)
        {
            var currency = parts[0];
            var amount = decimal.Parse(parts[1], CultureInfo.InvariantCulture);
            return Price.Create(amount, currency);
        }

        var fallbackAmount = decimal.Parse(stored, CultureInfo.InvariantCulture);
        return Price.Create(fallbackAmount, "BRL");
    }

    private static int ConvertStockToStorage(StockQuantity value)
    {
        return value.Value;
    }

    private static StockQuantity ConvertStockFromStorage(int stored)
    {
        return StockQuantity.Create(stored);
    }
}