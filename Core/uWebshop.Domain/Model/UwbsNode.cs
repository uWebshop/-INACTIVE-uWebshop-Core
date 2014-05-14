using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain.Helpers
{
#pragma warning disable 1591
	public class UwbsNode // voor caching
	{
		public int Id;
		public int Level;
		public string NodeTypeAlias;
		public int ParentId;
		public string Path;
		public int SortOrder;
		public string UrlName;

		private UwbsNode _parent;

		public UwbsNode Parent
		{
			get { return _parent ?? (_parent = IO.Container.Resolve<ICMSEntityRepository>().GetByGlobalId(ParentId)); }
		}
	}
}