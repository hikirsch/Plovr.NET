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

namespace Plovr.Helpers
{
	public static class PathHelpers
	{
		/// <summary>
		/// Pass a full path and convert it to a URL basd on the current HttpContext.
		/// </summary>
		/// <param name="fullPath">the full path to convert</param>
		/// <returns>the url of where this item would be</returns>
		public static string MakeRelativeFromPath(string fullPath)
		{
			Uri fullPathUri = new Uri(fullPath);

			string applicationPathUri = System.Web.HttpContext.Current.Server.MapPath("~");

			var webRootUri = new Uri(applicationPathUri);

			return "/" + webRootUri.MakeRelativeUri(fullPathUri);
		}

		/// <summary>
		/// Go through a list and convert all the contents relative pathing to the base path passed.
		/// </summary>
		/// <see cref="MakeRelativeFromPath"/>
		/// <param name="rawList">the list of paths that are shorthand url</param>
		/// <param name="basePath">the base path to calculate the relative path against</param>
		/// <returns>a list of paths converted into full paths</returns>
		public static IEnumerable<string> MakeAbsoluteFromUrlAndBasePath(IEnumerable<string> rawList, string basePath )
		{
			if (rawList == null) return null;

			var list = rawList.ToList();
			// fix the extern paths.
			for (var index = 0; index < list.Count; index++)
			{
				list[index] = MakeAbsoluteFromUrlAndBasePath(list[index], basePath);
			}

			return list;
		}

		/// <summary>
		/// Takes a path and normalizes it from a URL path to a valid windows path. This function does 2 things to a path:
		///		A) replaces "~" with the the base path that is passed in.
		///		B) replaces "/" with "\"
		/// </summary>
		/// <param name="path">a short hand style path for an ASPNET application</param>
		/// <param name="basePath">the base path to normalize the path with</param>
		/// <returns>a fixed path</returns>
		public static string MakeAbsoluteFromUrlAndBasePath(string path, string basePath)
		{
			if (string.IsNullOrEmpty(path)) return null;

			string fixedPath = path.Replace('/', '\\');
			return fixedPath.Replace("~", basePath);
		}

		/// <summary>
		/// Using the path passed in, if it contains %JAVA_HOME%, set it with what its set.
		/// </summary>
		/// <param name="javaPath"></param>
		/// <returns>the path with the JAVA_HOME environmental variable replaced</returns>
		public static string ResolveJavaPath(string javaPath)
		{
			string currentPath = javaPath;
			if (currentPath.IndexOf("%JAVA_HOME%") > -1)
			{
				var javaHome = Environment.GetEnvironmentVariable("JAVA_HOME");
				currentPath = currentPath.Replace("%JAVA_HOME%", javaHome);
			}

			return currentPath;
		}
	}
}