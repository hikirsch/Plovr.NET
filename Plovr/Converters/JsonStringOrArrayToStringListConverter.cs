using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Plovr.Converters
{
	public class StringOrArrayConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) { }

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var list = new List<string>();
			if (reader.TokenType.Equals(JsonToken.String))
			{
				list.Add((string)reader.Value);
			}
			else if (reader.TokenType.Equals(JsonToken.StartArray))
			{
				while (reader.Read() && reader.TokenType.Equals(JsonToken.String))
				{
					list.Add((string) reader.Value);
				}
			}
			return list;
		}

		public override bool CanConvert(Type objectType)
		{
			return true;
		}
	}
}
