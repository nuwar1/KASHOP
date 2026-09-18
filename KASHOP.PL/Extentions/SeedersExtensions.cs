using System.Runtime.CompilerServices;
using KASHOP.PL.Utils;

namespace KASHOP.PL.Extentions
{
    public static class SeedersExtensions
    {
        public static async Task SeedDatabaseAsync(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var seeders = services.GetServices<ISeedData>();
                foreach (var seeder in seeders)
                {
                    await seeder.SeedData();
                }
            }
        }
    }
}
