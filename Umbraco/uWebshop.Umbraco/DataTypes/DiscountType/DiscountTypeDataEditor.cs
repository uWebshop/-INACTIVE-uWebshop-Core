using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco;
using umbraco.cms.businesslogic.web;
using umbraco.interfaces;
using uWebshop.Domain;

namespace uWebshop.Umbraco.DataTypes.DiscountType
{
	public class DiscountTypeDataEditor : UpdatePanel, IDataEditor
	{
		private readonly IData _data;

		private DropDownList _dlDiscountTypes;

		public DiscountTypeDataEditor(IData data)
		{
			_data = data;
		}

		public void Save()
		{
			if (_data != null) _data.Value = _dlDiscountTypes.SelectedValue;
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

			_dlDiscountTypes = new DropDownList();

			var discountTypeAmountText = library.GetDictionaryItem("DiscountTypeAmount");
			if (string.IsNullOrEmpty(discountTypeAmountText))
			{
				discountTypeAmountText = "Amount";
			}

			var discountTypePercentageText = library.GetDictionaryItem("DiscountTypePercentage");
			if (string.IsNullOrEmpty(discountTypePercentageText))
			{
				discountTypePercentageText = "Percentage";
			}


			var discountTypeFreeShippingText = library.GetDictionaryItem("DiscountTypeFreeShipping");
			if (string.IsNullOrEmpty(discountTypeFreeShippingText))
			{
				discountTypeFreeShippingText = "Free Shipping";
			}

			var discountTypeNewPriceText = library.GetDictionaryItem("DiscountTypeNewPrice");
			if (string.IsNullOrEmpty(discountTypeNewPriceText))
			{
				discountTypeNewPriceText = "New Price";
			}

			_dlDiscountTypes.Items.Add(new ListItem(discountTypeAmountText, Common.DiscountType.Amount.ToString()));
			_dlDiscountTypes.Items.Add(new ListItem(discountTypePercentageText, Common.DiscountType.Percentage.ToString()));
			_dlDiscountTypes.Items.Add(new ListItem(discountTypeNewPriceText, Common.DiscountType.NewPrice.ToString()));


			var nodeIdstring = HttpContext.Current.Request["id"];

			int nodeId;
			int.TryParse(nodeIdstring, out nodeId);

			if (nodeId > 0)
			{
				var doc = new Document(nodeId);

				if (!doc.ContentType.Alias.StartsWith(DiscountProduct.NodeAlias))
				{
					_dlDiscountTypes.Items.Add(new ListItem(discountTypeFreeShippingText, Common.DiscountType.FreeShipping.ToString()));
				}
			}
			
			_dlDiscountTypes.SelectedValue = _data.Value.ToString();

			if (ContentTemplateContainer != null) ContentTemplateContainer.Controls.Add(_dlDiscountTypes);
		}
	}
}