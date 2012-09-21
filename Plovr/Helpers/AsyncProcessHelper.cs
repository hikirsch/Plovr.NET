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
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Plovr.Model;

namespace Plovr.Helpers
{
	public class AsyncProcessHelper
	{
		/// <summary>
		/// Our main regex when parsing the StandardError stream from the compiler into a strong typed object.
		/// </summary>
		private const string MessageMatch = @"^(.*)\:(\d+)\:(.*)\-(.*)\n(.*)\n(.*)\n$";

		/// <summary>
		/// This StringBuilder is used when running the process asynchronously which is required when reading both StandardOutput and StandardError streams. This shared variable
		/// stores the StandardOutput stream.
		/// </summary>
		private StringBuilder ProcessOutput { get; set; }

		/// <summary>
		/// This StringBuilder is used when running the process asynchronously which is required when reading both StandardOutput and StandardError streams. This shared variable
		/// stores the StandardError stream.
		/// </summary>
		private StringBuilder ProcessErrorOutput { get; set; }

		/// <summary>
		/// Execute a java command with the params being passed.
		/// </summary>
		/// <param name="exePath">the path to the exe that should execute.</param>
		/// <param name="parameters">java command line parameters</param>
		/// <param name="output">the output of the compilation process</param>
		/// <returns>the exit code from the compiler</returns>
		public int ExecuteJavaCommand(string exePath, string parameters, out ClosureCompilerOutput output)
		{
			string stringOutput;
			string stringErrorOutput;
			int exitCode = this.ExecuteJavaCommand(exePath, parameters, out stringOutput, out stringErrorOutput);

			output = new ClosureCompilerOutput
			         	{
			         		StandardOutput = stringOutput.ToString(),
			         		StandardError = stringErrorOutput.ToString(),
			         		Messages = ParsePlovrMessages(stringErrorOutput.ToString())
			         	};

			return exitCode;
		}

		public int ExecuteJavaCommand(string exePath, string parameters, out string output, out string errorOutput)
		{
			int exitCode;

			// create our new start info object with the exe and parameters
			ProcessStartInfo startInfo = new ProcessStartInfo(exePath, parameters)
			{
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = true
			};

			// prepare our shared string builders for async read
			ProcessOutput = new StringBuilder();
			ProcessErrorOutput = new StringBuilder();

			try
			{
				// start the process
				using (Process process = Process.Start(startInfo))
				{
					// set our read events
					process.OutputDataReceived += ServerOutputHandler;
					process.ErrorDataReceived += ServerErrorHandler;

					// start reading in lines.
					process.BeginErrorReadLine();
					process.BeginOutputReadLine();

					// now just wait till we're done 
					process.WaitForExit();

					exitCode = process.ExitCode;
				}

				// create our object to send back with all the output
				output = ProcessOutput.ToString();
				errorOutput = ProcessErrorOutput.ToString();

				// reset them back to null since we got them already.z
				ProcessErrorOutput = null;
				ProcessOutput = null;
			}
			catch (Win32Exception e)
			{
				throw new Win32Exception(e.Message + "\n" + "EXE Path: '" + exePath + "'\nParams: '" + parameters + "'");
			}

			return exitCode;
		}

		/// <summary>
		/// We take the entire StandardError stream as a string and we parse it to be a List of ClosureCompilerMessage's. This way, we can serialize them
		/// and send them back to the client for easier integration with a console.
		/// </summary>
		/// <param name="messagesString">the raw string from the closure compiler.</param>
		/// <returns>a parsed list of ColsureCompilerMessage's</returns>
		private List<ClosureCompilerMessage> ParsePlovrMessages(string messagesString)
		{
			var messages = new List<ClosureCompilerMessage>();

			MatchCollection messageMatches = Regex.Matches(messagesString, MessageMatch, RegexOptions.Multiline);
			if (messageMatches.Count > 0)
			{
				// we can have multiple provides in the same file, so we have to go through all of them.
				foreach (Match match in messageMatches)
				{
					if (match.Groups[1].Success)
					{
						var codeLine = match.Groups[5].Value;
						// tabs are a bit of a problem in terms of how much to show, so we replace it with 4 spaces.
						codeLine = codeLine.Replace("\t", "    ");
						
						var errorStartIndex = match.Groups[6].Value;
						// more tabs
						errorStartIndex = errorStartIndex.Replace("\t", "    ");

						var message = new ClosureCompilerMessage
						    {
						        FilePath = match.Groups[1].Value.Trim(),
						        LineNumber = int.Parse(match.Groups[2].Value),
						        Type = Mappers.MapToEnum<PlovrMessageType>(match.Groups[3].Value),
						        ErrorMessage = match.Groups[4].Value.Trim(),
								CodeLine = codeLine,
						        ErrorStartIndex = errorStartIndex.Length
						    };

						// if the code line is empty when its trimed, then we clear out the start index and the code line.
						if( codeLine.Trim().Length == 0)
						{
							message.CodeLine = string.Empty;
							message.ErrorStartIndex = 0;
						}

						messages.Add(message);
					}
				}
			}

			return messages;
		}

		/// <summary>
		/// Our delegate method to handle the StandardOutput stream . It will append a string to a shared StringBuilder member on this class so we can continue to
		/// read the streams asynchronously.
		/// </summary>
		/// <param name="sender">the sender</param>
		/// <param name="e">the data</param>
		private void ServerOutputHandler(object sender, DataReceivedEventArgs e)
		{
			ProcessOutput.Append(e.Data + "\n");
		}

		/// <summary>
		/// Our delegate method to handle the StandardError stream . It will append a string to a shared StringBuilder member on this class so we can continue to
		/// read the streams asynchronously.
		/// </summary>
		/// <param name="sender">the sender</param>
		/// <param name="e">the data</param>
		private void ServerErrorHandler(object sender, DataReceivedEventArgs e)
		{
			ProcessErrorOutput.Append(e.Data + "\n");
		}
	}
}
