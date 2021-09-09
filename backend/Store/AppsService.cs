using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogServer.Store
{
	public class AppsService
	{
		private Dictionary<string, App> _apps = new();
		private IDataStore _store;

		public AppsService(IDataStore store) => _store = store;

		public async Task<string> GetAppId(string key)
		{
			App app;

			if (_apps.TryGetValue(key, out app))
				return app.Blocked ? null : app.Id;

			app = await _store.GetAppByKey(key);

			if (app == null)
				return null;

			_apps[key] = app;

			return app.Blocked ? null : app.Id;
		}

		public void Drop(string key)
		{
			_apps.Remove(key);
		}
	}
}
