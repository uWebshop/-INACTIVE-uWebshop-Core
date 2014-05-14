using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using umbraco;
using umbraco.cms.businesslogic.web;
using umbraco.interfaces;
using uWebshop.Common;
using uWebshop.Domain.Helpers;

namespace uWebshop.Umbraco.DataTypes.OrderStatusPicker
{
	public class OrderStatusPickerDataEditor : UpdatePanel, IDataEditor
	{
		private readonly IData _data;

		private DropDownList _dlOrderStatus;
		private CheckBox _cbSentEmail;
		private Label _lblSentEmail;

		public OrderStatusPickerDataEditor(IData data)
		{
			_data = data;
		}

		public void Save()
		{
			if (_data != null) _data.Value = _dlOrderStatus.SelectedValue;

			var documentId = int.Parse(HttpContext.Current.Request.QueryString["id"]);
			var orderDoc = new Document(documentId);

			var orderGuidValue = orderDoc.getProperty("orderGuid").Value;
			if (orderGuidValue == null)
			{
				return;
			}
			if (string.IsNullOrEmpty(orderGuidValue.ToString()))
			{
				return;
			}

			var orderInfo = OrderHelper.GetOrder(Guid.Parse(orderDoc.getProperty("orderGuid").Value.ToString()));

			orderInfo.SetStatus((OrderStatus) Enum.Parse(typeof (OrderStatus), _dlOrderStatus.SelectedValue), _cbSentEmail.Checked);
			orderInfo.Save();
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
			_cbSentEmail = new CheckBox();
			_lblSentEmail = new Label();

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

			var lblSentEmailText = library.GetDictionaryItem("SendEmailsWithStatusUpdate");
			if (string.IsNullOrEmpty(lblSentEmailText))
			{
				lblSentEmailText = "Send Emails With Status Update";
			}

			_lblSentEmail.Text = lblSentEmailText;
			_lblSentEmail.AssociatedControlID = _cbSentEmail.ID;

			var documentId = int.Parse(HttpContext.Current.Request.QueryString["id"]);

			var orderDoc = new Document(documentId);

			var orderGuidValue = orderDoc.getProperty("orderGuid").Value;
			if (orderGuidValue != null && !string.IsNullOrEmpty(orderGuidValue.ToString()))
			{
				var orderGuid = Guid.Parse(orderDoc.getProperty("orderGuid").Value.ToString());

				var orderInfo = OrderHelper.GetOrderInfo(orderGuid);

				_dlOrderStatus.SelectedValue = orderInfo.Status.ToString();
			}
			else
			{
				_dlOrderStatus.SelectedValue = OrderStatus.Incomplete.ToString();
			}

			if (ContentTemplateContainer != null) ContentTemplateContainer.Controls.Add(_dlOrderStatus);
			if (ContentTemplateContainer != null) ContentTemplateContainer.Controls.Add(new HtmlGenericControl("br"));
			if (ContentTemplateContainer != null) ContentTemplateContainer.Controls.Add(_cbSentEmail);
			if (ContentTemplateContainer != null) ContentTemplateContainer.Controls.Add(_lblSentEmail);
		}
	}
}