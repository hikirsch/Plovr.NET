﻿// Copyright 2011 Ogilvy & Mather. All Rights Reserved.
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
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Plovr.Configuration;
using Plovr.Helpers;
using Plovr.Model;

namespace Plovr.Modules
{
	internal abstract class Handler
	{
		#region "Properties"

		/// <summary>
		/// The current Http Context.
		/// </summary>
		protected HttpContext Context { get; set; }

		/// <summary>
		/// the id key in the query string
		/// </summary>
		private const string IdQueryStringParam = "id";

		/// <summary>
		/// the mode key in the query string
		/// </summary>
		private const string ModeQueryStringParam = "mode";

		/// <summary>
		/// the current project settings that are loaded
		/// </summary>
		protected PlovrProject CurrentProject;

		/// <summary>
		/// the current settings for plovr
		/// </summary>
		protected PlovrSettings CurrentSettings;

		/// <summary>
		/// An abstract method, every handler will implement this function and will be called after alll
		/// common pre-processing has occured.
		/// </summary>
		public abstract void Run();

		#endregion

		#region "Static Methods"

		/// <summary>
		/// Create a new instance of the specific route that is, this is a static method.
		/// </summary>
		/// <param name="typeStr">a string representation of the current route</param>
		/// <param name="context">the http context</param>
		/// <returns></returns>
		public static Handler CreateInstance(string typeStr, HttpContext context)
		{
			Type handlerType = Type.GetType(typeStr);

			if (handlerType != null)
			{
				return (Handler) Activator.CreateInstance(handlerType, new object[] {context});
			}

			throw new Exception("The route '" + typeStr + "' was unable to be mapped");
		}

		#endregion

		#region "Common Plovr Functions"

		/// <summary>
		/// Constructor, starts by initalizing the current project and settings properties.
		/// </summary>
		/// <param name="context"></param>
		protected Handler(HttpContext context) 
		{
			this.Context = context;

			// get the project configuration and application settings
			this.InitActiveProjectAndSettings();
		}

		/// <summary>
		/// Using the HttpContext as the current website that is loaded, load the current project and settings.
		/// </summary>
		private void InitActiveProjectAndSettings()
		{
			// get the id from the QueryString
			var id = GetIdFromUri();

			PlovrConfiguration.GetCurrentPlovrSettingsAndProjectById(id, out this.CurrentSettings, out this.CurrentProject);

			// support %JAVA_HOME% env variable
			this.CurrentSettings.JavaPath = PathHelpers.ResolveJavaPath(CurrentSettings.JavaPath);

			// override the mode from the querystring if its passed
			this.CurrentProject.Mode = this.GetModeFromQueryString() ?? CurrentProject.Mode;

			// reformat the base paths so we have full paths
			this.CurrentProject.BasePaths = CurrentProject.BasePaths.Select(Context.Server.MapPath);

			if (this.CurrentProject.Externs != null)
			{
				this.CurrentProject.Externs = CurrentProject.Externs.Select(Context.Server.MapPath);
			}
		}

		#endregion

		#region "Response Helpers"

		/// <summary>
		/// Show the response with the specified content type. The request will then end.
		/// </summary>
		/// <param name="response">the output</param>
		/// <param name="contentType">the content type</param>
		protected void ShowResponse(string response, string contentType)
		{
			Context.Response.ContentType = contentType;
			Context.Response.Write(response);
			Context.Response.End();			
		}

		/// <summary>
		/// Show the response with a javascript content type.
		/// </summary>
		/// <param name="response">the output</param>
		protected void ShowJavaScriptResponse(string response)
		{
			this.ShowResponse(response, "application/x-javascript");
		}

		/// <summary>
		/// Show the resposne with the html content type.
		/// </summary>
		/// <param name="response"></param>
		protected void ShowHtmlResponse(string response)
		{
			this.ShowResponse(response, "text/html");
		}

		/// <summary>
		/// Show a file in the response and set the content type as javascript.
		/// </summary>
		/// <param name="file">the file to output</param>
		protected void ShowJavaScriptFileResponse(string file)
		{
			this.Context.Response.ContentType = "application/x-javascript";
			this.Context.Response.WriteFile(file);
			this.Context.Response.End();
		}

		#endregion

		#region "QueryString Helpers"

		/// <summary>
		/// The ID can be passed from the QueryString to override the DefaultProject flag.
		/// </summary>
		/// <remarks>Override in subclasses if you need to get it from path instead of QueryString</remarks>
		protected virtual string GetIdFromUri()
		{
			NameValueCollection queryString = Context.Request.QueryString;

			if (queryString.AllKeys.Contains(IdQueryStringParam))
			{
				return queryString[IdQueryStringParam];
			}

			return null;
		}


		/// <summary>
		/// Retrieves a mode override from the querystring, if it exists.
		/// </summary>
		/// <returns>the mode from the querystring as an enum if it exists</returns>
		private ClosureCompilerMode? GetModeFromQueryString()
		{
			NameValueCollection queryString = Context.Request.QueryString;

			if (queryString.AllKeys.Contains(IdQueryStringParam))
			{
				return Mappers.MapToEnum<ClosureCompilerMode>(queryString[ModeQueryStringParam]);
			}

			return null;
		}

		#endregion

		#region "Utilities"
		/// <summary>
		/// Convert closure compiler messages to JSON.
		/// </summary>
		/// <param name="output">the message output from the closure compiler</param>
		/// <returns></returns>
		protected string ToJson(List<ClosureCompilerMessage> output)
		{
			return JsonConvert.SerializeObject(output);
		}

		/// <summary>
		/// return a root url of the plovr handler, e.g. http://localhost:9810/Plovr.NET
		/// </summary>
		/// <returns>a url friendly absolute URL to the site-root</returns>
		protected string GetRootUrl()
		{
			StringBuilder builder = new StringBuilder();
			builder.Append(Context.Request.Url.Scheme);
			builder.Append("://");
			builder.Append(Context.Request.Url.Host);

			if (Context.Request.Url.Port != 80)
			{
				builder.Append(":");
				builder.Append(Context.Request.Url.Port);
			}

			builder.Append(PlovrHttpModule.PlovrUrlHandler);

			return builder.ToString();
		}

		/// <summary>
		/// Windows file system slashes are \, URL's are in /, this makes things a little easier for us.
		/// </summary>
		/// <param name="str">the string we want to switch the slashes in</param>
		/// <returns>the slashes swapped</returns>
		protected string FixSlash(string str)
		{
			return str.Replace("\\", "/");
		}

		#endregion
	}
}