using System;
using System.Web;
using uWebshop.Common.Interfaces;

namespace uWebshop.Domain.Businesslogic
{
	/// <summary>
	/// 
	/// </summary>
	public class HttpContextWrapper : IHttpContextWrapper
	{
		/// <summary>
		/// Gets the absolute path.
		/// </summary>
		/// <value>
		/// The absolute path.
		/// </value>
		public string AbsolutePath
		{
			get { return HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request.Url.AbsolutePath); }
		}

		/// <summary>
		/// Gets the query string.
		/// </summary>
		/// <value>
		/// The query string.
		/// </value>
		public string QueryString
		{
			get
			{
				HttpContext context = HttpContext.Current;
				return context.Request.QueryString.HasKeys() ? "&" + context.Request.QueryString : string.Empty;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the connection is secure.
		/// </summary>
		/// <value>
		///   <c>true</c> if the connection is secure; otherwise, <c>false</c>.
		/// </value>
		public bool IsSecureConnection
		{
			get { return HttpContext.Current.Request.IsSecureConnection; }
		}

		/// <summary>
		/// Gets the URL.
		/// </summary>
		/// <value>
		/// The URL.
		/// </value>
		public string Url
		{
			get { return HttpContext.Current.Request.RawUrl; }
		}

		/// <summary>
		/// Rewrites the path.
		/// </summary>
		/// <param name="newUrl">The new URL.</param>
		public void RewritePath(string newUrl)
		{
			HttpContext.Current.RewritePath(newUrl, true);
		}

		/// <summary>
		/// Redirects permanently.
		/// </summary>
		/// <param name="newUrl">The new URL.</param>
		public void RedirectPermanent(string newUrl)
		{
			HttpContext.Current.Response.RedirectPermanent(newUrl);
		}

		/// <summary>
		/// Path points to physical file?
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns></returns>
		public bool PathPointsToPhysicalFile(string path)
		{
			return System.IO.File.Exists(HttpContext.Current.Server.MapPath(path));
		}
	}
}