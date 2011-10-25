using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Web;
using Plovr.Builders;
using Plovr.Configuration;
using Plovr.Helpers;
using Plovr.Model;

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
					context.Response.WriteFile(fullFilePath);
					context.Response.Flush();
				}
			}
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
