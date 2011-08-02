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
using System.Reflection;

namespace Plovr.App
{
	/// <summary>
	/// This class contains all the help copy for the App.
	/// TODO: move to external Resource file.
	/// </summary>
	public static class Instructions
	{
		public const string FULL_HEADER = @"
This application allows you to compile a JavaScript project easier when 
working with a ASPNET or MVC project. This application accepts multiple 
parameters. A config file is the main input for this application and 
configures project specific properties such as paths, output file 
destination.
";

		public const string SHORT_HEADER = "A wrapper dependency checker for Google's Closure Compiler.";

		public const string USAGE = @"
PlovrApp [/?] [/help] [/version] [/verbose] 
	[/config:filepath] [/webRootPath:path] 
";

		public const string PARAM_HELP = @"
    /? /help          this help menu.
";

		public const string PARAM_VERSION = @"
    /version          shows the current version of this application
";

		public const string PARAM_VERBOSE = @"
    /verbose          show all details about what would happen, this
                      will also show the dependencies for the
                      namespaces
";

		public const string PARAM_CONFIG = @"
    /config:filepath  the path to the plovrConfig, see full-example.js
";

		public const string PARAM_WEBROOT_PATH = @"
    /webRootPath:path the path to the website root for a project. a 
                      js file can reference this root via ~/ 
";

		public static string ShowFull()
		{
			return GetVersion() + 
				FULL_HEADER +
				USAGE +
				ShowAllParams();
		}

		public static string ShowUsage()
		{
			return GetVersion() + 
				SHORT_HEADER +
				USAGE;	
		}

		public static string ShowAllParams()
		{
			return PARAM_HELP +
				PARAM_VERSION +
				PARAM_CONFIG +
				PARAM_WEBROOT_PATH +
				PARAM_VERBOSE;
		}

		public static string GetVersion()
		{
			return "Plovr.App v" + Assembly.GetExecutingAssembly().GetName().Version + "\n";
		}
	}
}
