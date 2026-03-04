using Microsoft.Extensions.DependencyInjection;

namespace EchoProject.Application.Common
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AppServiceAttribute : Attribute
    {
        public ServiceLifetime Lifetime { get; }

        public AppServiceAttribute(ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            Lifetime = lifetime;
        }
    }
}