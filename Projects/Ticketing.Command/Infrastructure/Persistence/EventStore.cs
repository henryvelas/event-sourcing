using Common.Core.Events;
using Common.Core.Producer;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Ticketing.Command.Aplication.Models;
using Ticketing.Command.Domain.Abstracts;
using Ticketing.Command.Domain.EventModels;

namespace Ticketing.Command.Infrastructure.Persistence;

public class EventStore : IEventStore
{
    private readonly IEventModelRepository _eventModelRepository;
    private readonly KafkaSettings _kafkaSettings;

    private readonly IEventProducer _eventProducer;

    public EventStore(IEventModelRepository eventModelRepository,
                        IOptions<KafkaSettings> kafkaSettings,
                        IEventProducer eventProducer)
    {
        _eventModelRepository = eventModelRepository;
        _kafkaSettings = kafkaSettings.Value;
        _eventProducer = eventProducer;
    }

    public async Task<List<BaseEvent>> GetEventsAsync(string aggregateId, CancellationToken cancellationToken)
    {
        var eventStream = await _eventModelRepository
        .FilterByAsync(doc => doc.AggregateIdentifier == aggregateId, cancellationToken);

        if (eventStream is null || !eventStream.Any())
        {
            throw new Exception("El Aggregate no tiene eventos");
        }

        return eventStream.OrderBy(x => x.Version).Select(x => x.EventData).ToList()!;
    }

    public async Task SaveEventsAsync(string aggregateId, IEnumerable<BaseEvent> events, int expetedVersion, CancellationToken cancellationToken)
    {
        var eventStream = await _eventModelRepository
        .FilterByAsync(doc => doc.AggregateIdentifier == aggregateId, cancellationToken);

        if (eventStream.Any() && expetedVersion != 1 && eventStream.Last().Version != expetedVersion)
        {
            throw new Exception("error de concurrencia");
        }

        var version = expetedVersion;

        foreach (var @event in events)
        {
            version++;
            @event.Version = version;
            var eventType = @event.GetType().Name;
            var eventModel = new EventModel
            {
                Timestamp = DateTime.UtcNow,
                AggregateIdentifier = aggregateId,
                Version = version,
                EventType = eventType,
                EventData = @event
            };

            await AddEventStore(eventModel, cancellationToken);

            var topic = _kafkaSettings.Topic ?? throw new Exception("no encontro el topic");

            await _eventProducer.ProduceAsync(topic,@event);

        }


    }

    private async Task AddEventStore(EventModel eventModel, CancellationToken cancellationToken)
    {
        IClientSessionHandle session = await _eventModelRepository.BeginSessionAsync(cancellationToken);

        try
        {
            _eventModelRepository.BeginTransaction(session);
            await _eventModelRepository.InsertOneAsync(eventModel, session, cancellationToken);

            await _eventModelRepository.CommitTransactionAsync(session, cancellationToken);
            
            _eventModelRepository.DisposeSession(session);

        }
        catch (Exception)
        {
            await _eventModelRepository.RollbackTransactionAsync(session, cancellationToken);
            _eventModelRepository.DisposeSession(session);
        }
    }
}