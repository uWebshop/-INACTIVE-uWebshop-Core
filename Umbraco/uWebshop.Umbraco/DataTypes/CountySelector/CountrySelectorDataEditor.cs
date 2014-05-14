using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco.interfaces;
using uWebshop.Domain.Helpers;

namespace uWebshop.Umbraco.DataTypes.CountySelector
{
	public class CountrySelectorDataEditor : UpdatePanel, IDataEditor
	{
		private readonly IData _data;

		private DropDownList _dlCountries;

		public CountrySelectorDataEditor(IData data)
		{
			_data = data;
		}

		public void Save()
		{
			if (_data != null) _data.Value = _dlCountries.SelectedValue;
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

			_dlCountries = new DropDownList();

			foreach (var countryListItem in StoreHelper.GetAllCountries().Select(country => new ListItem(country.Name, country.Code)))
			{
				_dlCountries.Items.Add(countryListItem);
			}

			_dlCountries.SelectedValue = _data.Value.ToString();

			if (ContentTemplateContainer != null) ContentTemplateContainer.Controls.Add(_dlCountries);
		}
	}
}