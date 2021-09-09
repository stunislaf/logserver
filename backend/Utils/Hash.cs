using System.Security.Cryptography;
using System.Text;

namespace LogServer.Utils
{
	public class Hash
	{
		public static string Sha256Hash(string value)
		{
			var stringBuilder = new StringBuilder();

			using var shA256 = SHA256.Create();

			foreach (var num in shA256.ComputeHash(Encoding.UTF8.GetBytes(value)))
				stringBuilder.Append(num.ToString("x2"));

			return stringBuilder.ToString();
		}
	}
}
