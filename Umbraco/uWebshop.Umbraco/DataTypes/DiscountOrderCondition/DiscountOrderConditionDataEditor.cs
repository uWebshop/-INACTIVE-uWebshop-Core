using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco;
using umbraco.interfaces;

namespace uWebshop.Umbraco.DataTypes.DiscountOrderCondition
{
	public class DiscountOrderConditionDataEditor : UpdatePanel, IDataEditor
	{
		private readonly IData _data;

		private DropDownList _dlDiscountOrderCondition;

		public DiscountOrderConditionDataEditor(IData data)
		{
			_data = data;
		}

		public void Save()
		{
			if (_data != null) _data.Value = _dlDiscountOrderCondition.SelectedValue;
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

			_dlDiscountOrderCondition = new DropDownList();

			var discountOrderConditionNoneText = library.GetDictionaryItem("DiscountOrderConditionNone");
			if (string.IsNullOrEmpty(discountOrderConditionNoneText))
			{
				discountOrderConditionNoneText = "None";
			}

			var discountOrderConditionOnTheXthItemText = library.GetDictionaryItem("DiscountOrderConditionOnTheXthItem");
			if (string.IsNullOrEmpty(discountOrderConditionOnTheXthItemText))
			{
				discountOrderConditionOnTheXthItemText = "On The Xth Item";
			}

			var discountOrderConditionPerSetOfXItemsText = library.GetDictionaryItem("DiscountOrderConditionPerSetOfXItems");
			if (string.IsNullOrEmpty(discountOrderConditionPerSetOfXItemsText))
			{
				discountOrderConditionPerSetOfXItemsText = "Per Set Of X Items";
			}

			_dlDiscountOrderCondition.Items.Add(new ListItem(discountOrderConditionNoneText, Common.DiscountOrderCondition.None.ToString()));
			_dlDiscountOrderCondition.Items.Add(new ListItem(discountOrderConditionOnTheXthItemText, Common.DiscountOrderCondition.OnTheXthItem.ToString()));
			_dlDiscountOrderCondition.Items.Add(new ListItem(discountOrderConditionPerSetOfXItemsText, Common.DiscountOrderCondition.PerSetOfXItems.ToString()));

			_dlDiscountOrderCondition.SelectedValue = _data.Value.ToString();

			if (ContentTemplateContainer != null) ContentTemplateContainer.Controls.Add(_dlDiscountOrderCondition);
		}
	}
}