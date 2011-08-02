using System.Collections.Generic;
using Newtonsoft.Json;
using Plovr.Model;

namespace Plovr.App.Model
{

	/// <summary>
	/// This Json file is modeled after plovr's Json file.
	/// </summary>
	public class PlovrJsonConfig : IPlovrProject
	{
		[JsonProperty("basePaths")]
		public IEnumerable<string> BasePaths { get; set; }

		[JsonProperty("namespaces")]
		public IEnumerable<string> Namespaces { get; set; }

		[JsonProperty("externs")]
		public IEnumerable<string> Externs { get; set; }

		[JsonProperty("mode")]
		public ClosureCompilerMode? Mode { get; set; }

		[JsonProperty("outputFile")]
		public string OutputFile { get; set; }

		[JsonProperty("customParams")]
		public string CustomParams { get; set; }
	}
}
