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
using System.Collections.Generic;
using System.Linq;
using Plovr.Configuration;
using Plovr.Helpers;
using Plovr.Model;

namespace Plovr
{
	internal static class Mappers
	{
		/// <summary>
		/// Convert a PlovrProjectElement into a PlovrProject, this is from the web.config.
		/// </summary>
		/// <param name="element">the PlovrProjectElement from web.config</param>
		/// <returns>a PlovrProject object</returns>
		public static PlovrProject ToPlovrProject(PlovrProjectElement element)
		{
			var project = new PlovrProject
			    {
			        BasePaths = ConvertStringToList(element.BasePath),
			        Externs = ConvertStringToList(element.ClosureExternFilesRaw),
					Mode = MapToEnum<ClosureCompilerMode>(element.ModeRaw),
			        Namespaces = ConvertStringToList(element.AllNamespaces),
					CustomParams = element.CustomParams
			    };

			return project;
		}

		/// <summary>
		/// Convert a PlovrSettingsElement into a PlovrSettings, this is from the web.config.
		/// </summary>
		/// <param name="element">the PlovrSettingsElement from web.config</param>
		/// <param name="basePath">The base path we have to cross reference some of our members in settings</param>
		/// <returns>a PlovrSettings object</returns>
		public static PlovrSettings ToPlovrSettings(PlovrSettingsElement element, string basePath)
		{
			var settings = new PlovrSettings
			    {
			        ClosureCompilerJarPath = PathHelpers.MakeAbsoluteFromUrlAndBasePath(element.ClosureCompilerJarPath, basePath),
			        IncludePath = element.IncludePath,
					JavaPath = PathHelpers.MakeAbsoluteFromUrlAndBasePath(element.JavaPath, basePath)
			    };

			return settings;
		}

		/// <summary>
		/// Convert a comma separated value string into a new list. Guarantee by returning a list of at least one, or null.
		/// </summary>
		/// <param name="str">a string to split</param>
		/// <returns>The string split in a list</returns>
		private static IEnumerable<string> ConvertStringToList(string str)
		{
			if (string.IsNullOrEmpty(str)) return null;

			if (str.IndexOf(',') > -1)
			{
				return str.Split(',').ToList();
			}

			return new List<string> {str};
		}

		/// <summary>
		/// Take an string value that represents an Enum and parse it.
		/// </summary>
		/// <typeparam name="TEnumType">the type of Enum to parse to</typeparam>
		/// <param name="value">the string value of the enum</param>
		/// <returns>the typed Enum or null if it wasn't valid</returns>
		public static TEnumType? MapToEnum<TEnumType>(string value) where TEnumType : struct
		{
			var mode = value;

			if (string.IsNullOrEmpty(mode))
			{
				return null;
			}

			TEnumType result = (TEnumType) Enum.Parse(typeof(TEnumType), value, true);

			return result;
		}
	}
}