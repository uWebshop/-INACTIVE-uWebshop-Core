using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Xml.Linq;
using umbraco.BasePages;
using umbraco.cms.businesslogic;
using umbraco.cms.businesslogic.language;

namespace uWebshop.DictionaryInstaller
{
	public class uWebshopDictionaryInstaller : UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			var languageXmlPath = HttpContext.Current.Server.MapPath("/App_Plugins/uWebshop/dictionary/uWebshopDictionary.xml");

			var languageXml = XDocument.Load(languageXmlPath);

			var languageXNode = languageXml.Descendants("Value").FirstOrDefault();

			if (languageXNode == null) return;

			var xAttribute = languageXNode.Attribute("LanguageCultureAlias");
			if (xAttribute == null) return;


			var languageCode = xAttribute.Value;

			var language = Language.GetByCultureCode(languageCode);

			if (language == null)
			{
				Language.MakeNew(languageCode);

				language = Language.GetByCultureCode(languageCode);
			}

			if (languageXmlPath != null)
			{
				InstallDictionary(language, languageXml);

				File.Delete(languageXmlPath);

				BasePage.Current.ClientTools.ShowSpeechBubble(BasePage.speechBubbleIcon.success, "Dictionary Installed!", "Dictionary filled with EN-GB Language!");
			}
			else
			{
				BasePage.Current.ClientTools.ShowSpeechBubble(BasePage.speechBubbleIcon.error, "Dictionary Install Failed", "Dictionary is not filled with EN-GB Language");
			}
		}

		protected void InstallDictionary(Language lang, XDocument languageXml)
		{
			foreach (var item in languageXml.Descendants("DictionaryItem").Where(x => x.Parent != null && x.Parent.Name == "DictionaryItems"))
			{
				InstallDictionaryItems(item, null);
			}

			UpdateDictionaryItems(Dictionary.getTopMostItems, lang, languageXml);
		}

		private void InstallDictionaryItems(XElement item, string parentKey)
		{
			var xAttribute = item.Attribute("Key");
			if (xAttribute != null)
			{
				var dictionaryItemfromXml = xAttribute.Value;

				if (!Dictionary.DictionaryItem.hasKey(dictionaryItemfromXml))
				{
					if (parentKey != null)
					{
						Dictionary.DictionaryItem.addKey(dictionaryItemfromXml, string.Empty, parentKey);
					}
					else
					{
						Dictionary.DictionaryItem.addKey(dictionaryItemfromXml, string.Empty);
					}

					if (item.Descendants().Any())
					{
						foreach (var childnode in item.Descendants().Where(x => x.Parent == item))
						{
							InstallDictionaryItems(childnode, dictionaryItemfromXml);
						}
					}
				}
				else
				{
					if (item.Descendants().Any())
					{
						foreach (var childnode in item.Descendants().Where(x => x.Parent == item))
						{
							InstallDictionaryItems(childnode, dictionaryItemfromXml);
						}
					}
				}
			}
		}

		private static void UpdateDictionaryItems(IEnumerable<Dictionary.DictionaryItem> items, Language lang, XContainer languageXml)
		{
			foreach (var di in items)
			{
				var di1 = di;
				var dictionaryXmlNode = languageXml.Descendants("DictionaryItem").FirstOrDefault(x =>
					{
						var xAttribute = x.Attribute("Key");
						return xAttribute != null && xAttribute.Value == di1.key;
					});

				if (dictionaryXmlNode != null)
				{
					var firstOrDefault = dictionaryXmlNode.Descendants("Value").FirstOrDefault();
					if (firstOrDefault != null && string.IsNullOrEmpty(di.Value(lang.id)))
					{
						di.setValue(lang.id, firstOrDefault.Value);
					}
				}

				UpdateDictionaryItems(di.Children, lang, languageXml);
			}
		}
	}
}