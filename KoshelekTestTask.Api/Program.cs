using System;
using System.IO;
using KoshelekTestTask.Api.Loggers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Serilog;
using Serilog.Events;

namespace KoshelekTestTask.Api
{
    public class Program
    {
        public static readonly string ConnectionString =
            $"Host={Environment.GetEnvironmentVariable("POSTGRES_HOST")};" +
            $"Username={Environment.GetEnvironmentVariable("POSTGRES_USER")};" +
            $"Password={Environment.GetEnvironmentVariable("POSTGRES_PASSWORD")};" +
            $"Database={Environment.GetEnvironmentVariable("POSTGRES_DB")}";

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
                var logsPath = @$"{Environment.CurrentDirectory}/logs";
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
                using var con = new NpgsqlConnection(ConnectionString);
                con.Open();

                using var cmd = new NpgsqlCommand();
                cmd.Connection = con;

                cmd.CommandText = "CREATE TABLE IF NOT EXISTS koshelek (\r\n" +
                                  "id serial NOT NULL,\r\n" +
                                  "serial_number int NOT NULL,\r\n" +
                                  "content varchar(128) NOT NULL,\r\n" +
                                  "moscow_date_time timestamp without time zone NOT NULL,\r\n" +
                                  "CONSTRAINT koshelek_pk PRIMARY KEY (id)\r\n" +
                                  ")";
                cmd.ExecuteNonQuery();

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