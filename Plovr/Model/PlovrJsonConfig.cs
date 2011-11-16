using System.Collections.Generic;
using Newtonsoft.Json;
using Plovr.Converters;

namespace Plovr.Model
{
	/// <summary>
	/// This Json file is modeled after plovr's Json file.
	/// </summary>
	public class PlovrJsonConfig : IPlovrProject
	{
		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonIgnore]
		public string ConfigPath { get; set; }

		[JsonProperty("paths")]
		[JsonConverter(typeof(StringOrArrayConverter))]
		public IEnumerable<string> Paths { get; set; }

		[JsonProperty("inputs")]
		[JsonConverter(typeof(StringOrArrayConverter))]
		public IEnumerable<string> Inputs { get; set; }

		[JsonProperty("externs")]
		[JsonConverter(typeof(StringOrArrayConverter))]
		public IEnumerable<string> Externs { get; set; }

		[JsonProperty("mode")]
		public ClosureCompilerMode? Mode { get; set; }

		[JsonProperty("compilerCustomParams")]
		public string CompilerCustomParams { get; set; }

		[JsonProperty("soyCustomParams")]
		public string SoyCustomParams { get; set; }

		[JsonProperty("namespaces")]
		[JsonConverter(typeof(StringOrArrayConverter))]
		public IEnumerable<string> Namespaces { get; set; }
	}
}