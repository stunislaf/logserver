# Logserver

Service for store and manage [serilog](https://serilog.net) logs in mongo db.

#### Parameters

| Environment    | Description                                     |
| -------------- | ----------------------------------------------- |
| MONGO_HOST     | URL to mongodb server, _required_               |
| MONGO_DB       | mongodb database name, _required_               |
| MONGO_LOGIN    | mongo user name, _required_                     |
| MONGO_PASS     | mongo user password, _required_                 |
| ADMIN_PASSWORD | password for admin auth, _required_             |
| JWT_SECRET     | secret for jwt generation and check, _required_ |

## API

### Push events

* `POST /events` - push events, _required `X-APP-KEY` header_

```
{
    "events": [
        {
            "timestamp": "2021-08-03T14:06:49.451605+03:00", # C# DateTimeOffset type
            "level": "",                                     # string
            "exception": "",                                 # string
            "renderedMessage": "",                           # string
            "properties": { "some_key": ... },               # C# Dictionary<string, object>
        },
        ...
    ]
}
```

### Admin

#### Authorization

* `POST /admin/auth` - login endpoint. Authorization token stored in `X-JWT` header

```
{
    "password": "some-password"
}
```

#### Events

* `GET /admin/events?appId={app-id}` - get list of events, _required admin auth_
  of all apps sorted byte time

#### Apps

* `POST /admin/apps` - add new app, _required admin auth_

```
{
    "desc":"some app"
}
```

* `GET /admin/apps` - get list of apps, _required admin auth_
* `POST /admin/apps/update-key?appId={app-id}` - generate new app key, _required admin auth_
* `POST /admin/apps/blocked/switch?appId={app-id}` - switch blocked status, _required admin auth_

## Client example

On client can be used [Serilog.Sinks.Http](https://www.nuget.org/packages/Serilog.Sinks.Http/7.2.0) lib.

```
new LoggerConfiguration().WriteTo.Http("http://host", httpClient: new LogServerClient("app-key"))

public class LogServerClient : IHttpClient
{
	HttpClient _httpClient;
	string _appKey;

	public LogServerClient(string appKey)
	{
		_appKey = appKey;
		_httpClient = new HttpClient();
	}

	public void Configure(IConfiguration configuration)
	{
		_httpClient.DefaultRequestHeaders.Add("X-APP-KEY", _appKey);
	}

	public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
	{
		return _httpClient.PostAsync(requestUri, content);
	}

	public void Dispose() => _httpClient?.Dispose();
}

```



































