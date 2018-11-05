using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

using Microsoft.Extensions.Configuration;

using NUnit.Framework;
using com.zhusmelb.Util.Logging;

namespace ConfigurationExpr.Test
{
    [TestFixture]
    public class FileConfigTest
    {
        private static readonly ILogger _log 
            = LogHelper.GetLogger(typeof(FileConfigTest).FullName);

        [Test]
        public void LoadJsonFileTest() {

            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("./Assets/appsettings1.json");

            var configRoot = builder.Build();

            var children = configRoot.GetChildren();
            var childrenList = children.ToList();
            Assert.That(childrenList.Count(), Is.EqualTo(5));
            childrenList.ForEach(s => _log.Info($"key: {s.Key}, path: {s.Path}, value: {s.Value}"));

            var child = configRoot.GetSection("key0");
            _log.Info($"child[\"key0\"] key: {child.Key}, path: {child.Path}, value: {child.Value}");

            var value = configRoot["section1"];
            _log.Info($"root[\"section1\"]: {value}");

            value = configRoot["section1:key0"];
            _log.Info($"root[\"section1:key0\"]: {value}");

            var section0 = configRoot.GetSection("section1");
            section0.GetChildren().ToList().ForEach(s => _log.Info($"secion0 key: {s.Key}, path: {s.Path}, value: {s.Value}"));

            var connStr = configRoot.GetConnectionString("efconnection");
            _log.Info($"connectionString: {connStr}");
        }

        [Test]
        public void LoadMultipleJsonFileTest() {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("./Assets/appsettings1.json")
                .AddJsonFile("./Assets/appsettings2.json");
            var configRoot = builder.Build();

            var children = configRoot.GetChildren();
            var childrenList = children.ToList();
            Assert.That(childrenList.Count(), Is.EqualTo(7));
            childrenList.ForEach(s => _log.Info($"key: {s.Key}, path: {s.Path}, value: {s.Value}"));

            var key3Section = configRoot.GetSection("key3");
            Assert.That(key3Section, Is.Not.Null);
            var key3List = key3Section.GetChildren().ToList();
            key3List.ForEach(s => _log.Info($"key: {s.Key}, path: {s.Path}, value: {s.Value}"));
            
        }
    }
}
