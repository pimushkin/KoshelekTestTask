using System;
using System.IO;
using KoshelekTestTask.Core.Interfaces;
using KoshelekTestTask.Infrastructure;
using KoshelekTestTask.Infrastructure.Data;
using KoshelekTestTask.Infrastructure.Handlers;
using KoshelekTestTask.Infrastructure.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace KoshelekTestTask.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder
                    .WithOrigins("http://localhost")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials());
            });
            services.AddSignalR();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "KoshelekTestTask API",
                        Version = "v1",
                        Description = "API for test task from Koshelek.ru",
                        Contact = new OpenApiContact
                        {
                            Name = "Dmitry Pimushkin",
                            Email = "d.pimushkin@gmail.com"
                        },
                        License = new OpenApiLicense
                        {
                            Name = "Apache 2.0",
                            Url = new Uri("http://www.apache.org/licenses/LICENSE-2.0.html")
                        }
                    }
                );
                var basePath = AppContext.BaseDirectory;
                var xmlPath = Path.Combine(basePath, "KoshelekTestTask.Api.xml");
                options.IncludeXmlComments(xmlPath);
            });
            services.AddTransient<IMessageDispatcher, MessageDispatcher>();
            services.AddTransient<IMessageHandler, MessageHandler>();
            services.AddTransient<IMessageService, MessageService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
            app.UseStaticFiles();
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "KoshelekTestTask API"); });

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<MessageHub>("/chat");
            });
        }
    }
}