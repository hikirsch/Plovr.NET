using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Plovr.Builders;
using Plovr.Model;

namespace Plovr.Helpers
{
	class ClosureTemplateRunner
	{
		private PlovrProject Project { get; set; }
		private PlovrSettings Settings { get; set; }
		private AsyncProcessHelper ProcessHelper { get; set; }


		public ClosureTemplateRunner(PlovrSettings settings, PlovrProject project)
		{
			this.Settings = settings;
			this.Project = project;
			ProcessHelper = new AsyncProcessHelper();
		}

		/// <summary>
		/// Taking a list of dependencies, use the ClosureComiplerParamBuilder to build out all the command arguments we need
		/// to execute the jar.
		/// </summary>
		/// <returns>all the params as a single string, ready to be executed and pass to the java executable</returns>
		private string BuildClosureCompilerParameters(string filePath)
		{
			SoyToJsSrcCompilerParamBuilder builder = new SoyToJsSrcCompilerParamBuilder(this.Settings.SoyToJsSrcCompilerJarPath);

			// set any other closure compiler options
			if (!string.IsNullOrEmpty(this.Project.SoyCustomParams))
			{
				builder.AddRawOptions(Project.CompilerCustomParams);
			}

			// return all the params
			return builder.GetParams();
		}

		public int Compile(string filePath, string plovrSoyContents)
		{
			string errorStringOutput = string.Empty;
			string stringOutput = string.Empty;
			string parameters = this.BuildClosureCompilerParameters(filePath);

			return ProcessHelper.ExecuteJavaCommand(this.Settings.JavaPath, parameters, out stringOutput, out errorStringOutput);
		}
	}
}
