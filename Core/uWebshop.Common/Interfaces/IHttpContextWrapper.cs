namespace uWebshop.Common.Interfaces
{
	public interface IHttpContextWrapper
	{
		string AbsolutePath { get; }
		string QueryString { get; }
		bool IsSecureConnection { get; }
		string Url { get; }
		void RewritePath(string newUrl);
		void RedirectPermanent(string newUrl);
		bool PathPointsToPhysicalFile(string path);
	}
}