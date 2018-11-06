using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

using NUnit.Framework;

public class MyConfigProvider : IConfigurationProvider
{
    public IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string parentPath)
    {
        throw new NotImplementedException();
    }

    public IChangeToken GetReloadToken() => null;

    public void Load()
    {
        // do nothing here
    }

    public void Set(string key, string value)
    {
        throw new NotImplementedException();
    }

    public bool TryGet(string key, out string value)
    {
        throw new NotImplementedException();
    }
}

public class MyConfigSource : IConfigurationSource
{
    public IConfigurationProvider Build(IConfigurationBuilder builder)
        => new MyConfigProvider();
}

[TestFixture]
public class NullReferenceBugRepo 
{
    [Test]
    public void NullChangeTokenTest() 
    {
        var builder = new ConfigurationBuilder();
        builder.Add(new MyConfigSource());
        var config = builder.Build();
    }
}