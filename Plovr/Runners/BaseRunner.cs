using Plovr.Helpers;
using Plovr.Model;

namespace Plovr.Runners
{
	public abstract class BaseRunner
	{
		protected AsyncProcessHelper ProcessHelper { get; set; }
		protected IPlovrProject Project { get; set; }
		protected IPlovrSettings Settings { get; set; }

		public BaseRunner(IPlovrSettings settings, IPlovrProject project)
		{
			this.Settings = settings;
			this.Project = project;

			ProcessHelper = new AsyncProcessHelper();
		}
	}
}
