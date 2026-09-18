namespace KASHOP.PL.Extentions
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddOpenApi();
            services.AddDatabaseServices(configuration);
            services.AddLocalizationServices();
            services.AddIdentityServices();
            services.AddJwtAuthenticationServices(configuration);
            services.AddApplicationServises();

            return services;

        }
    }
}
