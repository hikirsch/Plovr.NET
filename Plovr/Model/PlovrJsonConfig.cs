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

		[JsonProperty("basePath")]
		[JsonConverter(typeof(StringOrArrayConverter))]
		public IEnumerable<string> BasePaths { get; set; }

		[JsonProperty("namespaces")]
		[JsonConverter(typeof(StringOrArrayConverter))]
		public IEnumerable<string> Namespaces { get; set; }

		[JsonProperty("externs")]
		[JsonConverter(typeof(StringOrArrayConverter))]
		public IEnumerable<string> Externs { get; set; }

		[JsonProperty("mode")]
		public ClosureCompilerMode? Mode { get; set; }

		[JsonProperty("outputFile")]
		public string OutputFile { get; set; }

		[JsonProperty("compilerCustomParams")]
		public string CompilerCustomParams { get; set; }

		[JsonProperty("soyCustomParams")]
		public string SoyCustomParams { get; set; }
	}
}
