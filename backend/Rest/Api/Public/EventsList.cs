using System.Collections.Generic;
using LogServer.Store;
using Newtonsoft.Json;

namespace LogServer.Rest.Api.Public
{
	public class EventsList
	{
		[JsonProperty("Events")]
		public List<Event> Events { get; set; }
	}
}
