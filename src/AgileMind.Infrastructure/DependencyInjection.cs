using AgileMind.Application.Interfaces;
using AgileMind.Infrastructure.Gpt;
using AgileMind.Infrastructure.Mongo;
using Microsoft.Extensions.DependencyInjection;

namespace AgileMind.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IBacklogRepository, BacklogRepository>();
        services.AddScoped<IAiClient, GptClient>();
    }
}
