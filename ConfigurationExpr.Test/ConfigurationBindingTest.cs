using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Microsoft.Extensions.Configuration;

using NUnit.Framework;
using com.zhusmelb.Util.Logging;

namespace ConfigurationExpr.Test
{
    public class DbSettings {
        public string Server {get; set;}
        public int[] Ports {get; set;}
        public string User {get; set;}
        public string Password {get; set;}
    }

    [TestFixture]
    public class ConfigurationBindingTest
    {
        [Test]
        public void BindingTest() {
            var builder = new ConfigurationBuilder();
            builder.AddInMemoryCollection(InMemoryData.TestData);

            var configRoot = builder.Build();
            var dbSettings = configRoot.GetSection("DbSettings").Get<DbSettings>();

            Assert.That(dbSettings, Is.TypeOf<DbSettings>());
            Assert.That(dbSettings.Server, Is.EqualTo("home.zhusmelb.com"));
            Assert.That(dbSettings.Ports, Is.TypeOf<int[]>());
            Assert.That(dbSettings.User, Is.EqualTo("yzhu"));
            Assert.That(dbSettings.Password, Is.EqualTo("123456789"));
        }

    }
}