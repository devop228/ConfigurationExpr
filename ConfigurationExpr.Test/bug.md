I created an implementation of `IConfigurationProvider`, of which `GetReloadToken` method returns null. After adding its corresponding `IConfiguraionSource` into `ConfigurationBuilder` then called `Build` method, a `NullReferenceException` was thrown. Below are the implementation of `IConfigurationProvider` and `IConfigurationSource`:
```csharp
class MyConfigProvider : IConfigurationProvider 
{
   ...
   public IChangeToken GetReloadToken() => null;
   ...
}

class MyConfigSource : IConfigurationSource
{
   ...
   public IConfigurationProvider Build(IConfigurationBuilder builder)
      => new MyConfigProvider( ... );
   ...
}
```
To use these implementations: 
```csharp 
var builder = new ConfigurationBuilder();
builder.Add(new MyConfigSource())
var config = builder.Build(); // NullReference thrown here
```
The reason of this bug is in ConfigurationRoot's constructor, it does not validate the return of `GetReloadToken` before passing it to `ChangeToken.OnChange` method.
```csharp
_providers = providers;
foreach (var p in providers)
{
   p.Load();
   ChangeToken.OnChange(() => p.GetReloadToken(), () => RaiseChanged());
}
```