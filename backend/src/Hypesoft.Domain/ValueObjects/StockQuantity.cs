using Hypesoft.Domain.Common;

namespace Hypesoft.Domain.ValueObjects;

public sealed class StockQuantity : ValueObject
{
    public const int LowStockThreshold = 10;
    
    public int Value { get; private set; }

    private StockQuantity() { }

    private StockQuantity(int value)
    {
        Value = value;
    }

    public static StockQuantity Create(int value)
    {
        if (value < 0)
            throw new ArgumentException("Stock quantity cannot be negative", nameof(value));

        return new StockQuantity(value);
    }

    public bool IsLowStock() => Value < LowStockThreshold;

    public StockQuantity Increase(int amount)
    {
        if (amount < 0)
            throw new ArgumentException("Amount must be positive", nameof(amount));

        return new StockQuantity(Value + amount);
    }

    public StockQuantity Decrease(int amount)
    {
        if (amount < 0)
            throw new ArgumentException("Amount must be positive", nameof(amount));

        if (Value < amount)
            throw new InvalidOperationException("Insufficient stock");

        return new StockQuantity(Value - amount);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();

    public static implicit operator int(StockQuantity quantity) => quantity.Value;
}
