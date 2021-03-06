using System;
using System.IO;
using KoshelekTestTask.Infrastructure.Loggers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace KoshelekTestTask.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var seqServerUrl = Environment.GetEnvironmentVariable("SEQ_SERVER_URL");
            var seqApiKey = Environment.GetEnvironmentVariable("SEQ_API_KEY");

            if (!string.IsNullOrWhiteSpace(seqServerUrl))
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("System", LogEventLevel.Error)
                    .Enrich.With(new LogEnricher())
                    .WriteTo.Seq(seqServerUrl, apiKey: seqApiKey)
                    .CreateLogger();
            }
            else
            {
                var basePath = AppContext.BaseDirectory;
                var logsPath = Path.Combine(basePath, "logs");
                if (!Directory.Exists(logsPath)) Directory.CreateDirectory(logsPath);
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("System", LogEventLevel.Error)
                    .Enrich.With(new LogEnricher())
                    .WriteTo.Console()
                    .WriteTo.File(@$"{logsPath}/log-{DateTime.Now:yyyy-MM-dd}.txt")
                    .CreateLogger();
            }

            try
            {
                Log.Information("Starting up Koshelek Api");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Koshelek Api terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                        .UseSerilog();
                });
        }
    }
}