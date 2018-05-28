using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chronicity.Core;
using Chronicity.Provider.EntityFramework;
using Chronicity.Provider.EntityFramework.DataContext;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;

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
            services.AddDbContext<ChronicityContext>(options =>
                options.UseSqlServer(Configuration["Connection"]));

            services.AddTransient<ITimelineService, TimeLineService>();

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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Timeline Service");
            });

            using (var serviceScope =
            app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context =
                serviceScope.ServiceProvider.GetRequiredService<ChronicityContext>();
                context.Database.EnsureCreated();
            }


        }
    }
}
