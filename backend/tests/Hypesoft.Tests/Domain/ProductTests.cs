using FluentAssertions;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Events;

namespace Hypesoft.Tests.Domain;

public class ProductTests
{
    [Fact]
    public void Create_WithNegativePrice_ShouldThrowArgumentException()
    {
        var action = () => Product.Create("Produto", "Desc", -1, 10, "cat-1");

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithNegativeStock_ShouldThrowArgumentException()
    {
        var action = () => Product.Create("Produto", "Desc", 10, -1, "cat-1");

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithLowStock_ShouldRaiseLowStockEvent()
    {
        var product = Product.Create("Produto", "Desc", 10, 5, "cat-1");

        product.DomainEvents.Should().ContainSingle(e => e is LowStockDetectedEvent);
    }

    [Fact]
    public void UpdateStock_TransitionToLowStock_ShouldRaiseLowStockEvent()
    {
        var product = Product.Create("Produto", "Desc", 10, 20, "cat-1");
        product.ClearDomainEvents();

        product.UpdateStock(4);

        product.DomainEvents.Should().ContainSingle(e => e is LowStockDetectedEvent);
    }

    [Fact]
    public void DecreaseStock_WithoutBalance_ShouldThrowInvalidOperationException()
    {
        var product = Product.Create("Produto", "Desc", 10, 2, "cat-1");

        var action = () => product.DecreaseStock(3);

        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Update_WithValidData_ShouldChangeMainFields()
    {
        var product = Product.Create("Produto", "Desc", 10, 15, "cat-1");

        product.Update("Produto novo", "Desc nova", 20, "cat-2", "https://img");

        product.Name.Should().Be("Produto novo");
        product.Description.Should().Be("Desc nova");
        product.Price.Amount.Should().Be(20);
        product.CategoryId.Should().Be("cat-2");
        product.ImageUrl.Should().Be("https://img");
    }
}
