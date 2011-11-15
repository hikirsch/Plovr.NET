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
	internal class PlovrProjectsElement : ConfigurationElementCollection
	{
		/// <summary>
		/// Gets the config file path.
		/// </summary>
		/// <value>The config file path.</value>
		[ConfigurationProperty("defaultProjectName", IsRequired = true)]
		public string DefaultProjectName
		{
			get { return (string)base["defaultProjectName"]; }
		}

		/// <summary>
		/// Get the Default Project Element
		/// </summary>
		public PlovrProjectElement DefaultProjectElement
		{
			get { return BaseGet(DefaultProjectName) as PlovrProjectElement; }
		}

		/// <summary>
		/// Get a single ProjectElement by its id.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public PlovrProjectElement GetProjectById(object id)
		{
			var project = BaseGet(id);

			if( project != null)
			{
				return project as PlovrProjectElement;
			}

			return null;
		}

		/// <summary>
		/// Create a new PlovrProjectElement.
		/// </summary>
		/// <returns></returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new PlovrProjectElement();
		}

		/// <summary>
		/// We use the Id as the key in the web.config.
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		protected override object GetElementKey(ConfigurationElement element)
		{
			var plovrProjectElement = element as PlovrProjectElement;
		
			if (plovrProjectElement == null)
			{
				throw new NullReferenceException("The PlovrProjectsElement node element is null. Weird setup in web.config?");		
			}

			return plovrProjectElement.Path;
		}
	}
}				