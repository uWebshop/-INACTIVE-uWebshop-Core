<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="uWebshopDashBoard.ascx.cs" Inherits="uWebshop.Package.Installer.uWebshopDashBoard" %>
<%@ Register TagPrefix="umb" Namespace="umbraco.uicontrols" Assembly="controls" %>
<link href="/umbraco_client/propertypane/style.css" rel="stylesheet" />

<style>
	hr.line {
		border: none;
		border-bottom: 1px solid #ccc;
	}

	input.button {
		padding: 5px 10px;
		font-size: 12px;
		margin-left: 10px;
	}
</style>

<umb:Pane ID="pane1" runat="server" Visible="true">
	<div class="dashboardWrapper">
		<h2>uWebshop Version</h2>
		<img src="/App_Plugins/uWebshop/images/uWebshop32x32.png" alt="Create an uWebshop Store!" class="dashboardIcon" />
		<p><asp:Literal runat="server" ID="uwbsVersionInfo"/></p>
		<hr class="line"/>
		<h3>uWebshop Changelog</h3>
		<p>View the uWebshop changelog <a href="http://uwebshop.com/changelog" target="_blank">on the uWebshop Website</a></p>
	</div>
</umb:Pane>

<umb:Pane ID="panel3" runat="server" Visible="true">
	<div class="dashboardWrapper">
		<h2>Add store picker to your current website</h2>
		<img src="/App_Plugins/uWebshop/images/storepicker32x32.png" alt="Create an uWebshop Store!" class="dashboardIcon" />
		<p>Adding a store picker to your website will allow your site to work with uWebshop.<br/>If you don't have a website yet <strong><a href="/install/?installStep=skinning" target="_blank">Install a Starter Kit</a></strong> or create a new website.</p>
				
		<p>Add the "Store Picker" datatype to the node where the store specific pages will be located under (Usually this will the Homepage).</p>
		<asp:Label runat="server" ID="lblStorePicker" Text="Pick the node:" AssociatedControlID="nodePickerStore"/><asp:PlaceHolder runat="server" ID="phInstallStorePicker"/><br/><asp:Button runat="server" ID="btnInstallStorePicker" CssClass="button"  Text="Install Store Picker" OnClick="BtnStorePickerCLick"/>
	</div>
</umb:Pane>

<umb:Pane ID="panel5" runat="server" Visible="true">
	<div class="dashboardWrapper">
		<h2>Documentation</h2>
		<img src="/App_Plugins/uWebshop/images/documentation32x32.png" alt="Work to do!" class="dashboardIcon" />
		<div class="dashboardColWrapper">
			<div class="dashboardCols">
				<ul>
					<li>You can find the post installation/getting started guide on our support platform. <a target="_blank" href="https://uwebshop.zendesk.com/entries/21952063-uwebshop-2-0-post-install-guide">Read the post-installation documentation</a></li>                            
					<li>Create templates to display categories and products and connect them to the uwbsCategory &amp; uwbsProduct document types. <a target="_blank" href="https://uwebshop.zendesk.com/forums/20718497-tips-tricks">read about uWebshop templating</a></li>
					<li>The default uWebshop URLrewriting uses a node "store" to render the uWebshop catalog to your site. Create this node. <a target="_blank" href="https://uwebshop.zendesk.com/forums/20718497-tips-tricks">read about uWebshop url rewriting</a></li>
					<li>The installer added a new macro <em>"BasketHandler"</em> to handle all the order updates. This macro should be placed on the template or the node you want to use for basket update (we advise to use the Master Document Template)  <a target="_blank" href="https://uwebshop.zendesk.com/forums/20718497-tips-tricks">read about the uWebshop basket handler</a></li>                      
				</ul>
			</div>
		</div>
	</div>
</umb:Pane>

<umb:Pane ID="panel6" runat="server" Visible="true">
	<div class="dashboardWrapper">
		<h2>Update Orders</h2>
		<img src="/App_Plugins/uWebshop/images/update32x32.png" alt="Work to do!" class="dashboardIcon" />
		<div class="dashboardColWrapper">
			<div class="dashboardCols">
				<div class="dashboardCol">
					<p>When this button is clicked, uWebshop will update all the old orders to include any new files added with the current release.</p>
					<asp:UpdatePanel ID="ordersUpdatePanel" runat="server">
						 <ContentTemplate>
							<asp:Button ID="upgradeToCurrentVersionButton" runat="server" CssClass="button"  Text="Upgrade uWebshop Orders" OnClick="UpgradeVersionClick" />
						 </ContentTemplate>
					</asp:UpdatePanel>
					<asp:UpdateProgress ID="ordersUpdateProgress" AssociatedUpdatePanelID="ordersUpdatePanel" runat="server">
						<ProgressTemplate>
							<div style="height: 100%; width: 100%; position: fixed; left: 0px; top: 0px; z-index: 9999;" class="jqmOverlay">&nbsp;</div>
						</ProgressTemplate>
					</asp:UpdateProgress>
				</div>
			</div>
		</div>
	</div>
</umb:Pane>