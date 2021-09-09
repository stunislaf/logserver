using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogServer.Store
{
	public interface IDataStore
	{
		Task AddEvents(string appId, List<Event> events);

		Task<List<Event>> GetEvents(string appId);

		Task<App> GetAppByKey(string key);

		Task<App> AddApp(string desc);

		Task<(string token, Exception error)> GenerateAppKey(string appId);

		Task<(App app, Exception error)> SwitchAppBlock(string appId);
	}
}
