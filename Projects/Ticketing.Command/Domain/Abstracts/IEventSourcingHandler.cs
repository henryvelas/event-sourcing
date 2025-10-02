namespace Ticketing.Command.Domain.Abstracts;

public interface IEventSourcingHandler<T>
{
    Task<T> GetByIdAsync(String aggregateId, CancellationToken cancellationToken);
    Task SaveAsync(AggregateRoot aggregate, CancellationToken cancellationToken); 
}