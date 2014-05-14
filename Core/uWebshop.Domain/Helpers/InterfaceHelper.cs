using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace uWebshop.Domain.Helpers
{
	/// <summary>
	/// 
	/// </summary>
	public static class InterfaceHelper
	{
		/// <summary>
		/// Gets the interfaces.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static List<T> GetInterfaces<T>()
		{
			var targetType = typeof (T);

			try
			{
				var path = HttpContext.Current.Server.MapPath("/bin");

				return Directory.GetFiles(path).Select(filepath => new FileInfo(filepath)).Where(fileInfo => fileInfo.Name.StartsWith("uWebshop.")).Select(fileInfo => Assembly.LoadFrom(fileInfo.FullName)).Where(assembly => assembly != null).SelectMany(assembly => assembly.GetExportedTypes()).Where(type => targetType.IsAssignableFrom(type)).Select(type => (T) Activator.CreateInstance(type)).Where(obj => obj != null).ToList();
			}
			catch
			{
				Log.Instance.LogDebug(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + " failure with loading payment or shipping extension " + typeof (T).Name);
			}

			return new List<T>();
		}
	}
}