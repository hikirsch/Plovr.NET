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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Plovr.Builders;
using Plovr.Configuration;
using Plovr.Model;
using Plovr.Helpers;

namespace Plovr.Handlers
{
	public class RequestHandler : IHttpHandler
	{
		/// <summary>
		/// the id key in the query string
		/// </summary>
		private const string IdQueryStringParam = "id";

		/// <summary>
		/// the mode key in the query string
		/// </summary>
		private const string ModeQueryStringParam = "mode";

		/// <summary>
		/// the resource id of the JavaScript loader.
		/// </summary>
		private const string PlovrJavaScriptLoaderResourceId = "Plovr.JavaScript.ScriptLoader.js";
		
		private const string PlovrJavaScriptMessageSystemResourceId = "Plovr.JavaScript.MessageSystem.js";

		/// <summary>
		/// The start of the Plovr handler request.
		/// </summary>
		/// <param name="context">the current HttpContext</param>
		public void ProcessRequest(HttpContext context)
		{

			// get the project configuration and application settings
			PlovrProject currentProject;
			PlovrSettings currentSettings;
			this.GetActiveProjectAndSettings(context, out currentProject, out currentSettings);

			// create our new builder
			DependencyBuilder builder = new DependencyBuilder(currentProject, currentSettings);

			if (ShouldLoadFireBugLight(context))
			{
				var firebugLightCode = ResourceHelper.GetTextResourceById("Plovr.JavaScript.FirebugLite.js");

				ShowResponse(context, firebugLightCode);
			}
			else
			{
				// if the mode isn't raw mode then we pass it off to the compiler, 
				// if it is raw mode then we handle it on our own
				if (currentProject.Mode == ClosureCompilerMode.Raw)
				{
					ShowRawMode(context, builder);
				}
				else
				{
					RunClosureCompiler(context, builder);
				}
			}
		}

		private bool ShouldLoadFireBugLight(HttpContext context)
		{
			return context.Request.QueryString.AllKeys.Contains("firebugLite");
		}

		/// <summary>
		/// Run the closure compiler.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="builder"></param>
		private void RunClosureCompiler(HttpContext context, DependencyBuilder builder)
		{
			// run the compiler
			ClosureCompilerOutput output;
			builder.Compile(out output);

			// show the response from the output
			ShowResponse(context, output.StandardOutput, output.Messages, output.StandardError);
		}

		/// <summary>
		/// Using the HttpContext as the current website that is loaded, load the current project and settings.
		/// </summary>
		/// <param name="context">the current HttpContext</param>
		/// <param name="currentProject">the project param to be set</param>
		/// <param name="currentSettings">the settings param to be set</param>
		private void GetActiveProjectAndSettings(HttpContext context, out PlovrProject currentProject, out PlovrSettings currentSettings)
		{
			// get the id from the QueryString
			var id = GetIdFromQueryString(context);

			PlovrConfiguration.GetStrongTypedConfig(out currentSettings, out currentProject, id);

			currentSettings.JavaPath = PathHelpers.ResolveJavaPath(currentSettings.JavaPath);

			// override the mode from the querystring if its passed
			currentProject.Mode = GetModeFromQueryString(context) ?? currentProject.Mode;

			// reformat the base paths so we have full paths
			currentProject.BasePaths = currentProject.BasePaths.Select(context.Server.MapPath);

			if (currentProject.Externs != null)
			{
				currentProject.Externs = currentProject.Externs.Select(context.Server.MapPath);
			}
		}

		/// <summary>
		/// Shows the Raw mode. This will display the JavaScript resource that is embeded to dynamically load all the dependency files
		/// individually. This is the most verbose mode that Plovr supports.
		/// </summary>
		/// <param name="context">HttpContext</param>
		/// <param name="builder">the builder</param>
		private void ShowRawMode(HttpContext context, DependencyBuilder builder)
		{
			// get all the dependencies and reformat the paths for URLs on the site.
			IEnumerable<string> urlDependencies = builder.GetDependencies().Select(PathHelpers.MakeRelativeFromPath);
			string dependencyCsv = string.Join("', '", urlDependencies);

			// grab the javascript embedded resource.
			string plovrJavaScriptIncluder = ResourceHelper.GetTextResourceById(PlovrJavaScriptLoaderResourceId);

			plovrJavaScriptIncluder = plovrJavaScriptIncluder.Replace("%FILES%", dependencyCsv);

			ShowResponse(context, plovrJavaScriptIncluder, null, string.Empty);
		}

		private void ShowResponse(HttpContext context, string output)
		{
			context.Response.ContentType = "application/x-javascript";
			
			string includePath = context.Request.Url.PathAndQuery;
			output = output.Replace("%INCLUDE_PATH%", includePath);

			context.Response.Write(output);
		}

		/// <summary>
		/// Show the response from the closure compiler. We are passing in the context in which we need to write to and the
		/// output and error responses.
		/// </summary>
		/// <param name="context">the current HttpContext</param>
		/// <param name="id">the id of the project that was built</param>
		/// <param name="output">the output received from the compiler.</param>
		/// <param name="errorOutput">the error messages received from the compiler.</param>
		/// <param name="rawError"></param>
		private void ShowResponse(HttpContext context, string output, List<ClosureCompilerMessage> errorOutput, string rawError)
		{
			var allOutput = new StringBuilder();
			
			if (errorOutput != null && errorOutput.Count > 0)
			{
				string jsonMessages = ToJson(errorOutput);
				allOutput.Append("(function(){\n");
				allOutput.Append(string.Format("window['plovrMessages'] = {0};\n", jsonMessages));

				string messageSystemJavaScript = ResourceHelper.GetTextResourceById(PlovrJavaScriptMessageSystemResourceId);
				allOutput.Append(messageSystemJavaScript);
				allOutput.Append("\n})();");
				allOutput.Append("\n/*\n");
				allOutput.Append(rawError);
				allOutput.Append("\n*/");
			}

			allOutput.Append(output);

			ShowResponse(context, allOutput.ToString());
		}

		private static string ToJson(List<ClosureCompilerMessage> errorOutput)
		{
			return JsonConvert.SerializeObject(errorOutput);
		}

		/// <summary>
		/// The ID can be passed from the QueryString to override the DefaultProject flag.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		private string GetIdFromQueryString(HttpContext context)
		{
			NameValueCollection queryString = context.Request.QueryString;
			if( queryString.AllKeys.Contains(IdQueryStringParam) )
			{
				return queryString[IdQueryStringParam];
			}

			return null;
		}

		private ClosureCompilerMode? GetModeFromQueryString(HttpContext context)
		{
			NameValueCollection queryString = context.Request.QueryString;
			if (queryString.AllKeys.Contains(IdQueryStringParam))
			{
				return Mappers.MapToEnum<ClosureCompilerMode>(queryString[ModeQueryStringParam]);
			}

			return null;
		}

		/// <summary>
		/// I think we can reuse this? I haven't had a problem yet.
		/// </summary>
		public bool IsReusable
		{
			get { return true; }
		}
	}
}
