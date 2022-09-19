using Localization.Languages;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;
using System.Globalization;

namespace LocalizationJson.ServiceCollectionExtensions
{
    public static class ServiceCollectionExtensionsLocalization
    {
        public static IServiceCollection AddLocalizationConfiguration(this IServiceCollection services)
        {
            services.AddLocalization();
            services.AddSingleton<LocalizationMiddleware>();
            services.AddDistributedMemoryCache();
            services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();

            return services;
        }

        public static WebApplication UseLocalizationConfiguraton(this WebApplication app)
        {
            var supportedCultures = new[] 
            { 
                new CultureInfo("pt-BR"),
                new CultureInfo("en-US")
            };
            var options = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(new CultureInfo("en-US")),
                SupportedCultures = supportedCultures
            };
            app.UseRequestLocalization(options);
            app.UseMiddleware<LocalizationMiddleware>();

            return app;
        }
    }
}
