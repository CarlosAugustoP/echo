using System.Reflection;
using EchoProject.Application.Common;
using Microsoft.Extensions.DependencyInjection;

namespace EchoProject.Application.DependencyInjection
{
    public  static class ServicesServiceCollectionExtensions
    {
    public static IServiceCollection AddAppServices(this IServiceCollection services, Assembly assembly)
    {
        var types = assembly.GetTypes()
            .Where(t => t.IsClass &&
                        !t.IsAbstract &&
                        t.GetCustomAttribute<AppServiceAttribute>() != null);

        foreach (var type in types)
        {
            var attribute = type.GetCustomAttribute<AppServiceAttribute>()!;

            services.Add(new ServiceDescriptor(
                type,
                type,
                attribute.Lifetime));
        }

        return services;
    }
    }
}