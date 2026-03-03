using Hypesoft.Domain.Common;
using Hypesoft.Domain.Events;
using Hypesoft.Domain.ValueObjects;

namespace Hypesoft.Domain.Entities;

public sealed class Product : AggregateRoot
{
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public Price Price { get; private set; } = null!;
    public StockQuantity StockQuantity { get; private set; } = null!;
    public string CategoryId { get; private set; } = null!;
    public string? ImageUrl { get; private set; }
    public bool IsActive { get; private set; }

    private Product() { }

    private Product(
        string name,
        string description,
        Price price,
        StockQuantity stockQuantity,
        string categoryId,
        string? imageUrl = null)
    {
        Name = name;
        Description = description;
        Price = price;
        StockQuantity = stockQuantity;
        CategoryId = categoryId;
        ImageUrl = imageUrl;
        IsActive = true;
    }

    public static Product Create(
        string name,
        string description,
        decimal priceAmount,
        int stockQuantity,
        string categoryId,
        string? imageUrl = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name is required", nameof(name));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Product description is required", nameof(description));

        if (string.IsNullOrWhiteSpace(categoryId))
            throw new ArgumentException("Category is required", nameof(categoryId));

        var price = Price.Create(priceAmount);
        var stock = StockQuantity.Create(stockQuantity);

        var product = new Product(name, description, price, stock, categoryId, imageUrl);

        if (stock.IsLowStock())
        {
            product.AddDomainEvent(new LowStockDetectedEvent(product.Id, product.Name, stock.Value));
        }

        return product;
    }

    public void Update(
        string name,
        string description,
        decimal priceAmount,
        string categoryId,
        string? imageUrl = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name is required", nameof(name));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Product description is required", nameof(description));

        if (string.IsNullOrWhiteSpace(categoryId))
            throw new ArgumentException("Category is required", nameof(categoryId));

        Name = name;
        Description = description;
        Price = Price.Create(priceAmount);
        CategoryId = categoryId;
        ImageUrl = imageUrl;
        SetUpdatedAt();
    }

    public void UpdateStock(int newQuantity)
    {
        var previousStock = StockQuantity;
        StockQuantity = StockQuantity.Create(newQuantity);

        if (StockQuantity.IsLowStock() && !previousStock.IsLowStock())
        {
            AddDomainEvent(new LowStockDetectedEvent(Id, Name, StockQuantity.Value));
        }

        SetUpdatedAt();
    }

    public void IncreaseStock(int amount)
    {
        StockQuantity = StockQuantity.Increase(amount);
        SetUpdatedAt();
    }

    public void DecreaseStock(int amount)
    {
        StockQuantity = StockQuantity.Decrease(amount);

        if (StockQuantity.IsLowStock())
        {
            AddDomainEvent(new LowStockDetectedEvent(Id, Name, StockQuantity.Value));
        }

        SetUpdatedAt();
    }

    public void Activate()
    {
        IsActive = true;
        SetUpdatedAt();
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdatedAt();
    }
}