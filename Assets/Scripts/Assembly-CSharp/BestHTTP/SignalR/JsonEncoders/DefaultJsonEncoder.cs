using System.Collections.Generic;
using BestHTTP.JSON;

namespace BestHTTP.SignalR.JsonEncoders
{
	public sealed class DefaultJsonEncoder : IJsonEncoder
	{
		public string Encode(object obj)
		{
			return Json.Encode(obj);
		}

		public IDictionary<string, object> DecodeMessage(string json)
		{
			bool success = false;
			IDictionary<string, object> dictionary = Json.Decode(json, ref success) as IDictionary<string, object>;
			return (!success) ? null : dictionary;
		}
	}
}
