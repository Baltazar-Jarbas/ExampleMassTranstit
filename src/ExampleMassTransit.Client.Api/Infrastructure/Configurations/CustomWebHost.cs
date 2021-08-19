using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

namespace ExampleMassTransit.Client.Api.Infrastructure.Configurations
{
    public static class CustomWebHost
    {
        public static IHostBuilder CreateCustomHostBuilder<T>(string[] args) where T : class
        {
            var host = Host.CreateDefaultBuilder(args);
            host.ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseKestrel();
#if DEBUG
                webBuilder.CaptureStartupErrors(true);
#endif

                webBuilder.UseStartup<T>();
            }).ConfigureAppConfiguration((hostContext, configApp) =>
            {
                // Faz com que qualquer variavel de ambiente que comece com CONFIG_ seja adicionada a variavel configuration[IConfiguration]
                configApp.AddEnvironmentVariables(prefix: "CONFIG_");
            }).ConfigureLogging((hostContext, configLog) =>
            {
                configLog.ClearProviders();
                if (hostContext.HostingEnvironment.IsDevelopment())
                {
                    configLog.AddConsole();
                }

                configLog.AddDebug();

            });

            return host;
        }

        public static IHostBuilder BuildAndRun(this IHostBuilder builder)
        {

            builder.Build().Run();

            return builder;
        }
    }
}
