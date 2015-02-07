using System;

namespace uWebshop.Domain.ContentTypes
{
#pragma warning disable 1591
	/// <summary>
	/// 
	/// </summary>
	public class ContentPropertyTypeAttribute : Attribute
	{
		public string Alias;
		public DataType DataType;
		public string Description;
		public bool Mandatory;
		public string Name;
		public int SortOrder;
		public ContentTypeTab Tab;
		public bool Umbraco6Only;
	}
}