using System;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco.cms.businesslogic.datatype;
using umbraco.cms.businesslogic.web;
using umbraco.interfaces;
using uWebshop.DataAccess;
using uWebshop.Domain;

namespace uWebshop.Umbraco.DataTypes.StockUpdate
{
	internal class StockUpdateDataEditor : UpdatePanel, IDataEditor
	{
		private readonly IData _data;

		private TextBox _txtStock;

		public StockUpdateDataEditor(IData data)
		{
			_data = data;
		}

		public void Save()
		{
			if (!(Page.Request.CurrentExecutionFilePath ?? string.Empty).Contains("editContent.aspx"))
				return;

			int currentId;

			int.TryParse(Page.Request.QueryString["id"], out currentId);

			var currentDoc = new Document(currentId);

			// Get the current property type
			var propertyTypeId = ((DefaultData) this._data).PropertyId;

			var property = currentDoc.GenericProperties.FirstOrDefault(x => x.Id == propertyTypeId);

			var propertyAlias = property.PropertyType.Alias;
			// test if the property alias contains an shopalias

			var storeAlias = GetStoreAliasFromProperyAlias(propertyAlias);

			int newStock;

			int.TryParse(_txtStock.Text, out newStock);

			if (currentId != 0)
			{
				UWebshopStock.UpdateStock(currentId, newStock, false, storeAlias);
			}
		}

		private static string GetStoreAliasFromProperyAlias(string propertyAlias)
		{
			var propertyParts = propertyAlias.Split('_');
			var storeAlias = propertyParts.Length >= 2 ? propertyParts[1] : string.Empty;
			return storeAlias;
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

			int currentId;

			int.TryParse(Page.Request.QueryString["id"], out currentId);

			if (!(Page.Request.CurrentExecutionFilePath ?? string.Empty).Contains("editContent.aspx"))
				return;

			if (currentId == 0) return;
			var currentDoc = new Document(currentId);

			// Get the current property type
			var propertyTypeId = ((DefaultData) _data).PropertyId;

			var property = currentDoc.GenericProperties.FirstOrDefault(x => x.Id == propertyTypeId);

			var propertyAlias = property.PropertyType.Alias;
			// test if the property alias contains an shopalias

			var storeAlias = GetStoreAliasFromProperyAlias(propertyAlias);


			var stock = UWebshopStock.GetStock(currentId, storeAlias);

			_txtStock = new TextBox {Text = stock.ToString(CultureInfo.InvariantCulture)};

			if (ContentTemplateContainer != null) ContentTemplateContainer.Controls.Add(_txtStock);
		}
	}
}