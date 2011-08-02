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

namespace Plovr.Model
{
	/// <summary>
	/// This object handles the output from the closure compiler.
	/// </summary>
	public class ClosureCompilerOutput
	{
		public string StandardOutput { get; set; }
		public string StandardError { get; set; }

		public List<ClosureCompilerMessage> Messages { get; set; }

		// TOOD: error and warning count
		public int ErrorCount { get; set; }
		public int WarningCount { get; set; }
	}
}
