using System.Collections.Generic;

internal static class InMemoryData
{
    public static IDictionary<string, string> TestData
        = new Dictionary<string, string> {
            ["DbSettings:server"] = "home.zhusmelb.com",
            ["DbSettings:ports:0"] = "3303",
            ["DbSettings:ports:1"] = "3305",
            ["DbSettings:ports:2"] = "3306",
            ["DbSettings:ports:3"] = "3307",
            ["DbSettings:user"] = "yzhu",
            ["DbSettings:password"] = "123456789",
            ["key0"] = "abcdef"
        };
}