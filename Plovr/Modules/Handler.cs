using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Plovr.Builders;
using Plovr.Configuration;
using Plovr.Helpers;
using Plovr.Model;

namespace Plovr.Modules
{
	abstract class Handler
	{
		protected PlovrProject currentProject;
		protected PlovrSettings currentSettings;

		protected HttpContext context { get; set; }

		/// <summary>
		/// the resource id of the JavaScript loader.
		/// </summary>
		protected const string PlovrJavaScriptLoaderResourceId = "Plovr.JavaScript.ScriptLoader.js";

		protected const string PlovrJavaScriptMessageSystemResourceId = "Plovr.JavaScript.MessageSystem.js";

		/// <summary>
		/// the id key in the query string
		/// </summary>
		private const string IdQueryStringParam = "id";

		/// <summary>
		/// the mode key in the query string
		/// </summary>
		private const string ModeQueryStringParam = "mode";

		public abstract void Run();

		public static Handler CreateInstance(string typeStr, HttpContext context) {
			return (Handler)Activator.CreateInstance(Type.GetType(typeStr), new object[] { context });
		}

		protected Handler(HttpContext context) 
		{
			this.context = context;

			// get the project configuration and application settings
			this.GetActiveProjectAndSettings(out currentProject, out currentSettings);
		}

		protected void ShowResponse(string output) {
			context.Response.ContentType = "application/x-javascript";

			string includePath = context.Request.Url.PathAndQuery;
			output = output.Replace("%INCLUDE_PATH%", includePath);

			context.Response.Write(output);
			context.Response.End();
		}

		/// <summary>
		/// Show the response from the closure compiler. We are passing in the context in which we need to write to and the
		/// output and error responses.
		/// </summary>
		/// <param name="context">the current HttpContext</param>
		/// <param name="output">the output received from the compiler.</param>
		/// <param name="errorOutput">the error messages received from the compiler.</param>
		/// <param name="rawError"></param>
		protected void ShowResponse(string output, List<ClosureCompilerMessage> errorOutput, string rawError) {
			var allOutput = new StringBuilder();

			if (errorOutput != null && errorOutput.Count > 0) {
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

			ShowResponse(allOutput.ToString());
		}

		/// <summary>
		/// The ID can be passed from the QueryString to override the DefaultProject flag.
		/// </summary>
		/// <remarks>Override in subclasses if you need to get it from path instead of QueryString</remarks>
		protected string GetIdFromUri() {
			NameValueCollection queryString = context.Request.QueryString;
			if (queryString.AllKeys.Contains(IdQueryStringParam)) {
				return queryString[IdQueryStringParam];
			}

			return null;
		}


		/// <summary>
		/// Using the HttpContext as the current website that is loaded, load the current project and settings.
		/// </summary>
		/// <param name="context">the current HttpContext</param>
		/// <param name="currentProject">the project param to be set</param>
		/// <param name="currentSettings">the settings param to be set</param>
		private void GetActiveProjectAndSettings(out PlovrProject currentProject, out PlovrSettings currentSettings) {
			// get the id from the QueryString
			var id = GetIdFromUri();

			PlovrConfiguration.GetStrongTypedConfig(out currentSettings, out currentProject, id);

			// support %JAVA_HOME% env variable
			currentSettings.JavaPath = PathHelpers.ResolveJavaPath(currentSettings.JavaPath);

			// override the mode from the querystring if its passed
			currentProject.Mode = this.GetModeFromQueryString() ?? currentProject.Mode;

			// reformat the base paths so we have full paths
			currentProject.BasePaths = currentProject.BasePaths.Select(context.Server.MapPath);

			if (currentProject.Externs != null) {
				currentProject.Externs = currentProject.Externs.Select(context.Server.MapPath);
			}
		}

		private ClosureCompilerMode? GetModeFromQueryString() {
			NameValueCollection queryString = context.Request.QueryString;
			if (queryString.AllKeys.Contains(IdQueryStringParam)) {
				return Mappers.MapToEnum<ClosureCompilerMode>(queryString[ModeQueryStringParam]);
			}

			return null;
		}

		private static string ToJson(List<ClosureCompilerMessage> errorOutput) {
			return JsonConvert.SerializeObject(errorOutput);
		}
	}
}
