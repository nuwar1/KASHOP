using System.Globalization;
using Microsoft.AspNetCore.Localization;

namespace KASHOP.PL.Extentions
{
    public static class LocalizationExtensions
    {
        public static IServiceCollection AddLocalizationServices(this IServiceCollection services)
        {
            services.AddLocalization(options => options.ResourcesPath = "");
            const string defaultCulture = "en";
            var supportedCultures = new[]
            {
                new CultureInfo(defaultCulture),
                new CultureInfo("ar")
            };
            services.Configure<RequestLocalizationOptions>(options => {
                options.DefaultRequestCulture = new RequestCulture(defaultCulture);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.RequestCultureProviders.Clear();
                options.RequestCultureProviders.Add(new AcceptLanguageHeaderRequestCultureProvider());
            });

            return services;
        }
    }
}
