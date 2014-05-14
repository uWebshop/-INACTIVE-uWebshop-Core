using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using umbraco.cms.businesslogic.web;
using umbraco.interfaces;
using umbraco.IO;
using uWebshop.Domain.Helpers;

namespace uWebshop.Umbraco.DataTypes.EmailPreview
{
	public class EmailPreviewDataEditor : UpdatePanel, IDataEditor
	{
		private Literal _lblRenderRazorContent;

		private readonly IData _data;

		public EmailPreviewDataEditor(IData data)
		{
			_data = data;
		}

		public void Save()
		{
			var saveValue = (Dictionary<string, string>) HttpContext.Current.Session["RazorFields"];

			if (saveValue != null && saveValue.Any())
			{
				var el = new XElement("values", saveValue.Select(kv => new XElement(kv.Key, kv.Value)));

				if (_data != null) _data.Value = el.ToString();

				HttpContext.Current.Session.Remove("RazorFields");
			}
		}

		public virtual bool ShowLabel
		{
			get { return false; }
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

			try
			{
				var documentId = HttpContext.Current.Request["id"];

				int docId;
				int.TryParse(documentId, out docId);

				if (docId != 0)
				{
					var emailDoc = new Document(docId);

					var property = emailDoc.getProperty("emailtemplate");

					if(property.Value == null)
					{
						// fallback for old installations
						property = emailDoc.getProperty("xslttemplate");
					}

					if (property.Value != null)
					{
						var value = property.Value.ToString();

						if (!string.IsNullOrEmpty(value))
						{
							var fileLocation = string.Format("{0}/{1}", SystemDirectories.MacroScripts.TrimEnd('/'), value.TrimStart('/'));

							_lblRenderRazorContent = new Literal {Text = RazorLibraryExtensions.RenderMacro(fileLocation, docId)};



							if (ContentTemplateContainer != null) ContentTemplateContainer.Controls.Add(_lblRenderRazorContent);
						}
					}
					else
					{

						_lblRenderRazorContent = new Literal {Text = "No template set or found, reload node?"};

						if (ContentTemplateContainer != null) ContentTemplateContainer.Controls.Add(_lblRenderRazorContent);
					}
				}
			}
			catch
			{
				_lblRenderRazorContent = new Literal { Text = "Error rendering data" };

				if (ContentTemplateContainer != null) ContentTemplateContainer.Controls.Add(_lblRenderRazorContent);
			}
		}
	}
}