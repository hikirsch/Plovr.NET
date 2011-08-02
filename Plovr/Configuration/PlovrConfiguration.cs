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
using System;
using System.Configuration;
using System.Web;
using Plovr.Model;

namespace Plovr.Configuration
{
	internal class PlovrConfiguration : ConfigurationSection
    {
		public const string SectionName = "plovr";

		/// <summary>
		/// The settings node contains all the appliaction level options available.
		/// </summary>
        [ConfigurationProperty("settings") ]
		public PlovrSettingsElement Settings
		{
			get { return this["settings"] as PlovrSettingsElement; }
		}

		/// <summary>
		/// The projects node contains a single PlovrProjectElement.
		/// </summary>
		[ConfigurationProperty("projects")]
		public PlovrProjectsElement ProjectsElement
		{
			get { return this["projects"] as PlovrProjectsElement; }
		}

		/// <summary>
		/// A helper function to get all the 
		/// </summary>
		/// <returns></returns>
		public static PlovrConfiguration GetConfig()
		{
			return ConfigurationManager.GetSection(SectionName) as PlovrConfiguration;
		}

		public static void GetStrongTypedConfig(out PlovrSettings settings, out PlovrProject project, string id)
		{
			var plovrConfiguration = GetConfig();

			if( plovrConfiguration == null )
			{
				throw new NullReferenceException("The Plovr Configuration is null. Is there a section configured in the web.config properly?");
			}

			settings = Mappers.ToPlovrSettings(plovrConfiguration.Settings, HttpContext.Current.Server.MapPath("~"));

			project = Mappers.ToPlovrProject(string.IsNullOrEmpty(id)
			    ? plovrConfiguration.ProjectsElement.DefaultProjectElement
			    : plovrConfiguration.ProjectsElement.GetProjectById(id));
		}
    }
}