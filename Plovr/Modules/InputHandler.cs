using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using Plovr.Runners;

namespace Plovr.Modules
{
	class InputHandler : Handler
	{
		public InputHandler(HttpContext context) : base(context) { }

		public override void Run() {
			string relFilePath = GetFilePathFromUri(currentProject.Id);

			foreach (string basePath in currentProject.BasePaths) {
				string fullFilePath = basePath + relFilePath;
				if (File.Exists(fullFilePath)) {
					if (fullFilePath.EndsWith(".soy"))
					{
						string plovrSoyContents;
						string output;
						string errorOutput;

						var closureTemplateRunner = new ClosureTemplateRunner(this.currentSettings, this.currentProject);
						closureTemplateRunner.Compile(fullFilePath, out plovrSoyContents, out output, out errorOutput);

						context.Response.Write(plovrSoyContents);
//						context.Response.Write("\n/*");
//						context.Response.Write("=================================================\n");
//						context.Response.Write(output);
//						context.Response.Write("\n=================================================\n");
//						context.Response.Write(errorOutput);
//						context.Response.Write("\n*/");
						context.Response.End();
					}
					else
					{
						ShowResponse(fullFilePath);
					}
				}
			}
		}

		protected override void ShowResponse(string file) {
			context.Response.ContentType = "application/x-javascript";
			context.Response.WriteFile(file);
			context.Response.End();
		}

		/// <summary>
		/// Override to get ID from the Uri path instead of from the query string.
		/// Returns the first path item after "/input/" as the id.
		/// </summary>
		protected override string GetIdFromUri() {
			MatchCollection mc = Regex.Matches(context.Request.Path, @"input/([^/]+)", RegexOptions.IgnoreCase);
			string idMatch = mc[0].Groups[1].ToString();
			return idMatch;
		}

		/// <summary>
		/// Get the path to file from base path.
		/// Returns full path after the current project id.
		/// </summary>
		private string GetFilePathFromUri(string projectId) {
			MatchCollection mc = Regex.Matches(context.Request.Path, @"" + projectId + "(.*)", RegexOptions.IgnoreCase);
			string pathMatch = mc[0].Groups[1].ToString();
			return pathMatch;
		}
	}
}
