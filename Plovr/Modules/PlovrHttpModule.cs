using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
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
	// http://code.google.com/p/plovr/source/browse/src/org/plovr/AbstractGetHandler.java
	class PlovrHttpModule : IHttpModule
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
		/// Register this module so that it listens on every Http Request.
		/// </summary>
		/// <param name="context"></param>
		public void Init(HttpApplication context)
		{
			context.BeginRequest += BeginRequest;
		}

		/// <summary>
		/// An event listener, this will fire whenever a request has started. If we contain /plovr in the URL, 
		/// then we handle the entire request.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		public void BeginRequest(object sender, EventArgs eventArgs)
		{
			HttpContext context = HttpContext.Current;
			string url = context.Request.RawUrl.ToLower();

			// make sure this is a plovr request; otherwise, pass it along
			if (!url.StartsWith("/plovr.net")) return;

			// Pass extra url params in to the appropriate handler
			// TODO: support all command_names from Plovr, @see http://code.google.com/p/plovr/source/browse/src/org/plovr/Handler.java 

			string pattern = @"/[^/]+"; // matches '/path1', '/path2', '/path3' in '/path1/path2/path3/'
			MatchCollection matches = Regex.Matches(url, pattern, RegexOptions.IgnoreCase);

			// if we have specified additional params in the url
			if (matches.Count > 1) {
				switch (matches[1].ToString()) {
					case "/config":
						ConfigHandler(context);
						break;
					case "/compile":
						CompileHandler(context);
						break;
					case "/externs":
						ExternsHandler(context);
						break;
					case "/input":
						InputHandler(context);
						break;
					case "/list":
						ListHandler(context);
						break;
					case "/module":
						ModuleHandler(context);
						break;
					case "/modules":
						ModulesHandler(context);
						break;
					case "/size":
						SizeHandler(context);
						break;
					case "/sourcemap":
						SourcemapHandler(context);
						break;
					case "/view":
						ViewHandler(context);
						break;
					default:
						// no matching handler, render index
						IndexHandler(context);
						break;
				}
			} else { // else we have only specified the /plovr.net root
				IndexHandler(context);
			}
		}

		private void IndexHandler(HttpContext context) {
			throw new NotImplementedException();
		}

		private void CompileHandler(HttpContext context) {
			// get the project configuration and application settings
			PlovrProject currentProject;
			PlovrSettings currentSettings;
			this.GetActiveProjectAndSettings(context, out currentProject, out currentSettings);

			// create our new builder
			DependencyBuilder builder = new DependencyBuilder(currentProject, currentSettings);

			// if the mode isn't raw mode then we pass it off to the compiler, 
			// if it is raw mode then we handle it on our own
			if (currentProject.Mode == ClosureCompilerMode.Raw) {
				ShowRawMode(context, builder);
			}
			else {
				RunClosureCompiler(context, builder);
			}
		}

		private void ConfigHandler(HttpContext context) {
			throw new NotImplementedException();
		}

		private void ExternsHandler(HttpContext context) {
			throw new NotImplementedException();
		}

		private void InputHandler(HttpContext context) {
			throw new NotImplementedException();
		}

		private void ListHandler(HttpContext context) {
			throw new NotImplementedException();
		}

		private void ModuleHandler(HttpContext context) {
			throw new NotImplementedException();
		}

		private void ModulesHandler(HttpContext context) {
			throw new NotImplementedException();
		}

		private void SizeHandler(HttpContext context) {
			throw new NotImplementedException();
		}

		private void SourcemapHandler(HttpContext context) {
			throw new NotImplementedException();
		}

		private void ViewHandler(HttpContext context) {
			throw new NotImplementedException();
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

			// support %JAVA_HOME% env variable
			currentSettings.JavaPath = PathHelpers.ResolveJavaPath(currentSettings.JavaPath);

			// override the mode from the querystring if its passed
			currentProject.Mode = this.GetModeFromQueryString(context) ?? currentProject.Mode;

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
			string dependencyCsv = string.Join("', '", urlDependencies.ToArray());

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
			if (queryString.AllKeys.Contains(IdQueryStringParam))
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

		void context_BeginRequest(object sender, System.EventArgs e)
		{
			var context = HttpContext.Current;
			if (context.Request.RawUrl.StartsWith("/plovr/"))
			{
				context.Response.ContentType = "text/html";
				context.Response.Write("I'm Here");
				context.Response.End();
			}
		}

		public void Dispose() { }
	}
}
