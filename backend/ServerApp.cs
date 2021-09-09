using System;
using System.Threading.Tasks;
using LogServer.Rest.Api;
using LogServer.Store;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace LogServer
{
	public class ServerApp
	{
		private IHost _host;

		public static (ServerApp App, Exception error) Create()
		{
			var app = new ServerApp();

			var (store, error) = CreateStore();
			if (error != null)
			{
				Log.Error(error, "Issue with mongo init");
				return (null, error);
			}

			var rest = Router.Create(store);

			app._host = BuildHost(rest);

			return (app, null);
		}

		public Task Run()
		{
			return _host.RunAsync();
		}

		private static IHost BuildHost(Router router)
		{
			var appHost = Host.CreateDefaultBuilder()
			.UseSerilog()
			.ConfigureWebHostDefaults(b =>
				{
					b.UseKestrel(o => o.AddServerHeader = false);
					b.UseUrls("http://0.0.0.0:8080");
					b.ConfigureServices(s => s.AddCors());
					b.Configure(ab => Configure(ab, router));
				});
			return appHost.Build();
		}

		private static void Configure(IApplicationBuilder app, Router router)
		{
			app.UseRouting();
			
			app.UseCors(b =>
			{
				b.WithOrigins("*");
				b.WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS");
				b.WithHeaders("Accept", "Content-Type", "X-JWT");
				b.WithExposedHeaders("Accept", "Content-Type", "X-JWT");
			});

			app.UseEndpoints(router.Routes);
		}

		private static (IDataStore store, Exception error) CreateStore()
		{
			return Mongo.CreateAndConnect(AppEnv.MongoHost, AppEnv.MongoDb, AppEnv.MongoLogin, AppEnv.MongoPassword);
		}
	}
}
