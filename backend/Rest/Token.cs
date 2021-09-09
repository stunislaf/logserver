using System;
using System.Collections.Generic;
using JWT.Algorithms;
using JWT.Builder;
using Newtonsoft.Json.Linq;

namespace LogServer.Rest
{
	public static class Token
	{
		public static string CreateAdmin()
		{
			var user = new Dictionary<string, object>
			{
				{ "admin", true }
			};

			var token = JwtBuilder.Create()
			.WithAlgorithm(new HMACSHA256Algorithm())
			.WithSecret(AppEnv.Secret)
			.ExpirationTime(DateTimeOffset.UtcNow.AddDays(7).ToUnixTimeSeconds())
			.Issuer("logserver")
			.AddClaim("user", user)
			.Encode();

			return token;
		}

		public static (bool admin, Exception error) AuthAdmin(string token)
		{
			JObject payload;
			try
			{
				payload = new JwtBuilder().WithAlgorithm(new HMACSHA256Algorithm())
				.WithSecret(AppEnv.Secret)
				.MustVerifySignature()
				.Decode<JObject>(token);
			}
			catch (Exception ex)
			{
				return (false, ex);
			}

			var user = payload["user"];

			if (user == null)
				return (false, new Exception("no 'user' segment in token payload"));

			return ((bool)user["admin"], null);
		}
	}
}
