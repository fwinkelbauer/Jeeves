# Jeeves

Jeeves is a standalone web services which provides a REST API used to read/write configuration data from your own custom data source.

## Usage

Jeeves offers two routes:

### Retrieve Configuration Data

- **Basic schema:** `/get/{user}/{application}/{key}`
- **Examples:**

```
# No authentication
curl http://localhost:9042/jeeves/get/my_user/my_application/some_key

# With authentication
curl http://localhost:9042/jeeves/get/my_user/my_application/some_key?apikey=some_apikey
```

### Write Configuration Data

- **Basic schema:** `/post/{user}/{application}/{key}`
- **Examples:**

```
# No authentication
curl http://localhost:9042/jeeves/post/my_user/my_application/some_key --data "value=foo"

# With authentication
curl http://localhost:9042/jeeves/post/my_user/my_application/some_key?apikey=some_apikey --data "value=foo"
```

## Projects

- **Jeeves.Core:** The basic template to create a REST API for any data store
- **Jeeves.Host:** An example implementation of **Jeeves.Core** using SQLite
  - This project can be configured using the `Jeeves.Host.exe.config` file
  - The service is installed and starting using the Chocolatey package
  - Use `jeeves.host.exe migrate` to update the database using DbUp. This can also be done while the service is running

## How To Implement A Custom Jeeves Host

**Note:** The NuGet package `Jeeves.Core` has not been published yet, but you can build one on your own by running the build script `Source\build.ps1`.

- Add the NuGet package `Jeeves.Core` to your project
- Create an implementation of `Jeeves.Core.IDataStore`. This interface is used to authenticate an API call, as well as to provide access to the actual stored configuration data
- Create an implementation of `Jeeves.Core.IJeevesLog`. The simplest approach would mean that you create a barebones class which does nothing
- Instantiate and start your own host like this:

```csharp
var baseUrl = "http://localhost:9042/jeeves/"; // Define the base URL
var settings = new JeevesSettings(false, false); // Configure these parameters however you like
IDataStore store = null; // Provide your implementation here
IJeevesLog log = null; // Provde your log implementation here

using (var host = new JeevesHost(new Uri(baseUrl), settings, store, log))
{
    host.Start();
    Console.WriteLine("Press ENTER to exit");
    Console.ReadLine();
}
```

## License

[MIT](http://opensource.org/licenses/MIT)
