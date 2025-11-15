
using System.Text.Json;
using Common.Core.Consumers;
using Common.Core.Events;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Ticketing.Query.Domain.Abstractions;
using Ticketing.Query.Infrastructure.Converters;

namespace Ticketing.Query.Infrastructure.Consumers;

public class ConsumerHostedService : IHostedService
{
    private readonly ILogger<ConsumerHostedService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly ConsumerConfig _config;

    public ConsumerHostedService(
        ILogger<ConsumerHostedService> logger, 
        IServiceProvider serviceProvider, 
        IOptions<ConsumerConfig> config
    )
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _config = config.Value;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("El Event Consumer service esta trabajando");
        var topic = "KAFKA_TOPIC";

        using (IServiceScope scope = _serviceProvider.CreateScope())
        {
            var eventConsumer = scope.ServiceProvider
                                .GetRequiredService<IEventConsumer>();
            
            Task.Run( () => eventConsumer.Consume(topic), cancellationToken);

        }
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("El servicio del consumer se detuvo");
        return Task.CompletedTask;
    }
}
