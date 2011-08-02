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
using System.Text.RegularExpressions;
using Plovr.Helpers;
using Plovr.Model;

namespace Plovr.Builders
{
	public class DependencyBuilder
	{
		#region "Properties and Constants"

		/// <summary>
		/// The javascript function name that delcares a namespace.
		/// </summary>
		private const string GoogProvide = "goog.provide";

		/// <summary>
		/// The javascript function name that will pull in another namespace from a different file.
		/// </summary>
		private const string GoogRequire = "goog.require";

		/// <summary>
		/// matches a string's value in a function call, used for detecting namespaces and require's inside goog.provide("{some namespace}")
		/// </summary>
		private const string JavascriptFunctionCallWithParam = @"{0}\s*\(\s*'([^']+)'\s*\)|{0}\s*\(\s*""([^""]+)""\s*\)";

		/// <summary>
		/// The path to where the namespace and require methods are defined. Base.js is the core to the entire framework. It must always be here.
		/// </summary>
		private const string PathToBaseJS = @"\goog\base.js";

		/// <summary>
		/// The current project we are going to be building for.
		/// </summary>
		private IPlovrProject Project { get; set; }

		/// <summary>
		/// The current project we are going to be building for.
		/// </summary>
		private IPlovrSettings Settings { get; set; }

		/// <summary>
		/// For the dependency tree, multiple provides can be declared in a single file but that namespace can only be declared once globally in a project
		/// </summary>
		private Dictionary<string, string> Provide { get; set; }

		/// <summary>
		/// For the dependency tree, a require can be 
		/// </summary>
		private Dictionary<string, List<string>> Require { get; set; }

		private AsyncProcessHelper ProcessHelper { get; set; }

		#endregion

		#region "Init"

		/// <summary>
		/// Our constructor, allows you to pass in a single base path that is comma delimited.
		/// </summary>
		/// <param name="projectElement">the project we want to compile</param>
		/// <param name="settings">the settings we use to compile</param>
		public DependencyBuilder(IPlovrProject projectElement, IPlovrSettings settings)
		{
			this.Project = projectElement;
			this.Settings = settings;
	
			// this.BasePaths = basePath.IndexOf(',') > 0 ? basePath.Split(',').ToList() : new List<string> { basePath };

			this.Provide = new Dictionary<string, string>();
			this.Require = new Dictionary<string, List<string>>();

			// we need to read output asynchronously. so we use a helper class
			ProcessHelper = new AsyncProcessHelper();

			// build out the dependency tree.
			this.BuildDepedencyTree();
		}

		#endregion

		#region "Build the Dependency Tree"

		/// <summary>
		/// Start building the depedency tree.
		/// </summary>
		private void BuildDepedencyTree()
		{
			foreach (string path in this.Project.BasePaths)
			{
				BuildDepedencyTree(path);
			}
		}

		/// <summary>
		/// Scan the directory passed and build the dependency tree.
		/// </summary>
		/// <param name="currentPath">the path to start scanning</param>
		private void BuildDepedencyTree(string currentPath)
		{
			// recurse through all the directories
			var currentDirectories = Directory.GetDirectories(currentPath);
			foreach (var directory in currentDirectories)
			{
				BuildDepedencyTree(directory);
			}

			// list all the JS files in this folder we don't care about any of the others.
			var currentFiles = Directory.GetFiles(currentPath, "*.JS");
			foreach (var currentFile in currentFiles)
			{
				GetDetailsFromFilePath(currentFile);
			}
		}

		/// <summary>
		/// Parse the dependency tree info from the passed in file.
		/// </summary>
		/// <param name="filePath">a file to parse</param>
		private void GetDetailsFromFilePath(string filePath)
		{
			var contents = this.GetFileContentsWithoutJavaScriptComments(filePath);

			// this exception should never happen but nonetheless, here it is.
			if (this.Require.ContainsKey(filePath))
			{
				throw new Exception("This JavaScript file has already been parsed. Something really went wrong. The file that got parsed again was: " + filePath);
			}

			this.Require.Add(filePath, new List<string>());

			////////////////////////////////////////////////
			// Handle the provides. (GoogProvide)
			////////////////////////////////////////////////
			// setup our pattern, this replace allows me to maintain the require and provide statements without worrying about the syntax of a regex
			var providePattern = string.Format(JavascriptFunctionCallWithParam, GoogProvide.Replace(".", @"\."));
			MatchCollection provideMatch = Regex.Matches(contents, providePattern);
			if (provideMatch.Count > 0)
			{
				// we can have multiple provides in the same file, so we have to go through all of them.
				foreach (Match match in provideMatch)
				{
					for (int i = 0; i < match.Groups.Count; i++)
					{
						if (match.Groups[i].Success)
						{
							// get the value of what this file provides
							string value = match.Groups[i].Value;

							// you are only allowed to declare a namespace once, if it gets declared again, we throw this error
							if (this.Provide.ContainsKey(value))
							{
								var declaredNamespace = this.Provide[value];
								throw new Exception("The namespace '" + value + "' has already been declared in '" + declaredNamespace + "' and '" + filePath + "'.");
							}

							// add the provide
							this.Provide.Add(value, filePath);
						}
					}
				}
			}

			////////////////////////////////////////////////
			// Handle the requires. (GoogRequire)
			////////////////////////////////////////////////
			var requirePattern = string.Format(JavascriptFunctionCallWithParam, GoogRequire.Replace(".", @"\."));
			MatchCollection requireMatch = Regex.Matches(contents, requirePattern, RegexOptions.Singleline);
			if (requireMatch.Count > 0)
			{
				// go through each match.
				foreach (Match match in requireMatch)
				{
					for (int i = 0; i < match.Groups.Count; i++)
					{
						if (match.Groups[i].Success)
						{
							// get the value of the match and add it
							string value = match.Groups[i].Value;
							if (value.Contains("goog.require"))
							{
								throw new Exception("error parsing file: " + filePath + ". Namespace detected as: '" + value + "'");
							}
							this.Require[filePath].Add(value);
						}
					}
				}
			}
		}

		/// <summary>
		/// Read the file contents from the path passed, and remove the javascript comments from the file.
		/// </summary>
		/// <param name="filePath">the path of the javascript file to read</param>
		/// <returns>the file's contents with no comments</returns>
		private string GetFileContentsWithoutJavaScriptComments(string filePath)
		{
			var contents = File.ReadAllText(filePath);
			
			// first attempt at removing javascript comments
			contents = Regex.Replace(contents, @"/\*(.|[\n])*?\*/", string.Empty, RegexOptions.Singleline);	
			contents = Regex.Replace(contents, @"//(.*)\n", string.Empty, RegexOptions.Multiline);
			return contents;
		}

		#endregion

		#region "Build Dependency Tree via a Namespace"

		/// <summary>
		/// Once a dependency tree is built, we can pass a namespace here and figure out what files are necessary to compile that namespace.
		/// This function is a variant and takes a list of strings where each item is a single namespace.
		/// </summary>
		/// <returns>a list of strings of the dependencies necessary to load the namespace passed (in order)</returns>
		public IEnumerable<string> GetDependencies()
		{
			// we must add the base js file, so it goes here.
			// TODO: check this.BasePaths to see if it contains PATH_TO_BASE_JS instead of hack
			List<string> dependencies = new List<string> { this.Project.BasePaths.ToList()[0] + PathToBaseJS };

			foreach (var ns in this.Project.Namespaces)
			{
				this.RecurseDependencies(ns, ref dependencies);
			}

			return dependencies;
		}

		/// <summary>
		/// Build out the dependencies needed to load the passed in namespace. This is a recursive function that detects what
		/// namespaces the passed in namespace requires and calls itself to resolve the depedencies. A dependency array is passed
		/// by reference to support the ability of recursing.
		/// </summary>
		/// <param name="jsNamespace">the namespace to get the dependencies for</param>
		/// <param name="dependencyList">the dependency list for all of the namespaces requested</param>
		private void RecurseDependencies(string jsNamespace, ref List<string> dependencyList)
		{
			// if the namespace we're requesting to build the dependencies for doesn't exist, we throw this exception
			if (!this.Provide.ContainsKey(jsNamespace))
			{
				throw new Exception("Tried to include the namespace '" + jsNamespace + "' when it was not provided. ");
			}

			// get the file that provides the namespace we're looking to get details on.
			string filePath = this.Provide[jsNamespace];

			// it's possible that this file may not even require anything and is a standalone file that can be provided, if thats
			// the case then we dont need to recurse.
			if (this.Require.ContainsKey(filePath))
			{
				// go through each of the namespaces that this file needs and add their dependencies as well.
				foreach (string dependencyNamespace in this.Require[filePath])
				{
					this.RecurseDependencies(dependencyNamespace, ref dependencyList);
				}
			}

			// if 2 namespaces both required the same file, it may have already been included at one point, therefore don't re-include it again.
			if (!dependencyList.Contains(filePath))
			{
				dependencyList.Add(filePath);
			}
		}

		#endregion

		#region "Closure Compiler Integration"

		/// <summary>
		/// Run the closure compiler
		/// </summary>
		/// <param name="output">the standard output from the compiler</param>
		/// <returns>the exit code from the compiler</returns>
		public int Compile(out ClosureCompilerOutput output)
		{
			var closureParams = this.BuildClosureCompilerParameters();

			return ProcessHelper.ExecuteJavaCommand(this.Settings.JavaPath, closureParams, out output);
		}


		/// <summary>
		/// Taking a list of dependencies, use the ClosureComiplerParamBuilder to build out all the command arguments we need
		/// to execute the jar.
		/// </summary>
		/// <returns>all the params as a single string, ready to be executed and pass to the java executable</returns>
		private string BuildClosureCompilerParameters()
		{
			ClosureCompilerParamBuilder builder = new ClosureCompilerParamBuilder(this.Settings.ClosureCompilerJarPath);

			IEnumerable<string> dependencies = this.GetDependencies();

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
			if (!string.IsNullOrEmpty(this.Project.CustomParams))
			{
				builder.AddRawOptions(Project.CustomParams);
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

		#endregion
	}
}