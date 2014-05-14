using System;
using System.IO;
using System.Net;
using System.Text;

namespace uWebshop.Common
{
	/// <summary>
	/// 
	/// </summary>
	public class HttpRequestSender : IHttpRequestSender
	{
		// 20140509MS is this class used in external code?

		/// <summary>
		/// Send data to an URL using HTTP POST.
		/// </summary>
		/// <param name="url">Url to send to</param>
		/// <param name="postData">The data to send</param>
		/// <returns></returns>
		public string SendRequest(string url, string postData)
		{
			var uri = new Uri(url);
			var request = WebRequest.Create(uri);
			var encoding = new UTF8Encoding();
			var requestData = encoding.GetBytes(postData);

			request.ContentType = "application/x-www-form-urlencoded";

			request.Method = "POST";
			request.Timeout = (300*1000); //TODO: Move timeout to config
			request.ContentLength = requestData.Length;

			using (var stream = request.GetRequestStream())
			{
				stream.Write(requestData, 0, requestData.Length);
			}

			return ReadToEnd(url, request);
		}

		private static string ReadToEnd(string url, WebRequest request)
		{
			using (var response = request.GetResponse())
			{
				var stream = response.GetResponseStream();
				if (stream == null) throw new Exception("No response from POST request to " + url);
				using (var reader = new StreamReader(stream, Encoding.ASCII))
				{
					return reader.ReadToEnd();
				}
			}
		}

		/// <summary>
		/// Send data to an URL using HTTP GET.
		/// </summary>
		/// <param name="url">Url to send to</param>
		/// <param name="postData">The data to send</param>
		/// <param name="headers">Additional Headers</param>
		/// <returns></returns>
		public string GetRequest(string url, string postData, WebHeaderCollection headers)
		{
			var uri = new Uri(url);
			var request = WebRequest.Create(uri + "?" + postData);

			if (headers.Count > 0)
				request.Headers = headers;

			request.Method = "GET";
			request.Timeout = (300*1000); //TODO: Move timeout to config
			request.ContentLength = 0;

			return ReadToEnd(url, request);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public interface IHttpRequestSender
	{
		/// <summary>
		/// Send data to an URL using HTTP POST.
		/// </summary>
		/// <param name="url">Url to send to</param>
		/// <param name="postData">The data to send</param>
		string SendRequest(string url, string postData);

		/// <summary>
		/// Send data to an URL using HTTP GET.
		/// </summary>
		/// <param name="url">Url to send to</param>
		/// <param name="postData">The data to send</param>
		/// <param name="headers">Additional Headers</param>
		string GetRequest(string url, string postData, WebHeaderCollection headers);
	}
}