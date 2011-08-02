// Copyright 2011 Ogilvy & Mather. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS-IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
using System.Configuration;

namespace Plovr.Configuration
{
	internal class PlovrProjectElement : ConfigurationElement
	{
		/// <summary>
		/// Every project must have an id.
		/// </summary>
		[ConfigurationProperty("id", IsRequired = true)]
		public string Id
		{
			get { return (string)this["id"]; }
		}

		/// <summary>
		/// Gets the basePath
		/// </summary>
		[ConfigurationProperty("basePath", IsRequired = true)]
		public string BasePath
		{
			get { return (string)this["basePath"]; }
		}

		/// <summary>
		/// Gets any extern file for the closure compiler, comma delimited value.
		/// </summary>
		[ConfigurationProperty("closureExternFiles")]
		public string ClosureExternFilesRaw
		{
			get { return (string)this["closureExternFiles"]; }
		}

		/// <summary>
		/// The namespaces to include, comma delimited value.
		/// </summary>
		[ConfigurationProperty("namespaces", IsRequired = true)]
		public string AllNamespaces
		{
			get { return (string) this["namespaces"]; }
		}

		/// <summary>
		/// The mode to execute in. Options are ClosureCompileMode enum.
		/// </summary>
		[ConfigurationProperty("mode")]
		public string ModeRaw
		{
			get { return (string)this["mode"]; }
		}

		/// <summary>
		/// Any custom param that this tool may not support yet but is supported in the compiler.
		/// </summary>
		[ConfigurationProperty("customParams")]
		public string CustomParams
		{
			get { return (string) this["customParams"]; }
		}
	}
}
