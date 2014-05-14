using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco;
using umbraco.cms.businesslogic.web;
using umbraco.interfaces;
using uWebshop.Domain.Helpers;

namespace uWebshop.Umbraco.DataTypes.OrderInfoViewer
{
	public class OrderInfoViewerDataEditor : UpdatePanel, IDataEditor
	{
		private IData _data;

		private Label _lblOrderInfo;

		public OrderInfoViewerDataEditor(IData data)
		{
			_data = data;
		}

		public void Save()
		{
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

			_lblOrderInfo = new Label();

			var documentId = int.Parse(HttpContext.Current.Request.QueryString["id"]);
			var orderDoc = new Document(documentId);

			var orderGuidValue = orderDoc.getProperty("orderGuid").Value;
			if (orderGuidValue != null && !string.IsNullOrEmpty(orderGuidValue.ToString()))
			{
				var orderGuid = Guid.Parse(orderDoc.getProperty("orderGuid").Value.ToString());

				var orderInfoXml = OrderHelper.GetOrderXML(orderGuid);

				var parameters = new Dictionary<string, object>(1) {{"uniqueOrderId", orderGuid.ToString()}};

				var transformation = macro.GetXsltTransformResult(orderInfoXml, macro.getXslt("uwbsOrderInfo.xslt"), parameters);

				_lblOrderInfo.Text = transformation ?? "uwbsOrderInfo.xslt render issue";
			}
			else
			{
				_lblOrderInfo.Text = "Unique Order ID not found. Republish might solve this issue";
			}


			if (ContentTemplateContainer != null) ContentTemplateContainer.Controls.Add(_lblOrderInfo);
		}
	}
}