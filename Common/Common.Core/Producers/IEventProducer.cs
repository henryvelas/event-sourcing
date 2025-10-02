using Common.Core.Events;

namespace Common.Core.Producer;

public interface IEventProducer
{
    Task ProduceAsync(string topic, BaseEvent @event);
}