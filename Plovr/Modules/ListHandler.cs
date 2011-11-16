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
using System.Linq;
using System.Text;
using System.Web;
using Plovr.Builders;
using Plovr.Helpers;

namespace Plovr.Modules
{
	internal class ListHandler : Handler
	{
		private const string PlovrListHandlerHtml = "Plovr.Html.ListHandler.html";

		public ListHandler(HttpContext context) : base(context) { }

		public override void Run()
		{
			this.InitCurrentProject();
			string response = this.GenerateResponse();
			this.ShowHtmlResponse(response);
		}

		protected string GenerateResponse()
		{
			DependencyBuilder builder = new DependencyBuilder(this.CurrentProject, this.CurrentSettings);
			List<string> dependencies = (builder.GetDependencies()).ToList();
			StringBuilder responseHtml = new StringBuilder();

			foreach (string dependency in dependencies)
			{
				var inputPath = this.ResolveInputPath(dependency);
				responseHtml.Append("<a href=\"");
				responseHtml.Append(inputPath);
				responseHtml.Append("\">");
				responseHtml.Append(this.GetRelativePathOfInputFromProjectId(inputPath));
				responseHtml.Append("</a>");
				responseHtml.Append("<br />");
			}

			string listHandlerHtml = ResourceHelper.GetTextResourceById(PlovrListHandlerHtml)
				.Replace("%PLOVR_HANDLER%", this.GetRootUrl())
				.Replace("%ID%", this.CurrentProject.Id)
				.Replace("%RESPONSE%", responseHtml.ToString());

			return listHandlerHtml;
		}

		private string GetRelativePathOfInputFromProjectId(string path)
		{
			int index = path.IndexOf(this.CurrentProject.Id);

			return path.Substring(index + this.CurrentProject.Id.Length);
		}
	}
}
