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

namespace Plovr.Builders
{
	internal class JavaJarParamBuilder
	{
		protected StringBuilder Params { get; set; }

		public JavaJarParamBuilder(string jarPath)
		{
			Params = new StringBuilder();
			Params.Append("-jar \"" + jarPath + "\" ");
		}

		/// <summary>
		/// Add a param to the parm list, this is a helper function to handle the syntax of the closure compiler param list. The
		/// jar takes the format of --option value. This is a shorthand method to help with this.
		/// </summary>
		/// <param name="paramName">the param key</param>
		/// <param name="paramValue">the param value</param>
		public void AddParam(string paramName, string paramValue = null)
		{
			Params.Append("--" + paramName + " ");

			if (!string.IsNullOrEmpty(paramValue))
			{
				Params.Append(paramValue + " ");
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
	}
}
