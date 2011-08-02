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
using Plovr.Model;

namespace Plovr.Configuration
{
	public class PlovrSettingsElement : ConfigurationElement, IPlovrSettings
	{
		[ConfigurationProperty("javaPath")]
		public string JavaPath
		{
			get { return (string)this["javaPath"]; }
			set { throw new System.NotImplementedException(); }
		}

		[ConfigurationProperty("closureCompilerJarPath", IsRequired = true)]
		public string ClosureCompilerJarPath
		{
			get { return (string)this["closureCompilerJarPath"]; }
			set { throw new System.NotImplementedException(); }
		}

		[ConfigurationProperty("includePath", IsRequired = true)]
		public string IncludePath
		{
			get { return (string)this["includePath"]; }
		}
	}
}