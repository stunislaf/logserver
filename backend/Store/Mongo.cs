using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace LogServer.Store
{
	public class Mongo : IDataStore
	{
		private IMongoCollection<App> _apps;
		private IMongoDatabase _database;
		private IMongoCollection<Event> _events;

		public async Task AddEvents(string appId, List<Event> events)
		{
			foreach (var ev in events)
			{
				ev.AppId = appId;
				ev.TimestampUnix = ev.Timestamp.ToUnixTimeMilliseconds();
				ev.ServerTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
			}

			await _events.InsertManyAsync(events);
		}

		public async Task<List<Event>> GetEvents(string appId)
		{
			var filter = Builders<Event>.Filter.Eq(u => u.AppId, appId);
			return (await _events.Find(filter).ToListAsync()).OrderBy(e => e.ServerTime).ToList();
		}

		public async Task<App> GetAppByKey(string key)
		{
			var filter = Builders<App>.Filter.Eq(u => u.AppKey, key);
			return await _apps.Find(filter).FirstOrDefaultAsync();
		}

		public async Task<App> AddApp(string desc)
		{
			var app = new App
			{
				Desc = desc,
				Blocked = false,
				AppKey = GenerateAppKey()
			};

			await _apps.InsertOneAsync(app);

			return app;
		}

		public async Task<(string token, Exception error)> GenerateAppKey(string appId)
		{
			var token = GenerateAppKey();

			var filter = Builders<App>.Filter.Eq(u => u.Id, appId);
			var update = Builders<App>.Update.Set(a => a.AppKey, token);

			var res = await _apps.UpdateOneAsync(filter, update);

			if (res.ModifiedCount != 1)
				return (null, new Exception("Issue with find app with id " + appId));

			return (token, null);
		}

		public async Task<(App app, Exception error)> SwitchAppBlock(string appId)
		{
			var filter = Builders<App>.Filter.Eq(u => u.Id, appId);

			var app = await _apps.Find(filter).FirstOrDefaultAsync();

			if (app == null)
				return (null, new Exception("Issue with find app with id " + appId));

			app.Blocked = !app.Blocked;

			await _apps.ReplaceOneAsync(filter, app);

			return (app, null);
		}

		public static (IDataStore store, Exception error) CreateAndConnect(string host, string db, string login, string password)
		{
			try
			{
				var store = new Mongo().Create(host, db, login, password);
				return (store, null);
			}
			catch (Exception e)
			{
				return (null, e);
			}
		}

		public Mongo Create(string host, string db, string login, string password)
		{
			var connectionString = $@"mongodb://{login}:{password}@{host}/{db}";

			var client = new MongoClient(connectionString);

			_database = client.GetDatabase(db);
			_apps = _database.GetCollection<App>("apps");
			_events = _database.GetCollection<Event>("events");

			return this;
		}

		private static string GenerateAppKey()
		{
			return Guid.NewGuid().ToString("N");
		}
	}
}
