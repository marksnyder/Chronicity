using Chronicity.Core;
using Chronicity.Provider.EntityFramework;
using Chronicity.Provider.EntityFramework.DataContext;
using ExampleCommon;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;

namespace ExampleTools
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            var services = new ServiceCollection();

            services.AddDbContext<ChronicityContext>(options =>
                options.UseSqlServer(configuration["Connection"]));

            services.AddTransient<TimeLineService>();

            var list = new List<IStateChangeReaction>()
            {
                new BirdDepartureAgent(),
                new BirdArrivalAgent()
            };

            var service = services.BuildServiceProvider().GetService<TimeLineService>();
            service.ReprocessAgents(list, "proxa");

        }

    }
}
