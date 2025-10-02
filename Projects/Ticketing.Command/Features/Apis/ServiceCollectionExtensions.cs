using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Ticketing.Command.Feature.Apis;

public static class ServiceColletionExtensions
{
    public static IServiceCollection RegisterMinimalApis(this IServiceCollection services)
    {
        var currentAssambly = Assembly.GetExecutingAssembly();
        var minimalApis = currentAssambly.GetTypes()
        .Where(t => typeof(IMinimalApi).IsAssignableFrom(t) && t != typeof(IMinimalApi)
        && t.IsPublic && !t.IsAbstract);

        foreach (var minimalApi in minimalApis)
        {
            services.AddSingleton(typeof(IMinimalApi), minimalApi);
        }

        return services;
    }


}