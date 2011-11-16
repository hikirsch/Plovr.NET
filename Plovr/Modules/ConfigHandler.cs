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

using System.Text.RegularExpressions;
using System.Web;

namespace Plovr.Modules
{
	class ConfigHandler : Handler
	{
		public ConfigHandler(HttpContext context) : base(context) { }

		public override void Run() {
			this.InitCurrentProject();

			this.ShowJavaScriptFileResponse(this.CurrentProject.ConfigPath);
		}

		/// <summary>
		/// Override to get ID from the Uri path instead of from the query string.
		/// Returns the first path item after "/input/" as the id.
		/// </summary>				
		protected override string GetIdFromUri()
		{
			MatchCollection mc = Regex.Matches(this.Context.Request.Path, @"config/([^/]+)", RegexOptions.IgnoreCase);
			string idMatch = mc[0].Groups[1].ToString();
			return idMatch;
		}
	}
}
