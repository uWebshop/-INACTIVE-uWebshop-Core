using uWebshop.Common.Interfaces;

namespace uWebshop.Test.Mocks
{
	public class MockHttpContextWrapper : IHttpContextWrapper
	{
		public string AbsolutePath { get; set; }
		public string QueryString { get; set; }
		public bool IsSecureConnection { get; set; }

		public string Url
		{
			get { return string.Empty; }
		}

		public string RewritePathCalledValue { get; set; }

		public void RewritePath(string newUrl)
		{
			RewritePathCalledValue = newUrl;
		}

		public string RedirectPermanentCalledValue { get; set; }

		public void RedirectPermanent(string newUrl)
		{
			RedirectPermanentCalledValue = newUrl;
		}

		public int PathPointsToPhysicalFileTimesCalled = 0;
		public bool PathPointsToPhysicalFileValue { get; set; }

		public bool PathPointsToPhysicalFile(string path)
		{
			PathPointsToPhysicalFileTimesCalled++;
			return PathPointsToPhysicalFileValue;
		}

		public MockHttpContextWrapper()
		{
		}

		public MockHttpContextWrapper(string url)
		{
			Configure(url);
		}

		public void Configure(string url)
		{
			if (!url.Contains("?"))
			{
				AbsolutePath = url;
				QueryString = "";
				return;
			}
			var urlParts = url.Split('?');
			AbsolutePath = urlParts[0];
			QueryString = "&" + urlParts[1];
		}
	}
}