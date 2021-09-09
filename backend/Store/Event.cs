using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace LogServer.Store
{
	public class Event
	{
		[BsonElement("app_id"), JsonIgnore]
		public string AppId;

		[BsonElement("exception"), JsonProperty("exception")]
		public string Exception;

		[BsonRepresentation(BsonType.ObjectId), JsonProperty("_id")]
		public string Id;

		[BsonElement("level"), JsonProperty("level")]
		public string Level;

		[BsonElement("properties"), JsonProperty("properties")]
		public Dictionary<string, object> Properties;

		[BsonElement("rendered_message"), JsonProperty("renderedMessage")]
		public string RenderedMessage;

		[BsonElement("server_time"), JsonProperty("server_time")]
		public long ServerTime;

		[BsonElement("timestamp"), JsonProperty("timestamp")]
		public DateTimeOffset Timestamp;

		[BsonElement("timestamp_unix"), JsonProperty("timestamp_unix")]
		public long TimestampUnix;
	}
}
