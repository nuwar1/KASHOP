using KASHOP.BLL.Common;
using KASHOP.BLL.Services;
using KASHOP.DAL.Repository;
using KASHOP.PL.Utils;

namespace KASHOP.PL.Extentions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServises(this IServiceCollection services) {
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<ISeedData, RoleSeedData>();
            services.AddTransient<IEmailSender, EmailSender>();

            return services;
        }
    }
}
