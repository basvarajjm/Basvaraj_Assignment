using Microsoft.AspNetCore.Authentication.JwtBearer;
using WebApp.Helpers;
using WebApp.Interfaces;
using WebApp.Models;
using WebApp.Repository;
using WebApp.Services;

namespace WebApp
{
    public class Program
    {

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            AuthConfig authConfig;
            if (!ConfigurationHelper.TryToGetAuthConfiguration(builder.Configuration, out authConfig))
            {
                Console.WriteLine("Auth configurations are required to start the application.");
                return;
            }

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddAuthorization();
            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtHelper.GetJwtOptionAction(authConfig));

            builder.Services.AddScoped<ICurrenyService, CurrencyService>();
            builder.Services.AddScoped<IRepository, FileRepository>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRouting();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
