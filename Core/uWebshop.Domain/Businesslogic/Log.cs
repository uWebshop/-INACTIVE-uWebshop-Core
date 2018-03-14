using uWebshop.Common.Interfaces;

namespace uWebshop.Domain
{
	/// <summary>
	/// 
	/// </summary>
	public class Log
	{
		private static ILoggingService _instance;

		/// <summary>
		/// Gets the current logger.
		/// </summary>
		/// <value>
		/// The logger.
		/// </value>
		public static ILoggingService Instance
		{
			get {  return _instance ?? (_instance = IO.Container.Resolve<ILoggingService>()); }
		}
	}
}
