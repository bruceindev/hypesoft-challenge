using Hypesoft.Domain.Common;

namespace Hypesoft.Domain.Events;

public sealed class LowStockDetectedEvent : IDomainEvent
{
    public string ProductId { get; }
    public string ProductName { get; }
    public int CurrentStock { get; }
    public DateTime OccurredOn { get; }

    public LowStockDetectedEvent(string productId, string productName, int currentStock)
    {
        ProductId = productId;
        ProductName = productName;
        CurrentStock = currentStock;
        OccurredOn = DateTime.UtcNow;
    }
}
