using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chronicity.Core;
using Chronicity.Provider.EntityFramework;
using Chronicity.Provider.EntityFramework.DataContext;
using Chronicity.Service.EventAgents;
using GlobalExceptionHandler.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;
using Syslog.Framework.Logging;

namespace Chronicity.Service
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
            var bp = services.BuildServiceProvider();

            services.AddDbContext<ChronicityContext>(options =>
                options.UseSqlServer(Configuration["Connection"]), ServiceLifetime.Singleton);

            services.AddSingleton<ITimelineService,TimeLineService>();


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Timeline Service", Version = "v1" });
            });

            services.AddMvc();

            services.AddCors(o => o.AddPolicy("ChronicityPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));

            services.AddLogging();
        }



        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var timeline = app.ApplicationServices.GetService<ITimelineService>();

            var agent = new ExampleEventAgent(timeline);
            timeline.RegisterAgent(agent);

            var slConfig = Configuration.GetSection("SyslogSettings");
            if (slConfig != null)
                loggerFactory.AddSyslog(slConfig, Configuration.GetValue<string>("SystemName", "localhost"));

            app.UseExceptionHandler().WithConventions(x => {
                x.ContentType = "application/json";
                x.MessageFormatter(s => JsonConvert.SerializeObject(new
                {
                    Message = "An error occurred while processing your request"
                }));
                x.OnError((exception, httpContext) =>
                {   
                    app.ApplicationServices.GetService<ILogger<string>>().LogError(
                        string.Concat(httpContext.Request.QueryString.Value,
                        Environment.NewLine, 
                        exception.Message, 
                        Environment.NewLine,
                        exception.StackTrace
                        ));
                    return Task.CompletedTask;
                });
            });

            app.UseMvc();
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Timeline Service");
            });

            //using (var serviceScope =
            //app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            //{
            //    var context =
            //    serviceScope.ServiceProvider.GetRequiredService<ChronicityContext>();
            //    context.Database.EnsureCreated();
            //}

            app.Map("/error", x => x.Run(y => throw new Exception()));
        }
    }
}
