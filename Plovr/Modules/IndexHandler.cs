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
using System.Text;
using System.Web;
using Plovr.Configuration;
using Plovr.Helpers;
using Plovr.Model;

namespace Plovr.Modules
{
	class IndexHandler : Handler
	{
		private const string PlovrIndexHandlerHtml = "Plovr.Html.IndexHandler.html";
		private const string PlovrIndexHandlerConfigHtml = "Plovr.Html.IndexHandlerConfig.html";

		public IndexHandler(HttpContext context) : base(context) { }

		public override void Run()
		{
			Dictionary<string, IPlovrProject> allProjects = PlovrConfiguration.GetAllProjects();
			string response = this.GenerateResponse(allProjects);
			this.ShowHtmlResponse(response);
		}

		protected string GenerateResponse(Dictionary<string, IPlovrProject> allProjects)
		{
			string indexHandlerHtml = ResourceHelper.GetTextResourceById(PlovrIndexHandlerHtml);
			string configHtml = ResourceHelper.GetTextResourceById(PlovrIndexHandlerConfigHtml);

			StringBuilder allConfigHtml = new StringBuilder();

			foreach(KeyValuePair<string, IPlovrProject> project in allProjects)
			{
				string currentConfigHtml = configHtml.Replace("%PLOVR_HANDLER%", this.GetRootUrl());
				currentConfigHtml = currentConfigHtml.Replace("%ID%", project.Key);
				allConfigHtml.Append(currentConfigHtml);
			}

			indexHandlerHtml = indexHandlerHtml.Replace("%RESPONSE%", allConfigHtml.ToString());

			return indexHandlerHtml;
		}
	}
}
