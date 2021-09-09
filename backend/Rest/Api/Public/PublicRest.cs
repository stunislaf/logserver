using System.Threading.Tasks;
using LogServer.Store;
using LogServer.Utils;
using Microsoft.AspNetCore.Http;

namespace LogServer.Rest.Api.Public
{
	public class PublicRest
	{
		private AppsService _appsService;
		private IDataStore _store;

		public PublicRest(IDataStore store, AppsService appsService)
		{
			_store = store;
			_appsService = appsService;
		}

		public async Task Events(HttpContext c)
		{
			var appKey = c.Request.GetAppKey();
			if (appKey.IsEmpty())
			{
				await c.Response.UnAuthorize();
				return;
			}

			var appId = await _appsService.GetAppId(appKey);
			if (appId == null)
			{
				await c.Response.UnAuthorize();
				return;
			}

			var (events, error) = await c.Request.BodyTo<EventsList>();
			if (error != null)
			{
				await c.Response.BadRequest(error.Message);
				return;
			}

			await _store.AddEvents(appId, events.Events);
		}
	}
}
