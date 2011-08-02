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
	public class ClosureCompilerParamBuilder
	{
		/// <summary>
		/// All the params
		/// </summary>
		private StringBuilder Params { get; set; }

		/// <summary>
		/// Create a new Closure Compiler Param Builder. You must pass the path where the closure jar is.
		/// </summary>
		/// <param name="pathToJar"></param>
		public ClosureCompilerParamBuilder(string pathToJar)
		{
			Params = new StringBuilder();
			Params.Append("-jar \"" + pathToJar + "\" ");
		}

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

		#region "Other Methods"
		/// <summary>
		/// Add any raw options directly to the param list. This must be properly formatted.
		/// </summary>
		/// <param name="closureCompilerOptions"></param>
		public void AddRawOptions(string closureCompilerOptions)
		{
			Params.Append(closureCompilerOptions);

			// we add a space to ensure any other params added are separated.
			Params.Append(" ");
		}

		/// <summary>
		/// Build the params and return the string.
		/// </summary>
		/// <returns>a string of all the arguments</returns>
		public string GetParams()
		{
			return Params.ToString();
		}

		#endregion

		#region "Utilities"

		/// <summary>
		/// Add a param to the parm list, this is a helper function to handle the syntax of the closure compiler param list. The
		/// jar takes the format of --option value. This is a shorthand method to help with this.
		/// </summary>
		/// <param name="paramName">the param key</param>
		/// <param name="paramValue">the param value</param>
		public void AddParam(string paramName, string paramValue)
		{
			if (paramValue.Length > 0)
			{
				Params.Append("--" + paramName + " " + paramValue + " ");
			}
		}

		/// <summary>
		/// Same as AddParam but we quote the value being passed, e.g. --js "D:\path\to\include.js"
		/// </summary>
		/// <param name="paramName">the param key</param>
		/// <param name="paramValue">the param value</param>
		public void AddParamQuotedValue(string paramName, string paramValue)
		{
			if (paramValue.Length > 0)
			{
				this.AddParam(paramName, "\"" + paramValue + "\"");
			}
		}

		#endregion
	}
}
