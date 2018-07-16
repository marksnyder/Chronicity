using Chronicity.Core;
using Chronicity.Core.Reaction;
using Chronicity.Provider.EntityFramework;
using Chronicity.Provider.EntityFramework.DataContext;
using ExampleCommon;
using KoenZomers.Tools.SunSetRiseLib;
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

            var service = services.BuildServiceProvider().GetService<TimeLineService>();

            if (args[0] == "reprocess")
            {
                service.RegisterReaction(new BirdArrivalAgent());
                service.RegisterReaction(new BirdDepartureAgent());
            }

            if(args[0] == "sunset")
            {
                var latitude = 43.038902;
                var longitude = -87.906471;
                var utcOffset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).Hours;

                var endDate = new DateTime(2025, 1, 1);
                var startDate = new DateTime(2018, 1, 1);
                var currentDate = startDate;

                while(currentDate < endDate)
                {
                    Console.WriteLine("Sunrise: " + SunSetRiseLib.SunriseAt(latitude, longitude, currentDate, utcOffset));
                    Console.WriteLine("SunSet: " + SunSetRiseLib.SunsetAt(latitude, longitude, currentDate, utcOffset));

                    service.RegisterEvent(new Chronicity.Core.Timeline.NewEvent()
                    {
                        Entities = new string[] { "Milwaukee" },
                        On = SunSetRiseLib.SunriseAt(latitude, longitude, currentDate, utcOffset).ToString(),
                        Type = "Sunrise"
                    });

                    service.RegisterEvent(new Chronicity.Core.Timeline.NewEvent()
                    {
                        Entities = new string[] { "Milwaukee" },
                        On = SunSetRiseLib.SunsetAt(latitude, longitude, currentDate, utcOffset).ToString(),
                        Type = "Sunset"
                    });

                    currentDate = currentDate.AddDays(1);
                }

            }

        }

    }
}
