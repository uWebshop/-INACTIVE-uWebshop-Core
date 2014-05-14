using System;
using System.Drawing;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;

namespace uWebshop.Umbraco.DataTypes.OrderStatusSection
{
	internal class OrderOverview
	{
		public delegate void OrderSelected(string orderNumber);

		public event OrderSelected OnOrderSelected;

		private GridView _grdOrders;
		private string _statusFilter;
		private int? _numberOfDaysBackFilter; // = 3650;
		private string _sortField = "Orderdate";
		private SortDirection _sortDirection = SortDirection.Descending;

		internal void InitOverviewGrid(UpdatePanel targetContainer)
		{
			_grdOrders = new GridView {CellPadding = 5, AllowSorting = true, CssClass = "uwbsOrderGridView"};

			const string alternatingRowStylehexValue = "#eeeeee";
			_grdOrders.AlternatingRowStyle.BackColor = ColorTranslator.FromHtml(alternatingRowStylehexValue);

			_grdOrders.RowDataBound += GrdOrdersOnRowDataBound;
			_grdOrders.Sorting += GrdOrdersOnSorting;
			_grdOrders.PageIndexChanging += GrdOrdersOnPageIndexChanging;
			_grdOrders.ShowHeader = true;

			BindData();
			targetContainer.ContentTemplateContainer.Controls.Add(_grdOrders); //targetContainer.Controls.Add(_grdOrders);
		}

		internal void BindData()
		{
			if (_statusFilter == null)
				_statusFilter = _grdOrders.Attributes["StatusFilter"];
			if (_numberOfDaysBackFilter == null && _grdOrders.Attributes["NumberOfDaysBackFilter"] != null)
			{
				int numberOfDaysBackFilter;
				if (int.TryParse(_grdOrders.Attributes["NumberOfDaysBackFilter"], out numberOfDaysBackFilter))
					_numberOfDaysBackFilter = numberOfDaysBackFilter;
			}

			var orders = OrderHelper.GetAllOrders().Where(orderinfo => orderinfo != null && orderinfo.ConfirmDate.GetValueOrDefault() >= DateTime.Now.AddDays(-_numberOfDaysBackFilter.GetValueOrDefault(3650))).Select(orderInfo => new OrderData(orderInfo));

			orders = string.IsNullOrEmpty(_statusFilter) ? orders.Where(x => !string.IsNullOrEmpty(x.Email)) : orders.Where(orderData => orderData.Status == _statusFilter && !string.IsNullOrEmpty(orderData.Email));

			//LoadSortSettings();
			if (_sortField == "Ordernumber" && _sortDirection == SortDirection.Ascending)
				_grdOrders.DataSource = orders.OrderBy(order => order.Ordernumber);
			else if (_sortField == "Ordernumber" && _sortDirection == SortDirection.Descending)
				_grdOrders.DataSource = orders.OrderByDescending(order => order.Ordernumber);
			else if (_sortField == "Orderdate" && _sortDirection == SortDirection.Ascending)
				_grdOrders.DataSource = orders.OrderBy(order => order.Orderdate);
			else if (_sortField == "Orderdate" && _sortDirection == SortDirection.Descending)
				_grdOrders.DataSource = orders.OrderByDescending(order => order.Orderdate);
			else if (_sortField == "Store" && _sortDirection == SortDirection.Ascending)
				_grdOrders.DataSource = orders.OrderBy(order => order.Store);
			else if (_sortField == "Store" && _sortDirection == SortDirection.Descending)
				_grdOrders.DataSource = orders.OrderByDescending(order => order.Store);
			else if (_sortField == "Email" && _sortDirection == SortDirection.Ascending)
				_grdOrders.DataSource = orders.OrderBy(order => order.Email);
			else if (_sortField == "Email" && _sortDirection == SortDirection.Descending)
				_grdOrders.DataSource = orders.OrderByDescending(order => order.Email);
			else if (_sortField == "Firstname" && _sortDirection == SortDirection.Ascending)
				_grdOrders.DataSource = orders.OrderBy(order => order.Firstname);
			else if (_sortField == "Firstname" && _sortDirection == SortDirection.Descending)
				_grdOrders.DataSource = orders.OrderByDescending(order => order.Firstname);
			else if (_sortField == "Lastname" && _sortDirection == SortDirection.Ascending)
				_grdOrders.DataSource = orders.OrderBy(order => order.Lastname);
			else if (_sortField == "Lastname" && _sortDirection == SortDirection.Descending)
				_grdOrders.DataSource = orders.OrderByDescending(order => order.Lastname);
			else if (_sortField == "Grandtotal" && _sortDirection == SortDirection.Ascending)
				_grdOrders.DataSource = orders.OrderBy(order => order.ChargedAmount);
			else if (_sortField == "Grandtotal" && _sortDirection == SortDirection.Descending)
				_grdOrders.DataSource = orders.OrderByDescending(order => order.ChargedAmount);
			else if (_sortField == "Status" && _sortDirection == SortDirection.Ascending)
				_grdOrders.DataSource = orders.OrderBy(order => order.Status);
			else if (_sortField == "Status" && _sortDirection == SortDirection.Descending)
				_grdOrders.DataSource = orders.OrderByDescending(order => order.Status);
			else if (_sortField == "Store" && _sortDirection == SortDirection.Ascending)
				_grdOrders.DataSource = orders.OrderBy(order => order.Store);
			else if (_sortField == "Store" && _sortDirection == SortDirection.Descending)
				_grdOrders.DataSource = orders.OrderByDescending(order => order.Store);


			_grdOrders.DataBind();
		}

		internal void SetStatusFilter(string statusFilter)
		{
			_statusFilter = statusFilter;
			if (_grdOrders != null) _grdOrders.Attributes["StatusFilter"] = statusFilter;
		}

		internal void SetNumberOfDaysBackFilter(int numberOfDaysBackFilter)
		{
			_numberOfDaysBackFilter = numberOfDaysBackFilter;
			_grdOrders.Attributes["NumberOfDaysBackFilter"] = numberOfDaysBackFilter.ToString();
		}

		private void GrdOrdersOnPageIndexChanging(object sender, GridViewPageEventArgs e)
		{
			_grdOrders.PageIndex = e.NewPageIndex;
			//_grdOrders.RowDataBound += GrdOrdersOnRowDataBound;
			BindData();
		}

		private void GrdOrdersOnSorting(object sender, GridViewSortEventArgs e)
		{
			GridViewSortDirection(_grdOrders, e, out _sortDirection, out _sortField);
			//_grdOrders.RowDataBound += GrdOrdersOnRowDataBound;
			BindData();
		}

		private void GridViewSortDirection(GridView gridView, GridViewSortEventArgs sortEventArgs, out SortDirection sortDirection, out string sortExpression)
		{
			// found this method on the internet and fixed it
			sortExpression = sortEventArgs.SortExpression;
			sortDirection = sortEventArgs.SortDirection;

			//Check if GridView control has required Attributes
			if (gridView.Attributes["CurrentSortField"] != null && gridView.Attributes["CurrentSortDir"] != null)
			{
				if (sortExpression == gridView.Attributes["CurrentSortField"])
				{
					sortDirection = SortDirection.Descending;
					if (gridView.Attributes["CurrentSortDir"] == "ASC")
					{
						sortDirection = SortDirection.Ascending;
					}
				}
			}
			gridView.Attributes["CurrentSortField"] = sortExpression;
			gridView.Attributes["CurrentSortDir"] = (sortDirection == SortDirection.Ascending ? "DESC" : "ASC");
		}

		private void LoadSortSettings()
		{
			// werkt nog niet
			_sortDirection = _grdOrders.Attributes["CurrentSortDir"] == "ASC" ? SortDirection.Ascending : SortDirection.Descending;
			_sortField = _grdOrders.Attributes["CurrentSortField"];
		}

		private void GrdOrdersOnRowDataBound(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.Header)
			{
				e.Row.Cells[1].Visible = false;
			}
			if (e.Row.RowType == DataControlRowType.DataRow)
			{
				var data = (OrderData) e.Row.DataItem;
				var lnk = new LinkButton {ID = "lnkFolder", CssClass = "lnkFolder", Text = data.Ordernumber, CommandArgument = data.OrderGuid.ToString(), CommandName = "CreateOrder",};
				lnk.Command += LnkOnCommand;

				if (e.Row.Cells[0].FindControl("lnkFolder") == null)
				{
					e.Row.Cells[0].Controls.Add(lnk);
				}
				e.Row.Cells[1].Visible = false;
				e.Row.Cells[4].Wrap = false;
				e.Row.Cells[8].HorizontalAlign = HorizontalAlign.Right;

				//var img = new System.Web.UI.HtmlControls.HtmlImage();
				//img.Src = "/images/file.jpg";
				//e.Row.Cells[0].Controls.AddAt(0, img);
			}
		}

		private void LnkOnCommand(object sender, CommandEventArgs commandEventArgs)
		{
			if (OnOrderSelected != null) OnOrderSelected(commandEventArgs.CommandArgument.ToString());
		}
	}

	public class OrderData
	{
		public OrderData()
		{
		}

		public OrderData(OrderInfo orderInfo)
		{
			if (orderInfo == null) return;
			OrderGuid = orderInfo.UniqueOrderId;
			if (orderInfo.OrderNumber != null) Ordernumber = orderInfo.OrderNumber;
			Status = orderInfo.Status.ToString();
			Store = orderInfo.StoreInfo.Alias;
			//if (orderInfo.OrderDate != null)
			//{
			//	DateTime val;
			//	if (!DateTime.TryParse(orderInfo.OrderDate, orderInfo.StoreInfo.CultureInfo.DateTimeFormat, DateTimeStyles.None, out val))
			//		if (!DateTime.TryParse(orderInfo.OrderDate, new CultureInfo("EN-us").DateTimeFormat, DateTimeStyles.None, out val))
			//			if (!DateTime.TryParse(orderInfo.OrderDate, new CultureInfo("NL-nl").DateTimeFormat, DateTimeStyles.None, out val))
			//				DateTime.TryParse(orderInfo.OrderDate, out val);
			//	Orderdate = val;
			//}
			if (orderInfo.ConfirmDate.HasValue) Orderdate = orderInfo.ConfirmDate.GetValueOrDefault();
			if (orderInfo.CustomerEmail != null) Email = orderInfo.CustomerEmail;
			if (orderInfo.CustomerFirstName != null) Firstname = orderInfo.CustomerFirstName;
			if (orderInfo.CustomerLastName != null) Lastname = orderInfo.CustomerLastName;

			ChargedAmount = orderInfo.ChargedAmount.ToString("C", orderInfo.StoreInfo.CurrencyCultureInfo);
		}

		//ordernummer orderdatum, email, firstname, lastname, subtotal grandtotal ?
		public string Ordernumber { get; set; }
		public Guid OrderGuid { get; set; }
		public string Status { get; set; }
		public string Store { get; set; }
		public DateTime? Orderdate { get; set; }
		public string Email { get; set; }
		public string Firstname { get; set; }
		public string Lastname { get; set; }
		public string ChargedAmount { get; set; }
		//public string RangePrice { get; set; }
	}
}