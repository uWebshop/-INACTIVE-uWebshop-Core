using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco;
using umbraco.interfaces;
using uWebshop.Common;

namespace uWebshop.Umbraco.DataTypes.OrderStatusDropdown
{
	public class OrderStatusDrowpDownDataEditor : UpdatePanel, IDataEditor
	{
		private readonly IData _data;

		private DropDownList _dlOrderStatus;

		public OrderStatusDrowpDownDataEditor(IData data)
		{
			_data = data;
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

			_dlOrderStatus = new DropDownList();

			foreach (OrderStatus orderstatus in Enum.GetValues(typeof (OrderStatus)))
			{
				var orderstatusText = library.GetDictionaryItem("Status" + orderstatus);
				if (string.IsNullOrEmpty(orderstatusText))
				{
					orderstatusText = orderstatus.ToString();
				}
				if (orderstatus != OrderStatus.Incomplete)
				{
					_dlOrderStatus.Items.Add(new ListItem(orderstatusText, orderstatus.ToString()));
				}
			}

			_dlOrderStatus.SelectedValue = _data.Value.ToString();
			

			if (ContentTemplateContainer != null) ContentTemplateContainer.Controls.Add(_dlOrderStatus);
		}
	}
}