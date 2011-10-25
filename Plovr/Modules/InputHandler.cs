using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using Plovr.Builders;
using Plovr.Configuration;
using Plovr.Helpers;
using Plovr.Model;

namespace Plovr.Modules
{
	class InputHandler : Handler
	{
		public InputHandler(HttpContext context) : base(context) { }

		public override void Run() {

		}

		/// <summary>
		/// Override to get ID from the Uri path instead of from the query string
		/// </summary>
		protected new string GetIdFromUri() {
			return context.Request.Path;

			//return null;
		}
	}
}
