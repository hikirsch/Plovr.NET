using System.IO;
using Plovr.Builders;
using Plovr.Model;

namespace Plovr.Runners
{
	public class ClosureTemplateRunner : BaseRunner
	{
		private const string ParamUseGoogRequireProvide = "shouldProvideRequireSoyNamespaces";

		public ClosureTemplateRunner(IPlovrSettings settings, IPlovrProject project) : base(settings, project) { }

		/// <summary>
		/// Using the SoyToJsSrcCompilerParamBuilder, build up all the params so we can pass this to the AsyncProcessHelper.
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="tempFilePath"></param>
		/// <returns>all the params as a single string, ready to be executed and passed to the java executable</returns>
		private string BuildParams(string filePath, string tempFilePath)
		{
			SoyToJsSrcCompilerParamBuilder builder = new SoyToJsSrcCompilerParamBuilder(this.Settings.SoyToJsSrcCompilerJarPath);

			// set any other closure compiler options
			if (!string.IsNullOrEmpty(this.Project.SoyCustomParams))
			{
				builder.AddRawOptions(Project.CompilerCustomParams);
			}

			builder.AddCodeStyle("concat");
			builder.ShouldGenerateJsDoc();

			builder.AddOutputFilePathFormat(tempFilePath);

			// since this is plovr, we are going to be providing goog.base anyway, so we can use goog.require statements here
			builder.AddParamQuotedValue(ParamUseGoogRequireProvide, "true");

			builder.AddFileToCompile(filePath);

			// return all the params
			return builder.GetParams();
		}

		/// <summary>
		/// Run the SoyToJsSrc Compiler. Unfortunately, this compiler saves the output to a file. So we ask .NET to give use
		/// a temporary file, run it, get the contents and then delete it.
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="plovrSoyContents"></param>
		/// <param name="outputString"></param>
		/// <param name="errorStringOutput"></param>
		/// <returns></returns>
		public int GetCompile(string filePath, out string plovrSoyContents, out string outputString, out string errorStringOutput)
		{
			string tempFilePath;

			int exitCode = this.Compile(filePath, out tempFilePath, out outputString, out errorStringOutput);
			plovrSoyContents = File.ReadAllText(tempFilePath);
			File.Delete(tempFilePath);

			return exitCode;
		}

		public int Compile(string filePath, out string tempFilePath, out string outputString, out string errorStringOutput)
		{
			tempFilePath = Path.GetTempFileName();
			string parameters = this.BuildParams(filePath, tempFilePath);

			int exitCode = ProcessHelper.ExecuteJavaCommand(
				this.Settings.JavaPath,
				parameters,
				out outputString,
				out errorStringOutput
			);

			return exitCode;
		}
	}
}