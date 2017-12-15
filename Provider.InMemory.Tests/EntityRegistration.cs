using Chronicity.Provider.InMemory;
using System;
using Xunit;

namespace Provider.InMemory.Tests
{
    public class EntityRegistration
    {
        [Fact]
        public void RegisteringEntity_Stores_Type()
        {
            var service = new TimeLineService();
            service.RegisterEntity("1", "TestType");
            Assert.Equal("TestType", service.GetEntityType("1"));
        }

        [Fact]
        public void RegisteringMultipleEntities_Stores_Types()
        {
            var service = new TimeLineService();
            service.RegisterEntity("1", "TestType");
            service.RegisterEntity("2", "TestType2");
            Assert.Equal("TestType", service.GetEntityType("1"));
            Assert.Equal("TestType2", service.GetEntityType("2"));
        }
    }
}
