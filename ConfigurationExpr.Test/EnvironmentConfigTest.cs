using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;
using Moq;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

using com.zhusmelb.Util.Logging;

[TestFixture]
public class EnvironmentConfigTest 
{
    private static readonly ILogger _log 
        = LogHelper.GetLogger(typeof(EnvironmentConfigTest).FullName);
    private IConfigurationBuilder _builder;

    [SetUp]
    public void Init() {
        _builder = new ConfigurationBuilder();
        _builder.AddEnvironmentVariables("ENVCONFIGTEST_");

        // Add environment variables
        Environment.SetEnvironmentVariable("ENVCONFIGTEST_SECTION1", "value1");
        Environment.SetEnvironmentVariable("ENVCONFIGTEST_SECTION1__KEY0", "value1-0");
        Environment.SetEnvironmentVariable("ENVCONFIGTEST_SECTION1__KEY1", "value1-1");
        Environment.SetEnvironmentVariable("ENVCONFIGTEST_SECTION1__KEY2", "value1-2");
        Environment.SetEnvironmentVariable("ENVCONFIGTEST_SECTION2__key0", "value2-0");
        Environment.SetEnvironmentVariable("ENVCONFIGTEST_SECTION2__key1", "value2-1");
        Environment.SetEnvironmentVariable("ENVCONFIGTEST_SECTION2__key2", "value2-2");

        // envrionment not to be used
        Environment.SetEnvironmentVariable("MYOWN_SECTION3__key0", "value3-0");
    }

    [TearDown]
    public void Cleanup() {
        Environment.SetEnvironmentVariable("ENVCONFIGTEST_SECTION1", null);        
        Environment.SetEnvironmentVariable("ENVCONFIGTEST_SECTION1__KEY0", null);
        Environment.SetEnvironmentVariable("ENVCONFIGTEST_SECTION1__KEY1", null);
        Environment.SetEnvironmentVariable("ENVCONFIGTEST_SECTION1__KEY2", null);
        Environment.SetEnvironmentVariable("ENVCONFIGTEST_SECTION2__KEY0", null);
        Environment.SetEnvironmentVariable("ENVCONFIGTEST_SECTION2__KEY1", null);
        Environment.SetEnvironmentVariable("ENVCONFIGTEST_SECTION2__KEY2", null);

        Environment.SetEnvironmentVariable("MYOWN_SECTION3__key0", null);
    }

    [Test]
    public void EnvConfigGeneralTest() {
        var config = _builder.Build();
        // The Key and Path value depend on the parameter passed to
        // GetSection method.
        var section1 = config.GetSection("section1");
        Assert.That(section1, Is.Not.Null);
        Assert.That(section1.Exists(), Is.True);
        Assert.That(section1.Key, Is.EqualTo("section1"));
        Assert.That(section1.Key, Is.Not.EqualTo("SECTION1"));

        var child = section1.GetSection("key0");
        Assert.That(child.Key, Is.EqualTo("key0"));
        Assert.That(child.Key, Is.Not.EqualTo("KEY0"));
        Assert.That(child.Path, Is.EqualTo("section1:key0"));
        Assert.That(child.Path, Is.Not.EqualTo("SECTION1:key0"));

        var section2 = config.GetSection("SECTION2");
        Assert.That(section2, Is.Not.Null);
        Assert.That(section2.Exists(), Is.True);
        Assert.That(section2.Key, Is.EqualTo("SECTION2"));
        Assert.That(section1.Key, Is.Not.EqualTo("section2"));

        // key is case-insensitive
        Assert.That(section2["key0"], Is.EqualTo("value2-0"));
        Assert.That(section2["KEY0"], Is.EqualTo("value2-0"));
    }
}