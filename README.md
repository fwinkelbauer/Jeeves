# Jeeves

A REST API for a key/value store interface.

## How To Implement A Custom Jeeves Host

- Add the NuGet package `Jeeves.Core` to your project
- Create an implementation of `Jeeves.Core.IDataStore`
- Instantiate and start your own host like this:

```csharp
IDataStore store = null; // Provide your implementation here
var settings = new JeevesSettings(false, false); // Configure these parameters however you like
var baseUrl = "http://localhost:9042/jeeves/"; // Define the base URL

// Start
using (var host = new JeevesHost(settings, store, new Uri(baseUrl)))
{
    host.Start();
    Console.WriteLine("Press ENTER to exit");
    Console.ReadLine();
}
```

## License

[MIT](http://opensource.org/licenses/MIT)
