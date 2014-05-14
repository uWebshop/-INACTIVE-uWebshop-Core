using uWebshop.Domain.Businesslogic;
using umbraco;
using uWebshop.Umbraco;

namespace uWebshop.XSLTExtensions
{
	[XsltExtension("uWebshop.Node")]
	public class Node
	{
		public static string GetMultilangualItem(int nodeId, string alias)
		{
			try
			{
				return new umbraco.NodeFactory.Node(nodeId).GetMultiStoreItem(alias).Value;
			}
			catch
			{
				return string.Empty;
			}
		}
	}
}