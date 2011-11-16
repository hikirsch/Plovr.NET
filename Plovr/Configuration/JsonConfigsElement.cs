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
using System.Configuration;

namespace Plovr.Configuration
{
	internal class JsonConfigsElement : ConfigurationElementCollection
	{
		/// <summary>
		/// Create a new PlovrProjectElement.
		/// </summary>
		/// <returns></returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new JsonConfigElement();
		}

		/// <summary>
		/// We use the Id as the key in the web.config.
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		protected override object GetElementKey(ConfigurationElement element)
		{
			var plovrProjectElement = element as JsonConfigElement;
		
			if (plovrProjectElement == null)
			{
				throw new NullReferenceException("The JsonConfigsElement node element is null. Weird setup in web.config?");		
			}

			return plovrProjectElement.Path;
		}
	}
}				