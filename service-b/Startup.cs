using System;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace ServiceB
{
    public class Startup
    {
        public IConfigurationRoot Configuration;

        public Startup(IHostingEnvironment env)
        {
            var aiKey = Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY");
            var devMode = Environment.GetEnvironmentVariable("APPINSIGHTS_DEVELOPER_MODE");
            var useDevMode = env.IsDevelopment() || !String.IsNullOrEmpty(devMode);
            this.Configuration = new ConfigurationBuilder()
                .AddApplicationInsightsSettings(
                    instrumentationKey: aiKey,
                    developerMode:  useDevMode ? (bool?)true : null)
                .Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry(this.Configuration);
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
            app.UseApplicationInsightsRequestTelemetry();
            app.UseApplicationInsightsExceptionTelemetry();
            app.ApplicationServices.GetService<TelemetryClient>().Context.Properties["Service name"] = "service-b";
            app.Run(async context =>
            {
                await context.Response.WriteAsync("Hello from service B running on " + Environment.MachineName);
            });
        }
    }
}
