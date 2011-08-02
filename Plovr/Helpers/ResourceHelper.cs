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
using System.IO;
using System.Reflection;

namespace Plovr.Helpers
{
	public static class ResourceHelper
	{
		/// <summary>
		/// Get an Embedded Resource by its ID.
		/// </summary>
		/// <param name="resourceId">the ID of the reesource</param>
		/// <returns>the text files contents</returns>
		public static string GetTextResourceById(string resourceId)
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			Stream resourceStream = assembly.GetManifestResourceStream(resourceId);
			StreamReader textStreamReader = new StreamReader(resourceStream);
			return textStreamReader.ReadToEnd();
		}

	}
}
