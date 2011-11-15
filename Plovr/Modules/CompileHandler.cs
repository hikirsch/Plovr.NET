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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Plovr.Builders;
using Plovr.Helpers;
using Plovr.Model;
using Plovr.Runners;

namespace Plovr.Modules
{
	internal class CompileHandler : Handler
	{
		/// <summary>
		/// the resource id of the JavaScript loader.
		/// </summary>
		protected const string PlovrJavaScriptLoaderResourceId = "Plovr.JavaScript.ScriptLoader.js";

		/// <summary>
		/// the resource id of the Plovr Message System.
		/// </summary>
		protected const string PlovrJavaScriptMessageSystemResourceId = "Plovr.JavaScript.MessageSystem.js";

		/// <summary>
		/// The constructor
		/// </summary>
		/// <param name="context">the current httpd context</param>
		public CompileHandler(HttpContext context) : base(context)
		{
			this.InitCurrentProject();
		}

		/// <summary>
		/// When the compile handler runs, it will either need to process the dependency list and send that back or
		/// run the closure compiler and output the results from that.
		/// </summary>
		public override void Run()
		{
			// create our new builder
			DependencyBuilder builder = new DependencyBuilder(this.CurrentProject, this.CurrentSettings);

			// if the mode isn't raw mode then we pass it off to the compiler, 
			// if it is raw mode then we handle it on our own
			if (this.CurrentProject.Mode == ClosureCompilerMode.Raw)
			{
				this.ShowRawMode(builder);
			}
			else
			{
				this.RunClosureCompiler(builder);
			}
		}

		/// <summary>
		/// Run the closure compiler.
		/// </summary>
		/// <param name="builder"></param>
		private void RunClosureCompiler(DependencyBuilder builder)
		{
			// run the compiler
			ClosureCompilerOutput output;
			var closureCompilerRunner = new ClosureCompilerRunner(this.CurrentSettings, this.CurrentProject);
			closureCompilerRunner.Compile(builder.GetDependencies(), out output);

			// show the response from the output
			this.GenerateResponse(output.StandardOutput, output.Messages, output.StandardError);
		}


		/// <summary>
		/// Show the response from the closure compiler. We are passing in the context in which we need to write to and the
		/// output and error responses.
		/// </summary>
		/// <param name="output">the output received from the compiler.</param>
		/// <param name="errorOutput">the error messages received from the compiler.</param>
		/// <param name="rawError">the raw error string, for testing purposes</param>
		protected void GenerateResponse(string output, List<ClosureCompilerMessage> errorOutput, string rawError)
		{
			var allOutput = new StringBuilder();

			if (errorOutput != null && errorOutput.Count > 0)
			{
				string jsonMessages = this.ToJson(errorOutput);
				allOutput.Append("(function(){\n");
				allOutput.Append(string.Format("window['plovrMessages'] = {0};\n", jsonMessages));

				string messageSystemJavaScript = ResourceHelper.GetTextResourceById(CompileHandler.PlovrJavaScriptMessageSystemResourceId);
				allOutput.Append(messageSystemJavaScript);
				allOutput.Append("\n})();");
				allOutput.Append("\n/*\n");
				allOutput.Append(rawError);
				allOutput.Append("\n*/");
			}

			allOutput.Append(output);

			this.ShowJavaScriptResponse(allOutput.ToString());
		}


		/// <summary>
		/// Shows the Raw mode. This will display the JavaScript resource that is embeded to dynamically load all the dependency files
		/// individually. This is the most verbose mode that Plovr supports.
		/// </summary>
		/// <param name="builder">the builder</param>
		private void ShowRawMode(DependencyBuilder builder)
		{
			// get all the dependencies and reformat the paths for URLs on the site.
			IEnumerable<string> urlDependencies = builder.GetDependencies().Select(ResolveInputPath);

			string dependencyCsv = string.Join("', '", urlDependencies.ToArray());
			string includePath = Context.Request.Url.PathAndQuery;

			// grab the javascript embedded resource.
			string plovrJavaScriptIncluder = ResourceHelper.GetTextResourceById(CompileHandler.PlovrJavaScriptLoaderResourceId);
			plovrJavaScriptIncluder = plovrJavaScriptIncluder.Replace("%FILES%", dependencyCsv);
			plovrJavaScriptIncluder = plovrJavaScriptIncluder.Replace("%INCLUDE_PATH%", includePath);

			this.GenerateResponse(plovrJavaScriptIncluder, null, string.Empty);
		}


		/// <summary>
		/// Pass a full path and convert it to an input handler path
		/// </summary>
		/// <param name="fullPath">the full path to convert</param>
		/// <returns>the url of the input handler</returns>
		private string ResolveInputPath(string fullPath)
		{
			foreach (string basePath in this.CurrentProject.BasePaths)
			{
				if (fullPath.StartsWith(basePath))
				{
					string relativeToBasePath = this.FixSlash(fullPath.Substring(basePath.Length + 1));
					string inputPath = this.GetRootUrl() + "/input/" + CurrentProject.Id + "/" + relativeToBasePath;
					return inputPath;
				}
			}

			throw new Exception("Matching base path not found.  Cannot resolve input path from full path.");
		}
	}
}