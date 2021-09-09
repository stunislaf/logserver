using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogServer.Store;
using Microsoft.AspNetCore.Http;

namespace LogServer.Rest.Api
{
	public class AdminRest
	{
		private AppsService _appsService;
		private IDataStore _store;

		public AdminRest(IDataStore store, AppsService appsService)
		{
			_store = store;
			_appsService = appsService;
		}

		public async Task Auth(HttpContext c)
		{
			var (body, error) = await c.Request.BodyTo<Dictionary<string, string>>();
			if (error != null)
			{
				await c.Response.BadRequest(error.Message);
				return;
			}

			body.TryGetValue("password", out var password);

			if (!AppEnv.AdminPassword.SequenceEqual(password ?? string.Empty))
			{
				await c.Response.UnAuthorize();
				return;
			}

			var token = Token.CreateAdmin();
			c.Response.SetJwt(token);
		}

		public async Task Events(HttpContext c)
		{
			if (!c.Request.Query.TryGetValue("appId", out var appId))
			{
				await c.Response.BadRequest("appId parameter is required");
				return;
			}

			var events = await _store.GetEvents(appId);
			await c.Response.WriteJson(events);
		}

		public async Task Apps(HttpContext c)
		{
			var apps = await _store.Apps();
			await c.Response.WriteJson(apps);
		}

		public async Task AddApp(HttpContext c)
		{
			var (app, error) = await c.Request.BodyTo<App>();
			if (error != null)
			{
				await c.Response.BadRequest(error.Message);
				return;
			}

			var created = await _store.AddApp(app.Desc);

			c.Response.StatusCode = 201;
			await c.Response.WriteJson(created);
		}

		public async Task GenerateAppKey(HttpContext c)
		{
			if (!c.Request.Query.TryGetValue("appId", out var appId))
			{
				await c.Response.BadRequest("appId parameter is required");
				return;
			}

			var (token, error) = await _store.GenerateAppKey(appId);
			if (error != null)
			{
				await c.Response.BadRequest(error.Message);
				return;
			}

			c.Response.StatusCode = 200;
			await c.Response.WriteJson(token);
		}

		public async Task SwitchAppBlock(HttpContext c)
		{
			if (!c.Request.Query.TryGetValue("appId", out var appId))
			{
				await c.Response.BadRequest("appId parameter is required");
				return;
			}

			var (app, error) = await _store.SwitchAppBlock(appId);
			if (error != null)
			{
				await c.Response.BadRequest(error.Message);
				return;
			}

			_appsService.Drop(app.AppKey);

			c.Response.StatusCode = 200;
			await c.Response.WriteJson(app);
		}
	}
}
