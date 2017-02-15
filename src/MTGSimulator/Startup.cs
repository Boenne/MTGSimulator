using LightInject;
using LightInject.Microsoft.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MTGSimulator.Data;
using MTGSimulator.Data.ContextFactory;
using MTGSimulator.Data.Repositories;
using MTGSimulator.Service;
using ILogger = MTGSimulator.Data.ILogger;

namespace MTGSimulator
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
            services.AddSignalR(options => options.Hubs.EnableDetailedErrors = true);
            var container = new ServiceContainer();
            services.AddScoped<ILogger, Logger>();
            services.AddScoped<IDraftPlayerRepository, DraftPlayerRepository>();
            services.AddScoped<IDraftSessionRepository, DraftSessionRepository>();
            services.AddSingleton<IDatabaseContextFactory>(new DatabaseContextFactory(Configuration.GetConnectionString("DefaultConnectionString")));
            services.AddScoped<ICardParser, CardParser>();
            services.AddScoped<IBoosterCreator, BoosterCreator>();
            container.CreateServiceProvider(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
            app.UseSignalR();
        }
    }
}