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
namespace Plovr.Model
{
	/// <summary>
	/// This class represents all the fields that constitute a message for the Closure Compiler.
	/// </summary>
	public class ClosureCompilerMessage
	{
		public PlovrMessageType? Type { get; set; }
		public string ErrorMessage { get; set; }
		public string FilePath { get; set; }
		public int LineNumber { get; set; }
		public string CodeLine { get; set; }
		public int ErrorStartIndex { get; set; }
	}
}
