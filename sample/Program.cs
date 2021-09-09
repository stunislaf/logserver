using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Sinks.Http;
using SimpleEnv;

namespace Sample
{
	class Program
	{
		static void Main()
		{
			Env.FillStaticClass(typeof(AppEnv));

			var loggerBuilder = new LoggerConfiguration().MinimumLevel.Debug()
			.WriteTo.Console()
			.WriteTo.Http(AppEnv.AppUrl, httpClient: new CustomHttpClient(AppEnv.AppKey));

			Log.Logger = loggerBuilder.CreateLogger();

			for (var i = 0; i < 100; i++)
			{
				try
				{
					Log.Information("Hello {Index}", i);

					if (i % 2 == 0)
						throw new Exception("Some another stupid exception");
				}
				catch (Exception ex)
				{
					Log.Error(ex, "Just {Index} ecxeption", i);
				}
			}

			Console.ReadKey();
		}
	}

	public static class AppEnv
	{
		[EnvVar("APP_KEY")]
		public static string AppKey { get; set; }

		[EnvVar("APP_URL")]
		public static string AppUrl { get; set; }
	}

	public class CustomHttpClient : IHttpClient
	{
		HttpClient _httpClient;
		string _appKey;

		public CustomHttpClient(string appKey)
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
}
