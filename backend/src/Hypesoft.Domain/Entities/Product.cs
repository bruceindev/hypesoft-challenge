namespace Hypesoft.Domain.Entities;

public class Product
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public Guid CategoryId { get; private set; }
    public int StockQuantity { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public ProductStatus Status => CalculateStatus();

    private Product() { }

    public Product(
        string name,
        string description,
        decimal price,
        Guid categoryId,
        int stockQuantity)
    {
        Validate(name, price, stockQuantity, categoryId);

        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        Price = price;
        CategoryId = categoryId;
        StockQuantity = stockQuantity;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(string name, string description, decimal price)
    {
        ValidateName(name);
        ValidatePrice(price);

        Name = name;
        Description = description;
        Price = price;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStock(int quantity)
    {
        if (quantity < 0)
            throw new ArgumentException("Stock cannot be negative.");

        StockQuantity = quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    private ProductStatus CalculateStatus()
    {
        if (StockQuantity == 0)
            return ProductStatus.OutOfStock;

        if (StockQuantity < 10)
            return ProductStatus.LowStock;

        return ProductStatus.Active;
    }

    private void Validate(string name, decimal price, int stock, Guid categoryId)
    {
        ValidateName(name);
        ValidatePrice(price);

        if (stock < 0)
            throw new ArgumentException("Stock cannot be negative.");

        if (categoryId == Guid.Empty)
            throw new ArgumentException("Category is required.");
    }

    private void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name is required.");
    }

    private void ValidatePrice(decimal price)
    {
        if (price <= 0)
            throw new ArgumentException("Price must be greater than zero.");
    }
}