using Microsoft.AspNetCore.Authentication.JwtBearer;
using WebApp.Helpers;
using WebApp.Interfaces;
using WebApp.Models;
using WebApp.Repositories;
using WebApp.Services;
using Coravel;
using WebApp.BackgroundTasks;
using Polly.Extensions.Http;
using Polly;
using Polly.Retry;
using NLog.Web;
using WebApp.Constants;

namespace WebApp
{
    public class Program
    {
        private static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            if (!ConfigurationHelper.IsAllRequiredConfigurationSet(builder.Configuration))
            {
                Console.WriteLine("Cannot start the application, some required configurations are missing.");
                return;
            }

            AuthConfig? authConfig;
            if (!ConfigurationHelper.TryToGetAuthConfiguration(builder.Configuration, out authConfig) || authConfig == null)
            {
                Console.WriteLine("Auth configurations are required to start the application.");
                return;
            }

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    AuthorizationPolicyConstants.AdminPolicyName, 
                    policy => policy.RequireClaim(AuthorizationPolicyConstants.AdminClaimName, "true")
                );
            });
            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtHelper.GetJwtOptionAction(authConfig));

            builder.Services.AddScoped<ICurrenyConversionService, CurrencyConversionService>();

            builder.Services.AddMemoryCache();
            builder.Services.AddSingleton<FileRepository>();
            builder.Services.AddSingleton<CacheRepository>();

            builder.Services.AddScheduler();
            builder.Services.AddSingleton<UpdateCurrenyJob>();

            builder.Services.AddSingleton<IDKBankService, DKBankService>();
            builder.Services.AddHttpClient<IDKBankService, DKBankService>().AddPolicyHandler(HttpRetryPolicy());

            builder.Logging.ClearProviders();
            builder.Host.UseNLog();

            var app = builder.Build();

            app.Services.UseScheduler(scheduler =>
            {
                scheduler.Schedule<UpdateCurrenyJob>().Hourly();
            });

            // Configure the HTTP request pipeline.

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(name: "default", pattern: "{controller=Swagger}/{action=Index}/{id?}");

            app.Run();
        }

        private static AsyncRetryPolicy<HttpResponseMessage> HttpRetryPolicy()
        {
            return HttpPolicyExtensions
               .HandleTransientHttpError()
               .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        }
    }
}
