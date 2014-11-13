using System;
using uWebshop.Common;
using uWebshop.Domain.BaseClasses;
using uWebshop.Domain.Helpers;

namespace uWebshop.Domain.ContentTypes
{
	/// <summary>
	/// 
	/// </summary>
	[ContentType(ParentContentType = typeof(UwebshopRootContentType), Name = "Order Repository", Description = "#OrderRepositoryDescription", Alias = "uwbsOrderRepository", IconClass = IconClass.archive, Icon = ContentIcon.Drawer, Thumbnail = ContentThumbnail.Folder)]
	public class OrderRepositoryContentType : DocumentBase
	{
		internal static string NodeAlias;

        /// <summary>
		/// Initializes a new instance of the <see cref="OrderSection"/> class.
		/// </summary>
		/// <param name="id">NodeId of the document</param>
        public OrderRepositoryContentType(int id)
            : base(id)
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