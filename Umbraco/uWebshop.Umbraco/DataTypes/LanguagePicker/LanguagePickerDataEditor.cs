using System;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco.BusinessLogic;
using umbraco.interfaces;
using uWebshop.Domain;

namespace uWebshop.Umbraco.DataTypes.LanguagePicker
{
	public class LanguagePickerDataEditor : UpdatePanel, IDataEditor
	{
		private readonly IData _data;

		private DropDownList _dlInstalledLanguages;

		public LanguagePickerDataEditor(IData data)
		{
			_data = data;
		}

		public void Save()
		{
			if (_data != null) _data.Value = _dlInstalledLanguages.SelectedValue;
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

			_dlInstalledLanguages = new DropDownList();

			foreach (var language in umbraco.cms.businesslogic.language.Language.GetAllAsList())
			{
				var culture = new CultureInfo(language.CultureAlias);
				if (!culture.IsNeutralCulture)
				{
					var currencyRegion = new RegionInfo(culture.LCID);

					var value = language.FriendlyName + " " + currencyRegion.ISOCurrencySymbol + " (" + currencyRegion.CurrencySymbol +
					            " - " +
					            currencyRegion.CurrencyEnglishName + ")";

					_dlInstalledLanguages.Items.Add(new ListItem(value, language.id.ToString(CultureInfo.InvariantCulture)));
				}
				//else
				//{
				//	var value = language.FriendlyName;

				//	_dlInstalledLanguages.Items.Add(new ListItem(value, language.id.ToString(CultureInfo.InvariantCulture)));
				//}
			}
			_dlInstalledLanguages.SelectedValue = _data.Value.ToString();

			var user = User.GetCurrent();

			if (!user.IsAdmin())
				_dlInstalledLanguages.Enabled = false;

			if (ContentTemplateContainer != null) ContentTemplateContainer.Controls.Add(_dlInstalledLanguages);
		}
	}
}