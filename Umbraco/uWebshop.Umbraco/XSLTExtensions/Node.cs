using Umbraco.Core.Macros;
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
				return umbHelper.TypedContent(nodeId).GetMultiStoreItem(alias).Value.ToString();
			}
			catch
			{
				return string.Empty;
			}
		}
	}
}