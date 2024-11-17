using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Extensions.EasyNetQ
{
    public static class EasyNetQBuilderExtension
    {
        public static IServiceCollection UseEasyNetQ(this IServiceCollection services, string connectionString)
        {
            services.TryAddSingleton(s =>
            {
                var advancedBus = RabbitHutch.CreateBus(connectionString, register => register.EnableConsoleLogger());
                return advancedBus;
            });
            return services;
        }
    }
}
