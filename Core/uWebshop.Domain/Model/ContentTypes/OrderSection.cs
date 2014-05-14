using System;
using uWebshop.Common;
using uWebshop.Domain.BaseClasses;
using uWebshop.Domain.ContentTypes;
using uWebshop.Domain.Helpers;

namespace uWebshop.Domain
{
	/// <summary>
	/// 
	/// </summary>
	[ContentType(ParentContentType = typeof (OrderRepositoryContentType), Name = "Order Section", Description = "#OrderSectionDescription", Alias = "uwbsOrderSection", IconClass = IconClass.statistics, Icon = ContentIcon.FolderOpenTable, Thumbnail = ContentThumbnail.Folder)]
	public class OrderSection : DocumentBase
	{
		/// <summary>
		/// The node alias
		/// </summary>
		public static string NodeAlias;

		/// <summary>
		/// Initializes a new instance of the <see cref="OrderSection"/> class.
		/// </summary>
		/// <param name="id">NodeId of the document</param>
		public OrderSection(int id) : base(id)
		{
		}

		/// <summary>
		///     Which kind of orders should be listed in this section
		/// </summary>
		[ContentPropertyType(Alias = "orderSection", DataType = DataType.OrderSection, Tab = ContentTypeTab.Global, Name = "#OrderSection", Description = "#OrderSectionDescription")]
		public OrderStatus OrderStatusInSection
		{
			get
			{
				string property = StoreHelper.GetMultiStoreItem(Id, "orderSection");
				return !string.IsNullOrEmpty(property) ? (OrderStatus) Enum.Parse(typeof (OrderStatus), property) : OrderStatus.Incomplete;
			}
			set { }
		}
	}
}