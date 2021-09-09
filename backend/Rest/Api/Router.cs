using System.Threading.Tasks;
using LogServer.Rest.Api.Public;
using LogServer.Store;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LogServer.Rest.Api
{
	public class Router
	{
		private AppsService _appsService;
		private AdminRest _adminRest;
		private PublicRest _public;
		private IDataStore _store;

		public static Router Create(IDataStore store)
		{
			var rest = new Router();

			rest._store = store;
			rest._appsService = new AppsService(store);
			rest._public = new PublicRest(rest._store, rest._appsService);
			rest._adminRest = new AdminRest(rest._store, rest._appsService);

			return rest;
		}

		public void Routes(IEndpointRouteBuilder e)
		{
			e.MapPost("/events", _public.Events);

			e.MapPost("/admin/auth", _adminRest.Auth);

			e.MapGet("/admin/events", c => AuthAdmin(c, _adminRest.Events));

			e.MapPost("/admin/apps", c => AuthAdmin(c, _adminRest.AddApp));

			e.MapPost("/admin/apps/update-key", c => AuthAdmin(c, _adminRest.GenerateAppKey));

			e.MapPost("/admin/apps/switch", c => AuthAdmin(c, _adminRest.SwitchAppBlock));
		}

		private Task AuthAdmin(HttpContext c, RequestDelegate rd)
		{
			var token = c.Request.GetJwt();
			var (admin, error) = Token.AuthAdmin(token);

			if (error != null)
				return c.Response.UnAuthorize(error.Message);

			if (!admin)
				return c.Response.UnAuthorize();

			return rd.Invoke(c);
		}
	}
}
