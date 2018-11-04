using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;
using Moq;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

using com.zhusmelb.Util.Logging;

namespace ConfigurationExpr.Test
{

    [TestFixture]
    public class ConfigurationBuilderTest
    {
        private static readonly ILogger _log 
            = LogHelper.GetLogger(typeof(ConfigurationBuilderTest).FullName);
        delegate void TryGetDelegate(string k, out string v);

        private Mock<IConfigurationProvider> _mockConfigProvider;

        [Test]
        public void ChangeAwareProviderTest() {
            var configRoot = ConfigBuilderTestWith(getChangeProvider());
            _mockConfigProvider.Verify(p=>p.Load(), Times.Once);
            _mockConfigProvider.Verify(p=>p.GetReloadToken(), Times.Once);

            var val1 = configRoot["key1"];
            _mockConfigProvider.Verify(p=>p.TryGet("key1", out It.Ref<string>.IsAny), Times.Once);

            configRoot["key1"] = "value0";
            _mockConfigProvider.Verify(p=>p.Set("key1", "value0"), Times.Once);

            var sections = configRoot.GetChildren();
            _mockConfigProvider.Verify(p=>p.GetChildKeys(It.IsAny<IEnumerable<string>>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void NoChangeProviderTest() {
            ConfigBuilderTestWith(getNonChangeProvider());
        }

        private IConfigurationRoot ConfigBuilderTestWith(IConfigurationProvider povider) {
            var configBuilder = new ConfigurationBuilder();

            var mockConfigSource = new Mock<IConfigurationSource>();
            mockConfigSource.Setup(s => s.Build(configBuilder))
                .Returns(povider);

            configBuilder.Add(mockConfigSource.Object);
            Assert.That(configBuilder.Sources.Count, Is.EqualTo(1));

            var config = configBuilder.Build();
            return config;
        }
        private IConfigurationProvider getNonChangeProvider() {
            return getConfigProvider(()=>null);
        }

        private IConfigurationProvider getChangeProvider() {
            var mockChangeToken = new Mock<IChangeToken>();
            return getConfigProvider(() => mockChangeToken.Object);
        }
        private IConfigurationProvider getConfigProvider(Func<IChangeToken> changeToken) {
            var mockConfigProvider = new Mock<IConfigurationProvider>();
            var returnedKeys = new List<string>();

            mockConfigProvider.Setup(p => p.GetChildKeys(It.IsAny<IEnumerable<string>>(), It.IsAny<string>()))
                .Callback(delegate(IEnumerable<string> keys, string parent) {
                    _log.Info($"provider.GetChildKey parent: {parent}");
                    keys.ToList().ForEach( s => {
                        returnedKeys.Add(s);
                        _log.Info($"key: {s}");
                    });
                })
                .Returns(returnedKeys)
                .Verifiable();

            mockConfigProvider.Setup(p => p.GetReloadToken())
                .Returns(changeToken())
                .Verifiable();
                
            mockConfigProvider.Setup(p => p.Load())
                .Callback(() => _log.Info("mock provider.load called"))
                .Verifiable();

            mockConfigProvider.Setup(p => p.Set(It.IsAny<string>(), It.IsAny<string>()))
                .Callback((string k, string v) => _log.Info($"provider.Set({k}, {v}) called"));
            
            var outValue = string.Empty;
            
            mockConfigProvider.Setup(p => p.TryGet(It.IsAny<string>(), out It.Ref<string>.IsAny))
                .Callback(new TryGetDelegate((string k, out string v) => {
                    _log.Info($"provider.TryGet({k}, out v) called");
                    v = "called";
                }))
                .Returns(true)
                .Verifiable();
            
            _mockConfigProvider = mockConfigProvider;

            return mockConfigProvider.Object;
        }
    }
}
