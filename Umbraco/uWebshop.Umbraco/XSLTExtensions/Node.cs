using uWebshop.Domain.Businesslogic;
using umbraco;
using uWebshop.Umbraco;
using Umbraco.Web;

namespace uWebshop.XSLTExtensions
{
	[XsltExtension("uWebshop.Node")]
	public class Node
	{
		public static string GetMultilangualItem(int nodeId, string alias)
		{
			try
			{
				var umbHelper = new UmbracoHelper(UmbracoContext.Current);
				return umbHelper.Content(nodeId).GetMultiStoreItem(alias).Value;
			}
			catch
			{
				return string.Empty;
			}
		}
	}
}