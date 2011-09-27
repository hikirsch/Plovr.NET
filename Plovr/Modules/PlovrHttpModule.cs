using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using Plovr.Builders;
using Plovr.Configuration;
using Plovr.Helpers;
using Plovr.Model;

namespace Plovr.Modules
{
	// http://code.google.com/p/plovr/source/browse/src/org/plovr/AbstractGetHandler.java
	class PlovrHttpModule : IHttpModule
	{
		/// <summary>
		/// Register this module so that it listens on every Http Request.
		/// </summary>
		/// <param name="context"></param>
		public void Init(HttpApplication context)
		{
			context.BeginRequest += BeginRequest;
		}

		/// <summary>
		/// An event listener, this will fire whenever a request has started. If we contain /plovr in the URL, 
		/// then we handle the entire request.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		public void BeginRequest(object sender, EventArgs eventArgs)
		{
			HttpContext context = HttpContext.Current;
			string url = context.Request.RawUrl.ToLower();

			// make sure this is a plovr request; otherwise, pass it along
			if (!url.StartsWith("/plovr.net")) return;

			// Pass extra url params in to the appropriate handler
			// TODO: support all command_names from Plovr, @see http://code.google.com/p/plovr/source/browse/src/org/plovr/Handler.java 

			string pattern = @"/[^/]+"; // matches '/path1', '/path2', '/path3' in '/path1/path2/path3/'
			MatchCollection matches = Regex.Matches(url, pattern, RegexOptions.IgnoreCase);

			// instantiate handler from query string
			Handler handler;
			if (matches.Count > 1) { // if we have specified additional params in the url (e.g. plovr.net/compile or plovr.net/index)
				try {
					// convert url param to handler type string
					MatchCollection typeMatches = Regex.Matches(matches[1].ToString(), @"[^/][^?]+", RegexOptions.IgnoreCase); // matches "compile" in "/compile?id=someId"
					string typeStr = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(typeMatches[0].ToString().ToLower()); // convert lowercase string to titlecase

					handler = Handler.CreateInstance("Plovr.Modules." + typeStr + "Handler", context); // TODO: Protect against XSS
				}
				catch (Exception) {
					handler = Handler.CreateInstance("Plovr.Modules.IndexHandler", context);
				}

				/*
				switch (matches[1].ToString()) {
					case "/config":
						ConfigHandler(context);
						break;
					case "/compile":
						CompileHandler(context);
						break;
					case "/externs":
						ExternsHandler(context);
						break;
					case "/input":
						InputHandler(context);
						break;
					case "/list":
						ListHandler(context);
						break;
					case "/module":
						ModuleHandler(context);
						break;
					case "/modules":
						ModulesHandler(context);
						break;
					case "/size":
						SizeHandler(context);
						break;
					case "/sourcemap":
						SourcemapHandler(context);
						break;
					case "/view":
						ViewHandler(context);
						break;
					default:
						// no matching handler, render index
						IndexHandler(context);
						break;
				}
				 * */
			} else { // else we have only specified the /plovr.net root
				handler = Handler.CreateInstance("Plovr.Modules.IndexHandler", context);
			}

			// run the handler
			handler.Run();
		}

		void context_BeginRequest(object sender, System.EventArgs e)
		{
			var context = HttpContext.Current;
			if (context.Request.RawUrl.StartsWith("/plovr/"))
			{
				context.Response.ContentType = "text/html";
				context.Response.Write("I'm Here");
				context.Response.End();
			}
		}

		public void Dispose() { }
	}
}
