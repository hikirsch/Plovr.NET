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
using Plovr.Helpers;
using Plovr.Model;

namespace Plovr.Runners
{
	public abstract class BaseRunner
	{
		protected AsyncProcessHelper ProcessHelper { get; set; }
		protected IPlovrProject Project { get; set; }
		protected IPlovrSettings Settings { get; set; }

		protected BaseRunner(IPlovrSettings settings, IPlovrProject project)
		{
			this.Settings = settings;
			this.Project = project;

			ProcessHelper = new AsyncProcessHelper();
		}
	}
}
