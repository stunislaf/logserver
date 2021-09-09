using System.Threading.Tasks;
using LogServer.Utils;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace LogServer.Rest
{
	public static class Response
	{
		public static async Task WriteJson<T>(this HttpResponse response, T value)
		{
			response.ContentType = "application/json; charset=utf-8";
			await response.WriteAsync(JsonConvert.SerializeObject(value));
		}

		public static void Ok(this HttpResponse response)
		{
			response.StatusCode = 200;
		}

		public static void Created(this HttpResponse response)
		{
			response.StatusCode = 201;
		}

		public static async Task BadRequest(this HttpResponse response, string message = "")
		{
			response.StatusCode = 400;
			if (!message.IsEmpty())
				await response.WriteAsync(message);
		}

		public static async Task UnAuthorize(this HttpResponse response, string message = "")
		{
			response.StatusCode = 401;
			if (!message.IsEmpty())
				await response.WriteAsync(message);
		}

		public static void SetJwt(this HttpResponse response, string token)
		{
			response.Headers["X-JWT"] = token;
		}
	}
}
