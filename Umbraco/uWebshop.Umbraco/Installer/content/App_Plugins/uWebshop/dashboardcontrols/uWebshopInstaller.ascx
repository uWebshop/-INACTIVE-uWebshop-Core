<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="uWebshopInstaller.ascx.cs" Inherits="uWebshop.Umbraco.Installer.uWebshopInstaller" %>
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

<umb:pane ID="pane7" runat="server" visible="true">
	<div class="dashboardWrapper">
		<h2>Thank you for installing uWebshop for Umbraco!</h2>
		<img src="/App_Plugins/uWebshop/images/uWebshop32x32.png" alt="uWebshop" class="dashboardIcon" />
		<p>uWebshop is a powerful, easy to manage, e-commerce solution for the Umbraco content management system and will get you setup with a store solution on Umbraco in minutes!</p>
	</div>
</umb:pane>

<umb:pane ID="panel5" runat="server" visible="true">
	<div class="dashboardWrapper">
		<h2>What's next?</h2>
		<img src="/App_Plugins/uWebshop/images/documentation32x32.png" alt="Work to do!" class="dashboardIcon" />
		<div class="dashboardColWrapper">
			<div class="dashboardCols">
				<ul>
					<li><a href="http://uwebshop.com/download" target="_blank">Download and install one of our demoshop package</a>! This will give you a great starting point. Available in both webforms and MVC flavours!</li>
					<li>Documentation can be found at <a target="_blank" href="http://docs.uWebshop.com">our documentation portal</a></li>              
				</ul>
			</div>
		</div>
	</div>
</umb:pane>

