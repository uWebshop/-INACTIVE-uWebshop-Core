using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco;
using umbraco.interfaces;
using uWebshop.Common;
using uWebshop.DataAccess;
using uWebshop.Domain;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Umbraco.DataTypes.OrderStatusSection
{
	public class OrderStatusSectionDataEditor : UpdatePanel, IDataEditor
	{
		private readonly IData _data;

		private readonly OrderOverview _orderOverview;
		private readonly int _containingDocumentId;

		private DropDownList _dlOrderStatus;
		private DropDownList _dlTimeRange;
		private Button _btnOrderStatus;

		public OrderStatusSectionDataEditor(IData data)
		{
			_data = data;
			_orderOverview = new OrderOverview();
			_orderOverview.OnOrderSelected += orderGuid =>
				{
					try
					{
						Guid guid;
						Guid.TryParse(orderGuid, out guid);
						if (guid == Guid.Empty)
						{
							Log.Instance.LogError("Trying to open an order with a broken guid: " + orderGuid);
							return;
						}
						API.CMS.CreateOrderDocument(guid);
					}
					catch (Exception ex)
					{
						Log.Instance.LogError(ex, "When opening order");
					}
				};
			if (int.TryParse(HttpContext.Current.Request["id"], out _containingDocumentId))
			{
				// !Page.IsPostBack && werkt niet
				//var orderSection = string.Empty;
				//var property = new Document(_containingDocumentId).getProperty("orderSection");
				//if (property != null)
				//	orderSection = property.Value.ToString();
				//_orderOverview.SetStatusFilter(orderSection);  dit kan niet, geeft raar effect na wisselen
			}
		}

		public void Save()
		{
			if (_data != null) _data.Value = _dlOrderStatus.SelectedValue;
		}

		public bool ShowLabel
		{
			get { return true; }
		}

		public bool TreatAsRichTextEditor
		{
			get { return false; }
		}

		public Control Editor
		{
			get { return this; }
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			if (!(Page.Request.CurrentExecutionFilePath ?? string.Empty).Contains("editContent.aspx"))
				return;
			
			_dlOrderStatus = new DropDownList {ID = "ddlOrderStatus"};
			_dlOrderStatus.SelectedIndexChanged += DlOrderStatusOnSelectedIndexChanged;
			_dlOrderStatus.AutoPostBack = false;

			var setText = library.GetDictionaryItem("Submit");

			if (string.IsNullOrEmpty(setText))
			{
				setText = "Submit";
			}

			_btnOrderStatus = new Button {ID = "btnOrderStatus", Text = setText};
			_btnOrderStatus.Click += BtnOrderStatusClick;

			var showAll = library.GetDictionaryItem("ShowAll");

			if (string.IsNullOrEmpty(showAll))
			{
				showAll = "Show All Orders";
			}

			_dlOrderStatus.Items.Add(new ListItem(showAll, string.Empty));

			foreach (OrderStatus orderstatus in Enum.GetValues(typeof (OrderStatus)))
			{
				var orderstatusText = library.GetDictionaryItem("Status" + orderstatus);
				if (string.IsNullOrEmpty(orderstatusText))
				{
					orderstatusText = orderstatus.ToString();
				}

				_dlOrderStatus.Items.Add(new ListItem(orderstatusText, orderstatus.ToString()));
			}

			_dlTimeRange = new DropDownList {ID = "ddlDateTimeRange"};
			_dlTimeRange.SelectedIndexChanged += DlTimeRangeSelectedIndexChanged;
			_dlTimeRange.AutoPostBack = false;

			_dlTimeRange.Items.Add(new ListItem("All", "99999"));
			_dlTimeRange.Items.Add(new ListItem("Month", "31"));
			_dlTimeRange.Items.Add(new ListItem("14 Days", "14"));
			_dlTimeRange.Items.Add(new ListItem("7 Days", "7"));
			_dlTimeRange.Items.Add(new ListItem("Today", "1"));

			var updatePanel = new UpdatePanel {ID = "uwbsOrdersUpdatePanel", UpdateMode = UpdatePanelUpdateMode.Always};
			updatePanel.ContentTemplateContainer.Controls.Add(_dlOrderStatus);
			updatePanel.ContentTemplateContainer.Controls.Add(_dlTimeRange);
			updatePanel.ContentTemplateContainer.Controls.Add(_btnOrderStatus);

			_orderOverview.InitOverviewGrid(updatePanel);
			var selectedStatus = _data.Value.ToString();
			_dlOrderStatus.SelectedValue = selectedStatus;
			_orderOverview.SetStatusFilter(selectedStatus);
			_orderOverview.BindData();

			var triggerStatus = new AsyncPostBackTrigger {ControlID = _dlOrderStatus.ID};
			var triggerStatusButton = new AsyncPostBackTrigger {ControlID = _btnOrderStatus.ID};
			var triggerDateTime = new AsyncPostBackTrigger {ControlID = _dlTimeRange.ID};

			updatePanel.Triggers.Add(triggerStatus);
			updatePanel.Triggers.Add(triggerDateTime);
			updatePanel.Triggers.Add(triggerStatusButton);

			var updateProgress = new UpdateProgress {ID = "uwbsOrdersUpdateProgress", AssociatedUpdatePanelID = "uwbsOrdersUpdatePanel", DynamicLayout = true, ProgressTemplate = new ProgressTemplate()};

			var loadingOverlay = new Literal {Text = "<div style='height: 100%; width: 100%; position: fixed; left: 0px; top: 0px; z-index: 9999;' class='jqmOverlay'>&nbsp;</div>"};

			updateProgress.Controls.Add(loadingOverlay);

			if (ContentTemplateContainer != null) ContentTemplateContainer.Controls.Add(updateProgress);
			if (ContentTemplateContainer != null) ContentTemplateContainer.Controls.Add(updatePanel);
		}

		private void DlTimeRangeSelectedIndexChanged(object sender, EventArgs e)
		{
			var thisDdl = (DropDownList) sender;
			var ddlOrderStatus = (DropDownList) thisDdl.Parent.FindControl("ddlOrderStatus");
			_orderOverview.SetStatusFilter(ddlOrderStatus.SelectedValue);

			int days;
			int.TryParse(thisDdl.SelectedValue, out days);
			_orderOverview.SetNumberOfDaysBackFilter(days);

			_orderOverview.BindData();
		}

		private void BtnOrderStatusClick(object sender, EventArgs e)
		{
			var thisButton = (Button) sender;

			var ddlDateTime = (DropDownList) thisButton.Parent.FindControl("ddlDateTimeRange");
			var ddlOrderStatus = (DropDownList) thisButton.Parent.FindControl("ddlOrderStatus");

			int days;
			int.TryParse(ddlDateTime.SelectedValue, out days);

			_orderOverview.SetStatusFilter(ddlOrderStatus.SelectedValue);
			_orderOverview.SetNumberOfDaysBackFilter(days);
			_orderOverview.BindData();

			Save();
		}

		private void DlOrderStatusOnSelectedIndexChanged(object sender, EventArgs eventArgs)
		{
			var thisDdl = (DropDownList) sender;
			var ddlDateTime = (DropDownList) thisDdl.Parent.FindControl("ddlDateTimeRange");

			int days;
			int.TryParse(ddlDateTime.SelectedValue, out days);

			_orderOverview.SetStatusFilter(((DropDownList) sender).SelectedValue);
			_orderOverview.SetNumberOfDaysBackFilter(days);
			_orderOverview.BindData();
		}

		private class ProgressTemplate : ITemplate
		{
			#region ITemplate Members

			public void InstantiateIn(Control container)
			{
			}

			#endregion
		}
	}
}