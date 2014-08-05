<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="uWebshopDashBoard.ascx.cs" Inherits="uWebshop.Package.Installer.uWebshopDashBoard" %>
<%@ Register TagPrefix="umb" Namespace="umbraco.uicontrols" Assembly="controls" %>
<link href="/umbraco_client/propertypane/style.css" rel="stylesheet" />
<asp:ScriptManager ID="ScriptManger1" runat="Server"/>
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
		<p>The installed uWebshop version is: <strong><asp:Literal runat="server" ID="uwbsVersionInfo"/></strong></p>		
		<p>View the uWebshop changelog <a href="http://uwebshop.com/changelog" target="_blank">on the uWebshop Website</a></p>
        <hr class="line"/>
	</div>
</umb:Pane>

<umb:Pane ID="panel5" runat="server" Visible="true">
	<div class="dashboardWrapper">
		<h2>Tips &amp; Tricks</h2>
		<img src="/App_Plugins/uWebshop/images/documentation32x32.png" alt="Work to do!" class="dashboardIcon" />
		<div class="dashboardColWrapper">
			<div class="dashboardCols">
				<p>You can find the post installation/getting started guide on our support platform. <a class="btn" target="_blank" href="http://docs.uwebshop.com/general/guides/getting-started/">Read the post-installation documentation</a></p>
                <p>Read about all the ins-and-out of uWebshop on our <a  class="btn" target="_blank" href="http://docs.uwebshop.com/">Documentation Portal</a></p>
                    <p>If you have questions or want help with uWebshop, <a  class="btn" target="_blank" href="http://support.uwebshop.com/">get in touch with our support desk!</a></p>
                    <p>Did you know we have various support options, including a Gold Partnership that gives you full uWebshop Source access on GitHub? <a class="btn btn-primary" target="_blank" href="http://uwebshop.com/partners/become-an-uwebshop-partner/">uWebshop Support &amp; Partnership Information</a></p>				
			</div>
		</div>
	</div>
</umb:Pane>
