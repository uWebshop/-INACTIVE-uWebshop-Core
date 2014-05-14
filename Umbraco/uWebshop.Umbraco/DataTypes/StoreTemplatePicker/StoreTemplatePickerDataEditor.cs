using System;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco;
using umbraco.cms.businesslogic.web;
using umbraco.interfaces;
using uWebshop.Domain;

namespace uWebshop.Umbraco.DataTypes.StoreTemplatePicker
{
	public class StoreTemplatePickerDataEditor : UpdatePanel, IDataEditor
	{
		private readonly IData _data;

		private DropDownList _dlTemplates;
		private DocumentType documentType;

		public void Save()
		{
			if (!(Page.Request.CurrentExecutionFilePath ?? string.Empty).Contains("editContent.aspx"))
				return;

			if (_data != null) _data.Value = _dlTemplates.SelectedValue;
		}

		public bool ShowLabel
		{
			get { return true; }
		}

		public bool TreatAsRichTextEditor
		{
			get { return false; }
		}

		public StoreTemplatePickerDataEditor(IData data)
		{
			_data = data;
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

			_dlTemplates = new DropDownList();

			var useDefaultText = library.GetDictionaryItem("UseDefault");
			if (string.IsNullOrEmpty(useDefaultText))
			{
				useDefaultText = "Use Default";
			}

			int currentId;

			int.TryParse(Page.Request.QueryString["id"], out currentId);


			if (currentId != 0)
			{
				var document = new Document(currentId);

				documentType = DocumentType.GetByAlias(document.ContentType.Alias);
			}

			_dlTemplates.Items.Add(new ListItem(useDefaultText, "0"));

			if (documentType != null)
			{
				foreach (var template in documentType.allowedTemplates)
				{
					_dlTemplates.Items.Add(new ListItem(template.Text, template.Id.ToString(CultureInfo.InvariantCulture)));
				}
			}
			else
			{
				foreach (var template in umbraco.cms.businesslogic.template.Template.GetAllAsList())
				{
					_dlTemplates.Items.Add(new ListItem(template.Text, template.Id.ToString(CultureInfo.InvariantCulture)));
				}
			}

			if (_data != null) _dlTemplates.SelectedValue = _data.Value.ToString();

			if (ContentTemplateContainer != null) ContentTemplateContainer.Controls.Add(_dlTemplates);
		}
	}
}