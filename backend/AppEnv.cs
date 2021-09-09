using SimpleEnv;

namespace LogServer
{
	public static class AppEnv
	{
		[EnvVar("MONGO_HOST")]
		public static string MongoHost { get; set; }

		[EnvVar("MONGO_LOGIN")]
		public static string MongoLogin { get; set; }

		[EnvVar("MONGO_PASS")]
		public static string MongoPassword { get; set; }

		[EnvVar("MONGO_DB")]
		public static string MongoDb { get; set; }

		[EnvVar("ADMIN_PASSWORD")]
		public static string AdminPassword { get; set; }
		
		[EnvVar("SECRET")]
		public static string Secret { get; set; }
	}
}
