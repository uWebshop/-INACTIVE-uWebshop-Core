using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco.interfaces;

namespace uWebshop.Umbraco.DataTypes.Price
{
	public class PriceDataEditor : UpdatePanel, IDataEditor
	{
		private readonly IData _data;
		private TextBox _txtPrice;

		public PriceDataEditor(IData data)
		{
			_data = data;
		}

		public void Save()
		{
			// 100,00 --> 100.00,00
			float price;
			if (_data != null && float.TryParse(_txtPrice.Text, out price))
			{
				_data.Value = Math.Round(price*100).ToString();
			}
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

			#region Arrange GUI

			decimal price;
			decimal.TryParse(_data.Value.ToString(), out price);
			var value = price/100m;

			// problem can be with . or , separated input: make configurable on store?
			_txtPrice = new TextBox {Text = value.ToString("F")};


			if (ContentTemplateContainer != null)
			{
				ContentTemplateContainer.Controls.Add(_txtPrice);
			}

			#endregion
		}
	}
}