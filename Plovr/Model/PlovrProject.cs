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
	public class PlovrProject : IPlovrProject
	{
		public string Id { get; set; }
		public IEnumerable<string> BasePaths { get; set; }
		public IEnumerable<string> Namespaces { get; set; }
		public IEnumerable<string> Externs { get; set; }
		public ClosureCompilerMode? Mode { get; set; }
		public string OutputFile { get; set; }
		public string CustomParams { get; set; }
	}
}
