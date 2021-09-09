using System.Threading.Tasks;
using Serilog;
using SimpleEnv;

namespace LogServer
{
	public class Program
	{
		public static async Task Main()
		{
			Env.FillStaticClass(typeof(AppEnv));

			InitLogger();

			var (server, error) = ServerApp.Create();
			if (error != null)
			{
				Log.Error(error, "Issue with create server");
				return;
			}

			Log.Information("Run server");

			await server.Run();
		}

		private static void InitLogger()
		{
			var format = "{Timestamp:yyyy-MM-ddTHH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}";
			var b = new LoggerConfiguration().WriteTo.Console(outputTemplate: format);
			Log.Logger = b.CreateLogger();
		}
	}
}
