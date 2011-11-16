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
	/// A Plovr project can be saved in different models. This is the interface for the data needed to define a project.
	/// </summary>
	public interface IPlovrProject
	{
		string ConfigPath { get; set; }

		string Id { get; set; }
		IEnumerable<string> Inputs { get; set; }
		IEnumerable<string> Paths { get; set; }
		IEnumerable<string> Externs { get; set; }
		ClosureCompilerMode? Mode { get; set; }
		
		string CompilerCustomParams { get; set; }
		string SoyCustomParams { get; set; }
		IEnumerable<string> Namespaces { get; set; }
	}
}

/*
Below are all the options from http://code.google.com/p/plovr/source/browse/src/org/plovr/ConfigOption.java rev d6db24beeb7f)

[x] id
[x] inputs
[x] paths
[x] externs
[ ] custom-externs-only
[ ] closure-library
[ ] experimental-exclude-closure-library
[x] mode
[ ] level
[ ] inherits
[ ] debug
[ ] pretty-print
[ ] print-input-delimiter
[ ] output-file
[ ] output-wrapper
[ ] output-charset
[ ] fingerprint
[ ] modules
[ ] module-output-path
[ ] module-production-uri
[ ] module-info-path
[ ] global-scope-name
[ ] define
[ ] checks
[ ] treat-warnings-as-errors
[ ] export-test-functions
[ ] name-suffixes-to-strip
[ ] type-prefixes-to-strip
[ ] id-generators
[ ] ambiguate-properties
[ ] disambiguate-properties
[ ] experimental-compiler-options
[ ] custom-passes
[ ] soy-function-plugins
[ ] jsdoc-html-output-path
[ ] variable-map-input-file
[ ] variable-map-output-file
[ ] property-map-input-file
[ ] property-map-output-file

*/