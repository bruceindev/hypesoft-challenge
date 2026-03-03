using Hypesoft.Domain.Common;

namespace Hypesoft.Domain.ValueObjects;

public sealed class Price : ValueObject
{
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = null!;

    private Price() { }

    private Price(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Price Create(decimal amount, string currency = "BRL")
    {
        if (amount < 0)
            throw new ArgumentException("Price cannot be negative", nameof(amount));

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be empty", nameof(currency));

        return new Price(amount, currency.ToUpperInvariant());
    }

    public Price ApplyDiscount(decimal discountPercentage)
    {
        if (discountPercentage < 0 || discountPercentage > 100)
            throw new ArgumentException("Discount must be between 0 and 100", nameof(discountPercentage));

        var discountedAmount = Amount * (1 - discountPercentage / 100);
        return new Price(discountedAmount, Currency);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString() => $"{Currency} {Amount:F2}";
}
