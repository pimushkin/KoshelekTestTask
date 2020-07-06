using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using KoshelekTestTask.Api.Loggers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                    .Enrich.With(new LogEnricher())
                    .WriteTo.Seq(seqServerUrl, apiKey: seqApiKey)
                    .CreateLogger();
            }
            else
            {
                var logsPath = @$"{Environment.CurrentDirectory}/logs";
                if (!Directory.Exists(logsPath))
                {
                    Directory.CreateDirectory(logsPath);
                }
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
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

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                        .UseSerilog();
                });
    }
}
