using System;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco.interfaces;
using uWebshop.DataAccess;
using uWebshop.Domain;

namespace uWebshop.Umbraco.DataTypes.OrderCountViewer
{
	public class OrderCountDataEditor : UpdatePanel, IDataEditor
	{
		private IData _data;

		private Label _lblOrderCount;

		public OrderCountDataEditor(IData data)
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

			Licensing.uWebshopTrialMessage();

			if (!(Page.Request.CurrentExecutionFilePath ?? string.Empty).Contains("editContent.aspx"))
				return;

			int currentId;

			int.TryParse(Page.Request.QueryString["id"], out currentId);

			var orderCount = UWebshopStock.GetOrderCount(currentId);

			_lblOrderCount = new Label {Text = orderCount.ToString(CultureInfo.InvariantCulture)};

			if (ContentTemplateContainer != null) ContentTemplateContainer.Controls.Add(_lblOrderCount);
		}
	}
}