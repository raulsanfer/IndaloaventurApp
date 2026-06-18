namespace IndaloAventurApi.Domain.Abstractions;

public interface IDomainEvent
{
    DateTime OccurredOnUtc { get; }
}
