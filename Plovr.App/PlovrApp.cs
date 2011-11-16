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
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Newtonsoft.Json;
using Plovr.Builders;
using Plovr.Helpers;
using Plovr.Model;
using Plovr.Runners;

namespace Plovr.App
{
	public class PlovrApp
	{
		#region "Properties"

		/// <summary>
		/// A Hashtable of the command line arguments that were passed when executed.
		/// </summary>
		private Hashtable ProgramArguments { get; set; }

		/// <summary>
		/// The path of the webroot. This is passed via the command line.
		/// </summary>
		private string WebRootPath { get; set; }

		/// <summary>
		/// The path of the JSON config file. This is passed via the command line.
		/// </summary>
		private string ConfigFilePath { get; set; }

		/// <summary>
		/// The project we want to build.
		/// </summary>
		private IPlovrProject Project { get; set; }

		/// <summary>
		/// The settings for the app
		/// </summary>
		public PlovrSettings Settings { get; set; }

		/// <summary>
		/// The dependency builder. This does all the actual work.
		/// </summary>
		private DependencyBuilder Builder { get; set; }

		#endregion

		/// <summary>
		/// Our main constructor for the application, pass the command line arguments straight up from main.
		/// </summary>
		/// <param name="args">The raw command line arguments from main</param>
		public PlovrApp(params string[] args)
		{
			// this has to be done in the right order, load our command line arguments first
			this.LoadAndValidateCommandLineParams(args);

			// load our config from json
			this.LoadConfig();

			// create our dependency builder
			this.InitDependencyBuilder();

			// our verbose mode, show everything that happens
			if (this.ProgramArguments.ContainsKey("verbose"))
			{
				this.ShowAllOptions();
				this.ShowNamespaceDependencies();
			}

			// run compilation
			if (this.ProgramArguments.ContainsKey("compile"))
			{
				this.RunCompiler();
			}
		}

		/// <summary>
		/// List  all of the files that are necessary to build the namespaces in the config.
		/// </summary>
		private void ShowNamespaceDependencies()
		{
			var files = Builder.GetDependencies();

			Console.WriteLine("--- Start Depedencies List ---");

			foreach (string file in files)
			{
				Console.WriteLine(file);
			}

			Console.WriteLine("--- End Depedencies List ---");
			Console.WriteLine();
		}

		/// <summary>
		/// Run the closure compiler.
		/// </summary>
		/// <returns>The output of what the compiler did.</returns>
		private void RunCompiler()
		{
			ClosureCompilerOutput output;

			Console.WriteLine("--- Running Compiler ---");
			var closureCompilerRunner = new ClosureCompilerRunner(this.Settings, this.Project);
			closureCompilerRunner.Compile(Builder.GetDependencies(), out output);
			Console.WriteLine("--- Compiler Complete ---");
			Console.WriteLine();

			Console.WriteLine("---- Start Output ----");
			Console.WriteLine(output.StandardOutput);
			Console.WriteLine("---- End Output ----");

			Console.WriteLine("---- Start Error Output ----");
			foreach (ClosureCompilerMessage plovrMessage in output.Messages)
			{
				Console.WriteLine("Code Line: " + plovrMessage.CodeLine);
				Console.WriteLine("ErrorMessage: " + plovrMessage.ErrorMessage);
				Console.WriteLine("ErrorStartIndex: " + plovrMessage.ErrorStartIndex);
				Console.WriteLine("FilePath: " + plovrMessage.FilePath);
				Console.WriteLine("LineNumber: " + plovrMessage.LineNumber);
				Console.WriteLine("Type: " + plovrMessage.Type);
			}
			Console.WriteLine("---- End Error Output ----");
			Console.WriteLine();

			return;
		}

		/// <summary>
		/// Show all the options being passed into the Builder and the Project
		/// </summary>
		private void ShowAllOptions()
		{
			Console.WriteLine("--- Project Options ---");
			Console.WriteLine("Java Path: " + this.Settings.JavaPath);
			
			if (this.Project.Namespaces != null)
			{
				Console.WriteLine("Namespaces: " + String.Join(", ", this.Project.Namespaces));
			}
			
			if (this.Project.Paths != null)
			{
				Console.WriteLine("Base Paths: " + String.Join(", ", this.Project.Paths));	
			}
			
			Console.WriteLine("Output File: " + this.Project.OutputFile);
			Console.WriteLine("Closure Compiler Mode: " + this.Project.Mode);
			
			if (this.Project.Externs != null)
			{
				Console.WriteLine("Closure Extern Files: " + String.Join(", ", this.Project.Externs));
			}
			
			Console.WriteLine("--- End Project Options ---");
			Console.WriteLine();
		}

		/// <summary>
		/// Init the DependencyBuilder so that we can start using it to build stuff.
		/// </summary>
		private void InitDependencyBuilder()
		{
			// create a base builder

			// string closureJarPath = ConfigurationManager.AppSettings["ClosureCompilerJarPath"];

			this.Builder = new DependencyBuilder(Project, Settings);

			// set the closure jar path from the app.config
		}

		/// <summary>
		/// Parse through the command line options and validate that it is correct. If it is not, the application will 
		/// exit. If it is accurate, it will populate the Project and ProgramArguments parameters in this class.
		/// </summary>
		/// <param name="args">The raw arguments from the main class</param>
		private void LoadAndValidateCommandLineParams(IEnumerable<string> args)
		{
			// take the command line arguments and parse it.
			this.ParseCommandLineArguments(args);

			this.Settings = this.CreateSettings();

			// after we parse it, we validate it
			this.ValidateArgs();

			// get the values from the command line arguments hash
			this.WebRootPath = this.ProgramArguments["webRootPath"].ToString();
			this.ConfigFilePath = this.ProgramArguments["config"].ToString();
		}

		private PlovrSettings CreateSettings()
		{
			// IPlovrSettings settings = new PlovrSettings();
			// settings.ClosureCompilerJarPath = "";
			// settings.CustomParams = Project.ExtraParams;
			return null;
		}

		private void ParseCommandLineArguments(IEnumerable<string> args)
		{
			this.ProgramArguments = new Hashtable();
			foreach (string value in args)
			{
				// check to see whether or not we have a valid set of arguments, they should all start with a "/"
				if (value[0] == '/')
				{
					// each argument is a pair so split on this.
					var charArray = ":".ToCharArray();
					string[] hashPair = value.Substring(1).Split(charArray, 2);

					// we may not have a value for the argument, it can be something like /verbose so handle
					string hashKey = hashPair[0];
					string hashValue = null;

					if (hashPair.Length > 1)
					{
						hashValue = hashPair[1];
					}

					// add our new argument
					if( string.IsNullOrEmpty(hashValue))
					{
						this.ProgramArguments.Add(hashKey, null);	
					} else
					{
						this.ProgramArguments.Add(hashKey, hashValue.Replace("\"", ""));	
					}
				}
				else
				{
					// something is wrong withb the arugment, show it
					Console.WriteLine("Invalid Param: '" + value + "'");
					Console.WriteLine(Instructions.ShowUsage());
					Console.WriteLine();

					Environment.Exit(1);
				}
			}
		}

		/// <summary>
		/// Validate the command line arguments and act on help if we need to.
		/// </summary>
		private void ValidateArgs()
		{
			// show help
			if (this.ProgramArguments.ContainsKey("help") || this.ProgramArguments.ContainsKey("?"))
			{
				Console.WriteLine(Instructions.ShowFull());
				Environment.Exit(1);
			}

			// show help
			if (this.ProgramArguments.ContainsKey("version"))
			{
				Console.WriteLine(Instructions.GetVersion());
				Environment.Exit(1);
			}

			// check for the root path of the project
			if (!this.ProgramArguments.ContainsKey("webRootPath"))
			{
				Console.WriteLine("A webRootPath is required!");
				Console.WriteLine();
				Console.WriteLine(Instructions.ShowUsage());
				Environment.Exit(1);
			}


			// check for config
			if (!this.ProgramArguments.ContainsKey("config"))
			{
				Console.WriteLine("A config file is required!");
				Console.WriteLine();
				Console.WriteLine(Instructions.ShowUsage());
				Environment.Exit(1);
			}
		}

		/// <summary>
		/// The paths we use in the JSON file are relative paths to what would be the webroot, e.g. ~/Assets/js/my-compiled-script.js.
		/// This function is responsible for changing the config object passed so that it is all based off absolute paths instead of
		/// relative paths for the assumed webroot. The basePath should be passed in from the build server so context is put into place.
		/// </summary>
		private void LoadConfig()
		{
			this.Project = this.GetConfigFromFileNewtonsoft(this.ConfigFilePath);

			this.Project.Externs = PathHelpers.MakeAbsoluteFromUrlAndBasePath(this.Project.Externs, this.WebRootPath);
			this.Project.Paths = PathHelpers.MakeAbsoluteFromUrlAndBasePath(this.Project.Paths, this.WebRootPath);
			this.Project.OutputFile = PathHelpers.MakeAbsoluteFromUrlAndBasePath(this.Project.OutputFile, this.WebRootPath);

			this.Settings = new PlovrSettings
			    {
			        ClosureCompilerJarPath = ConfigurationManager.AppSettings["ClosureCompilerJarPath"],
			        JavaPath = PathHelpers.ResolveJavaPath(ConfigurationManager.AppSettings["JavaExePath"])
			    };
		}

		/// <summary>
		/// Read the JSON file using Newtonsoft.Json. Newtonsoft was preferred over the .NET native parser since the .NET
		/// native one doesn't allow comments in the JS file itself. 
		/// </summary>
		/// <param name="fileName">the file to parse</param>
		/// <returns>a PlovrJsonConfig object representation of the JSON file passed</returns>
		public IPlovrProject GetConfigFromFileNewtonsoft(string fileName)
		{
			IPlovrProject config;

			JsonSerializer serializer = new JsonSerializer
				{
					NullValueHandling = NullValueHandling.Ignore
				};

			using (StreamReader sr = new StreamReader(fileName))
			using (JsonTextReader reader = new JsonTextReader(sr))
			{
				config = (PlovrJsonConfig)serializer.Deserialize(reader, typeof(PlovrJsonConfig));
			}

			return config;
		}
	}
}