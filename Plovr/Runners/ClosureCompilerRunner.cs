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
using System.IO;
using Plovr.Builders;
using Plovr.Model;

namespace Plovr.Runners
{
	public class ClosureCompilerRunner : BaseRunner
	{
		public ClosureCompilerRunner(IPlovrSettings settings, IPlovrProject project) : base(settings, project) { }

		/// <summary>
		/// Run the closure compiler
		/// </summary>
		/// <param name="dependencies">the dependencies</param>
		/// <param name="output">the standard output from the compiler</param>
		/// <returns>the exit code from the compiler</returns>
		public int Compile(IEnumerable<string> dependencies, out ClosureCompilerOutput output)
		{
			List<string> list;

			IEnumerable<string> soyDependencies = this.CreateSoyJsFromDependencies(dependencies, out list);

			string closureParams = this.BuildParams(list);

			int exitCode = ProcessHelper.ExecuteJavaCommand(this.Settings.JavaPath, closureParams, out output);

			foreach (string soyDependency in soyDependencies )
			{
				File.Delete(soyDependency);
			}
				
			return exitCode;
		}

		/// <summary>
		/// Go through all the dependencies and for each of the soy files, we need to compile it and save the path to the 
		/// result. We send the list of the soy dependencies out as well as a complete list of all the exisitng JS 
		/// dependencies with the newly generated JS files.
		/// </summary>
		/// <param name="dependencies">a list of all the dependencies</param>
		/// <param name="list">a new list of all the javascript files with the soy files replaced with the js counterpart</param>
		/// <returns>a list of all the newly created soy js source files</returns>
		private IEnumerable<string> CreateSoyJsFromDependencies(IEnumerable<string> dependencies, out List<string> list)
		{
			list = new List<string>();
			List<string> soyDependencies = new List<string>();

			ClosureTemplateRunner soyRunner = new ClosureTemplateRunner(this.Settings, this.Project);
			foreach (string dependency in dependencies)
			{
				// if this dependency is a soy, then we need to compile it and add the new temp file to the list.
				if (dependency.EndsWith(".soy"))
				{
					string soyJsSrcFile;
					string outputString;
					string errorString;

					// run compile 
					soyRunner.Compile(dependency, out soyJsSrcFile, out outputString, out errorString);

					list.Add(soyJsSrcFile);
					soyDependencies.Add(soyJsSrcFile);
				}
				else
				{
					list.Add(dependency);
				}
			}

			return soyDependencies;
		}


		/// <summary>
		/// Taking a list of dependencies, use the ClosureComiplerParamBuilder to build out all the command arguments we need
		/// to execute the jar.
		/// </summary>
		/// <returns>all the params as a single string, ready to be executed and pass to the java executable</returns>
		private string BuildParams(IEnumerable<string> dependencies)
		{
			ClosureCompilerParamBuilder builder = new ClosureCompilerParamBuilder(this.Settings.ClosureCompilerJarPath);

			// add all our dependency files
			foreach (var dependencyFilePath in dependencies)
			{
				builder.AddJavaScriptFile(dependencyFilePath);
			}

			// add any externs
			if (this.Project.Externs != null)
			{
				foreach (var externFile in this.Project.Externs)
				{
					builder.AddExternsFile(externFile);
				}
			}

			// set any other closure compiler options
			if (!string.IsNullOrEmpty(this.Project.CompilerCustomParams))
			{
				builder.AddRawOptions(Project.CompilerCustomParams);
			}

			// set the output file
			if (!string.IsNullOrEmpty(Project.OutputFile))
			{
				builder.AddOutputFile(Project.OutputFile);
			}

			// set the compilation mode
			builder.AddCompilationLevel(this.Project.Mode);

			// return all the params
			return builder.GetParams();
		}
	}
}
