# Jeeves

Jeeves is a standalone web services which provides a REST API used to read/write application configuration data.

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
curl http://localhost:9042/jeeves/post/my_user/my_application/some_key --data "value=42"
curl http://localhost:9042/jeeves/post/my_user/my_application/some_json --data 'value={ "foo" : "bar" }'

# With authentication
curl http://localhost:9042/jeeves/post/my_user/my_application/some_key?apikey=some_apikey --data "value=42"
curl http://localhost:9042/jeeves/post/my_user/my_application/some_json?apikey=some_apikey --data 'value={ "foo" : "bar" }'
```

## Projects

- **Jeeves.Core:** The core project to create a REST API for any data store
- **Jeeves:** A work in progress example implementation of **Jeeves.Core** using SQLite

## How To Use Jeeves.Core

- Add the NuGet package `Jeeves.Core` to your project
- Create an implementation of `Jeeves.Core.IDataStore`. This interface is used to authenticate an user (if you activate authentication via `JeevesSettings`, see below), as well as to retrieve configuration data for an application
- **Optional:** Create an implementation of `Jeeves.Core.IJeevesLog` to receive internal logging information
- Jeeves.Core is a self hosted [NancyFX](http://nancyfx.org/) application. To enable SSL follow [this guide](https://coderead.wordpress.com/2014/08/07/enabling-ssl-for-self-hosted-nancy/)
- Instantiate and start your own host like this:

```csharp
var settings = new JeevesSettings(false, false); // Configure https and authentication details
var baseUrl = "http://localhost:9042/jeeves/"; // Define the base URL
IDataStore store = new MyDataStore(); // Provide your implementation here
IJeevesLog log = new MyLog(); // Provde your log implementation here

using (var host = new JeevesHost(new Uri(baseUrl), settings, store, log))
{
    host.Start();
    Console.WriteLine("Press ENTER to exit");
    Console.ReadLine();
}
```

## License

[MIT](http://opensource.org/licenses/MIT)
