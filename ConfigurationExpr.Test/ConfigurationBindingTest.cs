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
        private Dictionary<string, string> inMemoryConfig = new Dictionary<string, string> {
            ["DbSettings:server"] = "home.zhusmelb.com",
            ["DbSettings:ports:0"] = "3303",
            ["DbSettings:ports:1"] = "3305",
            ["DbSettings:ports:2"] = "3306",
            ["DbSettings:ports:3"] = "3307",
            ["DbSettings:user"] = "yzhu",
            ["DbSettings:password"] = "123456789",
            ["key0"] = "abcdef"
        };

        [Test]
        public void BindingTest() {
            var builder = new ConfigurationBuilder();
            builder.AddInMemoryCollection(inMemoryConfig);

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