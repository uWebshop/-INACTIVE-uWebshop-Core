using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using uWebshop.Domain;
using uWebshop.Domain.Interfaces;
using umbraco.BasePages;
using umbraco.cms.businesslogic.web;
using umbraco.controls;
using uWebshop.Domain.Upgrading;

namespace uWebshop.Package.Installer
{
	public partial class uWebshopDashBoard : UserControl
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			var path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);

			if (path != null)
			{
				var localPath = new Uri(path).LocalPath;

				var assembly = Assembly.LoadFrom(localPath + "/uWebshop.Domain.dll");
				if (assembly != null)
				{
					var versionInfo = assembly.GetName().Version;

					if (uwbsVersionInfo != null)
					{
						uwbsVersionInfo.Text = string.Format("{0}", versionInfo);
					}
				}
			}
		}

		protected ContentPicker NodePickerStore = new ContentPicker {ID = "nodePickerStore", AppAlias = "content", ClientIDMode = ClientIDMode.Static};
		protected ContentPicker NodePickerContent = new ContentPicker {ID = "nodePickerContent", AppAlias = "content", ClientIDMode = ClientIDMode.Static};

		public static List<DocumentType> DocumentTypeListWithoutuWebshopDocuments
		{
			get
			{
				var docTypeList = DocumentType.GetAllAsList();

				foreach (var docType in docTypeList.ToList().Where(docType => DocumentTypeAliasList.List.Contains(docType.Alias)))
				{
					docTypeList.Remove(docType);
				}

				return docTypeList;
			}
		}

		protected void BtnStorePickerCLick(object sender, EventArgs e)
		{
			var nodePicker = NodePickerStore;

			if (!string.IsNullOrWhiteSpace(nodePicker.Value))
			{
				int id = int.Parse(nodePicker.Value);
				string feedbackSmall, feedbackLarge;

				var success = IO.Container.Resolve<ICMSInstaller>().InstallStorePickerOnNodeWithId(id, out feedbackSmall, out feedbackLarge);
				BasePage.Current.ClientTools.ShowSpeechBubble(success ? BasePage.speechBubbleIcon.success : BasePage.speechBubbleIcon.error, feedbackSmall, feedbackLarge);
			}
			else
			{
				BasePage.Current.ClientTools.ShowSpeechBubble(BasePage.speechBubbleIcon.error, "StorePicker Install Failed", "StorePicker install failed, no node selected");
			}
		}

		protected void UpgradeVersionClick(object sender, EventArgs e)
		{
			new OrderTableUpdater().AddStoreOrderReferenceIdToExistingOrders();

			new OrderTableUpdater().UpdateXMLAndFieldsOfExistingOrders();

			BasePage.Current.ClientTools.ShowSpeechBubble(BasePage.speechBubbleIcon.success, "Orders Updated!", "All orders are now updated to the newest versions");
		}
	}
}