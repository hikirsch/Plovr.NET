using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using Plovr.Builders;
using Plovr.Configuration;
using Plovr.Helpers;
using Plovr.Model;
using Plovr.Runners;

namespace Plovr.Modules
{
	class CompileHandler : Handler
	{
		public CompileHandler(HttpContext context) : base(context) { }

		public override void Run() {
			// create our new builder
			DependencyBuilder builder = new DependencyBuilder(currentProject, currentSettings);

			// if the mode isn't raw mode then we pass it off to the compiler, 
			// if it is raw mode then we handle it on our own
			if (currentProject.Mode == ClosureCompilerMode.Raw) {
				ShowRawMode(builder);
			}
			else {
				RunClosureCompiler(builder);
			}
		}

		/// <summary>
		/// Run the closure compiler.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="builder"></param>
		private void RunClosureCompiler(DependencyBuilder builder) {
			// run the compiler
			ClosureCompilerOutput output;
			var closureCompilerRunner = new ClosureCompilerRunner(this.currentSettings, this.currentProject);
			closureCompilerRunner.Compile(builder.GetDependencies(), out output);

			// show the response from the output
			ShowResponse(output.StandardOutput, output.Messages, output.StandardError);
		}


		/// <summary>
		/// Shows the Raw mode. This will display the JavaScript resource that is embeded to dynamically load all the dependency files
		/// individually. This is the most verbose mode that Plovr supports.
		/// </summary>
		/// <param name="context">HttpContext</param>
		/// <param name="builder">the builder</param>
		private void ShowRawMode(DependencyBuilder builder) {
			// get all the dependencies and reformat the paths for URLs on the site.
			IEnumerable<string> urlDependencies = builder.GetDependencies().Select(ResolveInputPath);
			string dependencyCsv = string.Join("', '", urlDependencies.ToArray());

			// grab the javascript embedded resource.
			string plovrJavaScriptIncluder = ResourceHelper.GetTextResourceById(PlovrJavaScriptLoaderResourceId);

			plovrJavaScriptIncluder = plovrJavaScriptIncluder.Replace("%FILES%", dependencyCsv);

			ShowResponse(plovrJavaScriptIncluder, null, string.Empty);
		}


		/// <summary>
		/// Pass a full path and convert it to an input handler path
		/// </summary>
		/// <param name="fullPath">the full path to convert</param>
		/// <returns>the url of the input handler</returns>
		private string ResolveInputPath(string fullPath) {
			foreach (string basePath in currentProject.BasePaths) {
				if (fullPath.StartsWith(basePath)) {
					string relativeToBasePath = fullPath.Substring(basePath.Length+1);
					string inputPath = "/plovr.net/input/" + currentProject.Id + "/" + relativeToBasePath.Replace("\\", "/"); // TODO: more elegant way to convert backslashes
					return inputPath;
				}
			}

			throw new Exception("Matching base path not found.  Cannot resolve input path from full path.");
		}
	}
}
