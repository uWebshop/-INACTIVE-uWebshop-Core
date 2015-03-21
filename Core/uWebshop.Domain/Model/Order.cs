using System;
using uWebshop.Domain.BaseClasses;
using uWebshop.Domain.ContentTypes;
using uWebshop.Domain.Helpers;

namespace uWebshop.Domain
{
	/// <summary>
	/// 
	/// </summary>
	[ContentType(ParentContentType = typeof(OrderRepositoryContentType), Name = "DateFolder", Description = "#DateFolderDescription", Alias = "uwbsOrderDateFolder", IconClass = IconClass.calendar, Icon = ContentIcon.CalendarMonth, Thumbnail = ContentThumbnail.Folder, AllowedChildTypes = new[] { typeof(DateFolder), typeof(Order) })]
	public class DateFolder
	{
		/// <summary>
		/// The node alias
		/// </summary>
		public static string NodeAlias;
	}

	/// <summary>
	/// 
	/// </summary>
	[ContentType(ParentContentType = typeof(OrderRepositoryContentType), Name = "Order Store Folder", Description = "#OrderStoreFolderDescription", Alias = "uwbsOrderStoreFolder", IconClass = IconClass.store, Icon = ContentIcon.Store, Thumbnail = ContentThumbnail.Folder, AllowedChildTypes = new[] { typeof(DateFolder) })]
	public class OrderStoreFolder
	{
		/// <summary>
		/// The node alias
		/// </summary>
		public static string NodeAlias;
	}

	/// <summary>
	///     Class representing an order in Umbraco
	/// </summary>
	[ContentType(ParentContentType = typeof(OrderRepositoryContentType), Name = "Order", Description = "#OrderDescription", Alias = "uwbsOrder", IconClass = IconClass.clipboard, Icon = ContentIcon.ClipboardInvoice, Thumbnail = ContentThumbnail.Folder, AllowedChildTypes = new[] { typeof(OrderedProduct) })]
	public class Order : DocumentBase
	{
		/// <summary>
		/// The node alias
		/// </summary>
		public static string NodeAlias;

		/// <summary>
		/// The order repository node alias
		/// </summary>
		/// <summary>
		/// The order repository node alias
		/// </summary>
		public static string OrderRepositoryNodeAlias
		{
			get { return OrderRepositoryContentType.NodeAlias; }
		}

		/// <summary>
		/// Gets or sets the order status picker.
		/// </summary>
		/// <value>
		/// The order status picker.
		/// </value>
		[ContentPropertyType(Alias = "orderStatusPicker", DataType = DataType.OrderStatusPicker, Tab = ContentTypeTab.Global, Name = "#OrderStatusPicker", Description = "#OrderStatusPickerDescription", Umbraco6Only = true)]
		public string OrderStatusPicker
		{
			get { return Document.GetProperty<string>("orderStatusPicker"); }
			set { Document.SetProperty("orderStatusPicker", value); }
		}

		/// <summary>
		/// Gets or sets a value indicating whether [order paid].
		/// </summary>
		/// <value>
		///   <c>true</c> if [order paid]; otherwise, <c>false</c>.
		/// </value>
		[ContentPropertyType(Alias = "orderPaid", DataType = DataType.TrueFalse, Tab = ContentTypeTab.Global, Name = "#OrderPaid", Description = "#OrderPaidDescription", Umbraco6Only = true)]
		public bool OrderPaid
		{
			get { return Document.GetProperty<bool>("orderPaid"); }
			set { Document.SetProperty("orderPaid", value); }
		}


		/// <summary>
		/// Gets or sets the order details.
		/// </summary>
		/// <value>
		/// The order details.
		/// </value>
		[ContentPropertyType(Alias = "orderDetails", DataType = DataType.OrderInfoViewer, Tab = ContentTypeTab.Global, Name = "#OrderDetails", Description = "#OrderDetailsDescription")]
		public string OrderDetails
		{
			get { return Document.GetProperty<string>("orderDetails"); }
			set { Document.SetProperty("orderDetails", value); }
		}

		/// <summary>
		/// Gets or sets the unique order unique identifier.
		/// </summary>
		/// <value>
		/// The unique order unique identifier.
		/// </value>
		[ContentPropertyType(Alias = "orderGuid", DataType = DataType.Label, Tab = ContentTypeTab.Global, Name = "#OrderGuid", Description = "#OrderGuidDescription", Umbraco6Only = true)]
		public Guid UniqueOrderId
		{
			get
			{
				Guid uniqueOrderId;
				string value = StoreHelper.GetMultiStoreItem(Id, "orderGuid"); // todo: niet echt multistore, maar moet wel met Node en Document kunnen werken
				Guid.TryParse(value, out uniqueOrderId);
				return uniqueOrderId;
			}
			set { Document.SetProperty("orderGuid", value); }
		}

		/// <summary>
		/// Gets or sets the customer email.
		/// </summary>
		/// <value>
		/// The customer email.
		/// </value>
		[ContentPropertyType(Alias = "customerEmail", DataType = DataType.String, Tab = ContentTypeTab.Customer, Name = "#CustomerEmail", Description = "#CustomerEmailDescription")]
		public string CustomerEmail
		{
			get { return Document.GetProperty<string>("customerEmail"); }
			set { Document.SetProperty("customerEmail", value); }
		}

		/// <summary>
		/// Gets or sets the first name of the customer.
		/// </summary>
		/// <value>
		/// The first name of the customer.
		/// </value>
		[ContentPropertyType(Alias = "customerFirstName", DataType = DataType.String, Tab = ContentTypeTab.Customer, Name = "#CustomerFirstName", Description = "#CustomerFirstNameDescription")]
		public string CustomerFirstName
		{
			get { return Document.GetProperty<string>("customerFirstName"); }
			set { Document.SetProperty("customerFirstName", value); }
		}

		/// <summary>
		/// Gets or sets the last name of the customer.
		/// </summary>
		/// <value>
		/// The last name of the customer.
		/// </value>
		[ContentPropertyType(Alias = "customerLastName", DataType = DataType.String, Tab = ContentTypeTab.Customer, Name = "#CustomerLastName", Description = "#CustomerLastNameDescription")]
		public string CustomerLastName
		{
			get { return Document.GetProperty<string>("customerLastName"); }
			set { Document.SetProperty("customerLastName", value); }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Order"/> class.
		/// </summary>
		public Order()
		{
			//		public static void CreateOrderDocument(OrderInfo orderInfo, int parentDocumentId = 0)
			//AutoSelectShippingProvider();
		}

		/// <summary>
		///     Initializes a new instance of the uWebshop.Domain.Order class
		/// </summary>
		/// <param name="id">NodeId of the order</param>
		public Order(int id) : base(id)
		{
		}
	}
}