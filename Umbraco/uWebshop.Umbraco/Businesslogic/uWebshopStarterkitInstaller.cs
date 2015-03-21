using System;
using System.Linq;
using System.Web.UI;
using umbraco;
using umbraco.BasePages;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.web;
using uWebshop.Domain;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Starterkits.Sandbox
{
	public partial class uWebshopStarterkitInstaller : UserControl
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			bool storePresent;
			IO.Container.Resolve<ICMSInstaller>().InstallStarterkit("sandbox", out storePresent);
			if (!storePresent)
			{
				panel2.Visible = true;
				BasePage.Current.ClientTools.ShowSpeechBubble(BasePage.speechBubbleIcon.info, "Automatically Created Store", "Please Republish All uWebshop Nodes");
			}
		}

		protected void BtnInstallStoreClick(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(txtStoreAlias.Text))
			{
				BasePage.Current.ClientTools.ShowSpeechBubble(BasePage.speechBubbleIcon.error, "Error", "No Store name entered");
			}
			else
			{
				Umbraco.Helpers.InstallStore(txtStoreAlias.Text);
				BasePage.Current.ClientTools.ShowSpeechBubble(BasePage.speechBubbleIcon.success, "Store Installed!", "Your " + txtStoreAlias.Text + " Store is installed!");
			}
		}

		public static bool PublishWithChildrenWithResult(Document doc, User u)
		{
			if (doc.PublishWithResult(u))
			{
				foreach (var dc in doc.Children.ToList())
				{
					dc.PublishWithChildrenWithResult(u);
				}
				return true;
			}
			return false;
		}

		[Obsolete("Umbraco now updates cache after publish ")]
		public static void UpdateDocumentCache(Document doc)
		{
			
		}
	}

	public partial class uWebshopStarterkitInstaller
	{
		/// <summary>
		/// panel2 control.
		/// </summary>
		/// <remarks>
		/// Auto-generated field.
		/// To modify move field declaration from designer file to code-behind file.
		/// </remarks>
		protected global::umbraco.uicontrols.Pane panel2;

		/// <summary>
		/// Label1 control.
		/// </summary>
		/// <remarks>
		/// Auto-generated field.
		/// To modify move field declaration from designer file to code-behind file.
		/// </remarks>
		protected global::System.Web.UI.WebControls.Label Label1;

		/// <summary>
		/// txtStoreAlias control.
		/// </summary>
		/// <remarks>
		/// Auto-generated field.
		/// To modify move field declaration from designer file to code-behind file.
		/// </remarks>
		protected global::System.Web.UI.WebControls.TextBox txtStoreAlias;

		/// <summary>
		/// btnInstallStore control.
		/// </summary>
		/// <remarks>
		/// Auto-generated field.
		/// To modify move field declaration from designer file to code-behind file.
		/// </remarks>
		protected global::System.Web.UI.WebControls.Button btnInstallStore;

		/// <summary>
		/// panel5 control.
		/// </summary>
		/// <remarks>
		/// Auto-generated field.
		/// To modify move field declaration from designer file to code-behind file.
		/// </remarks>
		protected global::umbraco.uicontrols.Pane panel5;
	}
}