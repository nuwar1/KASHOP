using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using KASHOP.BLL.Common;
using KASHOP.BLL.Mapping;
using KASHOP.BLL.Services;
using KASHOP.DAL.Data;
using KASHOP.DAL.Models;
using KASHOP.DAL.Repository;
using KASHOP.PL.Extentions;
using KASHOP.PL.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace KASHOP.PL
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddServices(builder.Configuration);
            var app = builder.Build();
            app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);
            MapsterConfig.MapsterConfigRegister();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            await app.SeedDatabaseAsync();

            app.MapControllers();

            app.Run();
        }
    }
}
