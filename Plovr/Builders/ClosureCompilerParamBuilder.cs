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
using System.Text;
using Plovr.Model;

namespace Plovr.Builders
{
	internal class ClosureCompilerParamBuilder : JavaJarParamBuilder
	{
		/// <summary>
		/// All the params
		/// </summary>
		private StringBuilder Params { get; set; }

		/// <summary>
		/// Create a new Closure Compiler Param Builder. You must pass the path where the closure jar is.
		/// </summary>
		/// <param name="pathToJar"></param>
		public ClosureCompilerParamBuilder(string pathToJar) : base(pathToJar) { }

		#region "Closure Params"

		/// <summary>
		/// Add a JavaScript file to the param list.
		/// </summary>
		/// <param name="filePath">the js file to include</param>
		public void AddJavaScriptFile(string filePath)
		{
			this.AddParamQuotedValue("js", filePath);
		}

		/// <summary>
		/// Add an extern file to the param list.
		/// </summary>
		/// <param name="externFile">the extern file</param>
		public void AddExternsFile(string externFile)
		{
			this.AddParamQuotedValue("externs", externFile);
		}

		/// <summary>
		/// Add the compilation level to the params.
		/// </summary>
		/// <param name="closureCompliationMode">the compilation mode</param>
		public void AddCompilationLevel(ClosureCompilerMode? closureCompliationMode)
		{
			string mode = GetCompilationModeFromEnum(ClosureCompilerMode.Whitespace);

			if( closureCompliationMode != null)
			{
				mode = GetCompilationModeFromEnum(closureCompliationMode);
			}

			this.AddParam("compilation_level", mode);
		}

		private static string GetCompilationModeFromEnum(ClosureCompilerMode? closureCompliationMode)
		{
			switch (closureCompliationMode)
			{
				case ClosureCompilerMode.Advanced:
					return "ADVANCED_OPTIMIZATIONS";
				case ClosureCompilerMode.Simple:
					return "SIMPLE_OPTIMIZATIONS";
				case ClosureCompilerMode.Whitespace:
					return "WHITESPACE_ONLY";
				case ClosureCompilerMode.Raw:
					return "RAW";
			}

			return null;
		}

		/// <summary>
		/// Add an output file param to the compiler.
		/// </summary>
		/// <param name="outputFile">the path of the file to output</param>
		public void AddOutputFile(string outputFile)
		{
			this.AddParamQuotedValue("js_output_file", outputFile);
		}

		#endregion
	}
}
