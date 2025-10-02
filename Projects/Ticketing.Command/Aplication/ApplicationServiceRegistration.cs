using FluentValidation;
using Ticketing.Command.Aplication.Core;
using Ticketing.Command.Aplication.Models;

namespace Ticketing.Command.Aplication;

public static class AplicationServiceRegostration
{
    public static IServiceCollection AddApliactionServices(this IServiceCollection services
    , IConfiguration configuration)
    {
        services.Configure<MongoSettings>(configuration.GetSection(nameof(MongoSettings)));

        services.Configure<KafkaSettings>(configuration.GetSection(nameof(KafkaSettings)));


        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(AplicationServiceRegostration).Assembly);
        });

        services.AddValidatorsFromAssembly(typeof(AplicationServiceRegostration).Assembly);

        services.AddAutoMapper(typeof(MappingProfile).Assembly);

        return services;
    }
}