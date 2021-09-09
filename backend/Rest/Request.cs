using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace LogServer.Rest
{
	public static class Request
	{
		public static async Task<string> BodyString(this HttpRequest request)
		{
			request.EnableBuffering();

			using var reader = new StreamReader(request.Body, Encoding.UTF8, false, 1024, true);

			var body = await reader.ReadToEndAsync();

			request.Body.Position = 0;

			return body;
		}

		public static async Task<(T value, Exception error)> BodyTo<T>(this HttpRequest request) where T : class
		{
			if (request.ContentType.Contains("application/json") == false)
				return (null, new Exception("invalid ContentType: " + request.ContentType));

			var stringBody = await request.BodyString();

			if (string.IsNullOrWhiteSpace(stringBody))
				return (null, new Exception("empty body"));

			if (typeof(T) == typeof(string))
				return (stringBody as T, null);

			if (typeof(T) == typeof(BsonDocument))
			{
				try
				{
					var value = BsonDocument.Parse(stringBody) as T;
					return (value, null);
				}
				catch (Exception e)
				{
					return (null, e);
				}
			}

			try
			{
				var value = JsonConvert.DeserializeObject<T>(stringBody);
				return (value, null);
			}
			catch (Exception e)
			{
				return (null, e);
			}
		}

		public static string GetJwt(this HttpRequest request)
		{
			return request.Headers.TryGetValue("X-JWT", out var jwt) ? (string)jwt : null;
		}

		public static string GetAppKey(this HttpRequest request)
		{
			return request.Headers.TryGetValue("X-APP-KEY", out var jwt) ? (string)jwt : null;
		}
	}
}
