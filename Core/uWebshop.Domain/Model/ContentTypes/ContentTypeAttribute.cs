using System;

namespace uWebshop.Domain.ContentTypes
{
#pragma warning disable 1591
	/// <summary>
	/// 
	/// </summary>
	public class ContentTypeAttribute : Attribute
	{
		public string Alias;
		public Type[] AllowedChildTypes;
		public string Description;
		public ContentIcon Icon;
		public IconClass IconClass;
		public string Name;
		public Type ParentContentType;
		public ContentThumbnail Thumbnail;
		public bool InstallerOnly;
		public int SortOrder;
	}
}