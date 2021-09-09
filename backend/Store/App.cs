using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace LogServer.Store
{
	public class App
	{
		[BsonElement("app_key"), JsonProperty("app_key")]
		public string AppKey;

		[BsonElement("blocked"), JsonProperty("blocked")]
		public bool Blocked;

		[BsonElement("desc"), JsonProperty("desc")]
		public string Desc;

		[BsonRepresentation(BsonType.ObjectId), JsonProperty("_id")]
		public string Id;
	}
}
