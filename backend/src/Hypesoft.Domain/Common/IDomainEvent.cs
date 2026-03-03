namespace Hypesoft.Domain.Common;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
