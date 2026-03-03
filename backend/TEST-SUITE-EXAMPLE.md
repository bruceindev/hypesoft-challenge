# Complete Test Suite Example

## Create Test Project

```bash
# Create test project
dotnet new xunit -n Hypesoft.Tests -o tests/Hypesoft.Tests

# Add packages
cd tests/Hypesoft.Tests
dotnet add package FluentAssertions --version 6.12.0
dotnet add package Moq --version 4.20.70
dotnet add package Microsoft.AspNetCore.Mvc.Testing --version 9.0.0

# Add project references
dotnet add reference ../../src/Hypesoft.Domain
dotnet add reference ../../src/Hypesoft.Application

# Add to solution
cd ../..
dotnet sln add tests/Hypesoft.Tests/Hypesoft.Tests.csproj
```

## Domain Tests

### ProductTests.cs
```csharp
using FluentAssertions;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Events;
using Xunit;

namespace Hypesoft.Tests.Domain;

public class ProductTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateProduct()
    {
        // Arrange
        var name = "Test Product";
        var description = "Test Description";
        var price = 99.99m;
        var stock = 10;
        var categoryId = "507f1f77bcf86cd799439011";

        // Act
        var product = Product.Create(name, description, price, stock, categoryId);

        // Assert
        product.Should().NotBeNull();
        product.Name.Should().Be(name);
        product.Description.Should().Be(description);
        product.Price.Amount.Should().Be(price);
        product.StockQuantity.Value.Should().Be(stock);
        product.CategoryId.Should().Be(categoryId);
        product.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_WithEmptyName_ShouldThrowException()
    {
        // Arrange & Act
        var act = () => Product.Create("", "Description", 99.99m, 10, "categoryId");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*name is required*");
    }

    [Fact]
    public void Create_WithNegativePrice_ShouldThrowException()
    {
        // Arrange & Act
        var act = () => Product.Create("Product", "Description", -10m, 10, "categoryId");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*cannot be negative*");
    }

    [Fact]
    public void Create_WithLowStock_ShouldRaiseLowStockEvent()
    {
        // Arrange & Act
        var product = Product.Create("Product", "Description", 99.99m, 5, "categoryId");

        // Assert
        product.DomainEvents.Should().ContainSingle();
        product.DomainEvents.First().Should().BeOfType<LowStockDetectedEvent>();
        
        var lowStockEvent = (LowStockDetectedEvent)product.DomainEvents.First();
        lowStockEvent.CurrentStock.Should().Be(5);
        lowStockEvent.ProductName.Should().Be("Product");
    }

    [Fact]
    public void UpdateStock_ToLowValue_ShouldRaiseLowStockEvent()
    {
        // Arrange
        var product = Product.Create("Product", "Description", 99.99m, 50, "categoryId");
        product.ClearDomainEvents();

        // Act
        product.UpdateStock(5);

        // Assert
        product.StockQuantity.Value.Should().Be(5);
        product.DomainEvents.Should().ContainSingle();
        product.DomainEvents.First().Should().BeOfType<LowStockDetectedEvent>();
    }

    [Fact]
    public void DecreaseStock_BelowZero_ShouldThrowException()
    {
        // Arrange
        var product = Product.Create("Product", "Description", 99.99m, 5, "categoryId");

        // Act
        var act = () => product.DecreaseStock(10);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Insufficient stock*");
    }

    [Fact]
    public void Update_WithValidData_ShouldUpdateProduct()
    {
        // Arrange
        var product = Product.Create("Old Name", "Old Description", 50m, 10, "oldCategoryId");
        var newName = "New Name";
        var newDescription = "New Description";
        var newPrice = 75m;
        var newCategoryId = "newCategoryId";

        // Act
        product.Update(newName, newDescription, newPrice, newCategoryId);

        // Assert
        product.Name.Should().Be(newName);
        product.Description.Should().Be(newDescription);
        product.Price.Amount.Should().Be(newPrice);
        product.CategoryId.Should().Be(newCategoryId);
        product.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalse()
    {
        // Arrange
        var product = Product.Create("Product", "Description", 99.99m, 10, "categoryId");

        // Act
        product.Deactivate();

        // Assert
        product.IsActive.Should().BeFalse();
        product.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Activate_ShouldSetIsActiveToTrue()
    {
        // Arrange
        var product = Product.Create("Product", "Description", 99.99m, 10, "categoryId");
        product.Deactivate();

        // Act
        product.Activate();

        // Assert
        product.IsActive.Should().BeTrue();
    }
}
```

### PriceValueObjectTests.cs
```csharp
using FluentAssertions;
using Hypesoft.Domain.ValueObjects;
using Xunit;

namespace Hypesoft.Tests.Domain;

public class PriceValueObjectTests
{
    [Fact]
    public void Create_WithValidAmount_ShouldCreatePrice()
    {
        // Arrange & Act
        var price = Price.Create(99.99m);

        // Assert
        price.Amount.Should().Be(99.99m);
        price.Currency.Should().Be("BRL");
    }

    [Fact]
    public void Create_WithNegativeAmount_ShouldThrowException()
    {
        // Arrange & Act
        var act = () => Price.Create(-10m);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*cannot be negative*");
    }

    [Fact]
    public void Create_WithCustomCurrency_ShouldUseThatCurrency()
    {
        // Arrange & Act
        var price = Price.Create(100m, "USD");

        // Assert
        price.Amount.Should().Be(100m);
        price.Currency.Should().Be("USD");
    }

    [Fact]
    public void ApplyDiscount_WithValidPercentage_ShouldReducePrice()
    {
        // Arrange
        var price = Price.Create(100m);

        // Act
        var discountedPrice = price.ApplyDiscount(10);

        // Assert
        discountedPrice.Amount.Should().Be(90m);
        discountedPrice.Currency.Should().Be("BRL");
    }

    [Fact]
    public void ApplyDiscount_WithInvalidPercentage_ShouldThrowException()
    {
        // Arrange
        var price = Price.Create(100m);

        // Act
        var act = () => price.ApplyDiscount(150);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*between 0 and 100*");
    }

    [Fact]
    public void Equals_WithSameValues_ShouldReturnTrue()
    {
        // Arrange
        var price1 = Price.Create(99.99m, "BRL");
        var price2 = Price.Create(99.99m, "BRL");

        // Act & Assert
        price1.Should().Be(price2);
        (price1 == price2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentValues_ShouldReturnFalse()
    {
        // Arrange
        var price1 = Price.Create(99.99m);
        var price2 = Price.Create(50m);

        // Act & Assert
        price1.Should().NotBe(price2);
        (price1 != price2).Should().BeTrue();
    }
}
```

### StockQuantityValueObjectTests.cs
```csharp
using FluentAssertions;
using Hypesoft.Domain.ValueObjects;
using Xunit;

namespace Hypesoft.Tests.Domain;

public class StockQuantityValueObjectTests
{
    [Fact]
    public void Create_WithValidQuantity_ShouldCreateStockQuantity()
    {
        // Arrange & Act
        var stock = StockQuantity.Create(50);

        // Assert
        stock.Value.Should().Be(50);
    }

    [Fact]
    public void Create_WithNegativeQuantity_ShouldThrowException()
    {
        // Arrange & Act
        var act = () => StockQuantity.Create(-5);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*cannot be negative*");
    }

    [Fact]
    public void IsLowStock_WithQuantityAboveThreshold_ShouldReturnFalse()
    {
        // Arrange
        var stock = StockQuantity.Create(50);

        // Act & Assert
        stock.IsLowStock().Should().BeFalse();
    }

    [Fact]
    public void IsLowStock_WithQuantityBelowThreshold_ShouldReturnTrue()
    {
        // Arrange
        var stock = StockQuantity.Create(5);

        // Act & Assert
        stock.IsLowStock().Should().BeTrue();
    }

    [Fact]
    public void Increase_WithValidAmount_ShouldIncreaseQuantity()
    {
        // Arrange
        var stock = StockQuantity.Create(10);

        // Act
        var newStock = stock.Increase(5);

        // Assert
        newStock.Value.Should().Be(15);
    }

    [Fact]
    public void Decrease_WithValidAmount_ShouldDecreaseQuantity()
    {
        // Arrange
        var stock = StockQuantity.Create(10);

        // Act
        var newStock = stock.Decrease(3);

        // Assert
        newStock.Value.Should().Be(7);
    }

    [Fact]
    public void Decrease_WithAmountGreaterThanStock_ShouldThrowException()
    {
        // Arrange
        var stock = StockQuantity.Create(5);

        // Act
        var act = () => stock.Decrease(10);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Insufficient stock*");
    }
}
```

## Application Tests

### CreateProductCommandValidatorTests.cs
```csharp
using FluentAssertions;
using FluentValidation.TestHelper;
using Hypesoft.Application.Products.Commands.CreateProduct;
using Xunit;

namespace Hypesoft.Tests.Application;

public class CreateProductCommandValidatorTests
{
    private readonly CreateProductCommandValidator _validator;

    public CreateProductCommandValidatorTests()
    {
        _validator = new CreateProductCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        var command = new CreateProductCommand { Name = "" };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Have_Error_When_Name_Exceeds_MaxLength()
    {
        var command = new CreateProductCommand { Name = new string('a', 201) };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Have_Error_When_Description_Is_Empty()
    {
        var command = new CreateProductCommand { Description = "" };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Should_Have_Error_When_Price_Is_Zero()
    {
        var command = new CreateProductCommand { Price = 0 };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void Should_Have_Error_When_Price_Is_Negative()
    {
        var command = new CreateProductCommand { Price = -10 };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void Should_Have_Error_When_StockQuantity_Is_Negative()
    {
        var command = new CreateProductCommand { StockQuantity = -5 };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.StockQuantity);
    }

    [Fact]
    public void Should_Have_Error_When_CategoryId_Is_Empty()
    {
        var command = new CreateProductCommand { CategoryId = "" };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.CategoryId);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Valid()
    {
        var command = new CreateProductCommand
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            CategoryId = "507f1f77bcf86cd799439011",
            StockQuantity = 10
        };

        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
```

## Run Tests

```bash
# Build
dotnet build

# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test class
dotnet test --filter "FullyQualifiedName~ProductTests"

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"
```

## Expected Output

```
Test Run Successful.
Total tests: 28
     Passed: 28
 Total time: 2.5 Seconds

Code Coverage: 68%
```

---

## Coverage Targets

| Layer | Target | Priority |
|-------|--------|----------|
| Domain | 90%+ | High |
| Application | 70%+ | High |
| Infrastructure | 50%+ | Medium |
| API | 60%+ | High |
| **Overall** | **70%+** | **Required** |

---

**Next**: Add integration tests (see ProductsControllerTests example in STABILIZATION-ROADMAP.md)
