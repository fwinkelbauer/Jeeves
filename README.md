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
- Create an implementation of `Jeeves.Core.IDataStore`. This interface is used to store/retrieve configuration data for a particular user and an application
- **Optional:** Create an implementation of `Jeeves.Core.IJeevesLog` to receive internal logging information. This object can be passed into the `JeevesHostBuilder`
- **Optional:** Create an implementation of `Jeeves.Core.IUserAuthenticator` to perform stateless authentication on each REST call. This object can be passed into the `JeevesHostBuilder`
  - **Note:** Because of security reasons this feature is only available if you are using an HTTPS URL
- **Optional:** Jeeves.Core is a self hosted [NancyFX](http://nancyfx.org/) application. To enable SSL follow [this guide](https://coderead.wordpress.com/2014/08/07/enabling-ssl-for-self-hosted-nancy/)
- Instantiate and start your own host like this:

```csharp
public class MyDataStore : IDataStore
{
    public void StoreValue(string userName, string application, string key, string value)
    {
        // (userName, application, key) -> store value or throw Exception
    }

    public string RetrieveValue(string userName, string application, string key)
    {
        // (userName, application, key) -> return value/null or throw Exception
    }
}

public class MyUserAuthenticator : IUserAuthenticator
{
    public JeevesUser RetrieveUser(string apikey)
    {
        // This method is only used if authentication
        // is enabled via the JeevesSettings class

        // return JeevesUser/null or throw Exception
    }
}

public class MyLog : IJeevesLog
{
    public void Information(string messageTemplate, params object[] values)
    {
        // Perfom logging
    }

    public void Error(Exception ex, string messageTemplate, params object[] values)
    {
        // Perform logging
    }
}
```

```csharp
Uri baseUrl = new Uri("http://localhost:9042/jeeves/");
IDataStore store = new MyDataStore();
IUserAuthenticator authenticator = new MyUserAuthenticator();
IJeevesLog log = new MyLog();

JeevesHostBuilder hostBuilder = new JeevesHostBuilder(baseUrl, store)
    .WithUserAuthentication(authenticator)
    .LogTo(log);

using (JeevesHost host = hostBuilder.Build())
{
    host.Start();
    Console.WriteLine("Press ENTER to exit");
    Console.ReadLine();
}
```

## License

[MIT](http://opensource.org/licenses/MIT)
