using Microsoft.Extensions.DependencyInjection;
using SmartGrid.Application.Interfaces;
using SmartGrid.Infrastructure.Services;

namespace SmartGrid.Infrastructure.Extensions
{
    internal static class ServiceExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

            services.AddScoped<IParallelSettingsProvider, ParallelSettingsProvider>();

            services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
            services.AddScoped<IImageOptimizationService, ImageOptimizationService>();
            services.AddScoped<IPaymentCheckoutService, StripeCheckoutService>();

            return services;
        }
    }
}
