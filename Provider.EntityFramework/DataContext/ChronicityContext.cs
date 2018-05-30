using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chronicity.Provider.EntityFramework.DataContext
{
    public class ChronicityContext : DbContext
    {
        public ChronicityContext(DbContextOptions<ChronicityContext> options)
                : base(options)
        { }

        public DbSet<DataModels.Event> Events { get; set; }
        public DbSet<DataModels.EventType> EventTypes { get; set; }
        public DbSet<DataModels.TimeAndState> TimeAndStates { get; set; }
        public DbSet<DataModels.EntityStateKey> EntityStateKeys { get; set; }
    }
}
