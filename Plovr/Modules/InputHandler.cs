﻿// Copyright 2011 Ogilvy & Mather. All Rights Reserved.
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
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using Plovr.Builders;
using Plovr.Runners;

namespace Plovr.Modules
{
	internal class InputHandler : Handler
	{
		public const string GoogBaseJsResourceId = "Plovr.JavaScript.GoogBase.js";
		public const string SoyUtilsJsResourceId = "Plovr.JavaScript.soyutils.js";
		public const string SoyUtilsUseGoogResourceId = "Plovr.JavaScript.soyutils_usegoog.js";

		public InputHandler(HttpContext context) : base(context)
		{
			this.InitCurrentProject();
		}

		/// <summary>
		/// When we run, we need to parse the URL for the specific file that's being requested. If the file that's being requested
		/// ends with a ".soy", then we pass it on to the SoyToJsSrcCompiler jar and show the results from that instead. Otherwise,
		/// it streams a file from the file system.
		/// </summary>
		public override void Run()
		{
			string relFilePath = GetFilePathFromUri(CurrentProject.Id);

			if (relFilePath.Equals(DependencyBuilder.BuiltInPathForGoogBaseJs, StringComparison.InvariantCultureIgnoreCase))
			{
				string baseJs = Helpers.ResourceHelper.GetTextResourceById(InputHandler.GoogBaseJsResourceId);
				this.ShowJavaScriptResponse(baseJs);
			}
			else if( relFilePath.Equals(DependencyBuilder.BuiltInPathForSoyJsUseGoog, StringComparison.InvariantCultureIgnoreCase))
			{
				string baseJs = Helpers.ResourceHelper.GetTextResourceById(InputHandler.SoyUtilsUseGoogResourceId);
				this.ShowJavaScriptResponse(baseJs);
			}
			else if (relFilePath.Equals(DependencyBuilder.BuiltInPathForSoyUtilsJs, StringComparison.InvariantCultureIgnoreCase))
			{
				string baseJs = Helpers.ResourceHelper.GetTextResourceById(InputHandler.SoyUtilsJsResourceId);
				this.ShowJavaScriptResponse(baseJs);
			}

			foreach (string basePath in CurrentProject.Paths)
			{
				string fullFilePath = basePath + relFilePath;
				if (File.Exists(fullFilePath))
				{
					if (fullFilePath.EndsWith(".soy"))
					{
						string plovrSoyContents;
						string output;
						string errorOutput;

						var closureTemplateRunner = new ClosureTemplateRunner(this.CurrentSettings, this.CurrentProject);
						closureTemplateRunner.GetCompile(fullFilePath, out plovrSoyContents, out output, out errorOutput);

						this.ShowJavaScriptResponse(plovrSoyContents);
					}
					else
					{
						this.ShowJavaScriptFileResponse(fullFilePath);
					}
				}
			}
		}

		/// <summary>
		/// Override to get ID from the Uri path instead of from the query string.
		/// Returns the first path item after "/input/" as the id.
		/// </summary>
		protected override string GetIdFromUri() {
			MatchCollection mc = Regex.Matches(this.Context.Request.Path, @"input/([^/]+)", RegexOptions.IgnoreCase);
			string idMatch = mc[0].Groups[1].ToString();
			return idMatch;
		}

		/// <summary>
		/// Get the path to file from base path.
		/// Returns full path after the current project id.
		/// </summary>
		private string GetFilePathFromUri(string projectId) {
			MatchCollection mc = Regex.Matches(this.Context.Request.Path, @"" + projectId + "(.*)", RegexOptions.IgnoreCase);
			string pathMatch = mc[0].Groups[1].ToString();
			return pathMatch;
		}
	}
}
