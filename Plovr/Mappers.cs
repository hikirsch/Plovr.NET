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
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
		/// <param name="configPath">the path to the json config file</param>
		/// <param name="basePath">a base path to resolve all the paths in the json to absolute paths</param>
		/// <returns>a PlovrProject object</returns>
		public static IPlovrProject ToPlovrProject(string configPath, string basePath)
		{
			string filePath = PathHelpers.MakeAbsoluteFromUrlAndBasePath(configPath, basePath);
			IPlovrProject project = Mappers.GetConfigFromFileNewtonsoft(filePath);

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
					SoyToJsSrcCompilerJarPath = PathHelpers.MakeAbsoluteFromUrlAndBasePath(element.SoyToJsSrcCompilerJarPath, basePath),
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


		/// <summary>
		/// Read the JSON file using Newtonsoft.Json. Newtonsoft was preferred over the .NET native parser since the .NET
		/// native one doesn't allow comments in the JS file itself. 
		/// </summary>
		/// <param name="fileName">the file to parse</param>
		/// <returns>a PlovrJsonConfig object representation of the JSON file passed</returns>
		public static IPlovrProject GetConfigFromFileNewtonsoft(string fileName)
		{
			PlovrJsonConfig config = null;

			JsonSerializer serializer = new JsonSerializer
			{
				NullValueHandling = NullValueHandling.Ignore
			};

			using (StreamReader sr = new StreamReader(fileName))
			using (JsonTextReader reader = new JsonTextReader(sr))
			{
				config = serializer.Deserialize<PlovrJsonConfig>(reader);
			}

			return config;
		}
	}
}