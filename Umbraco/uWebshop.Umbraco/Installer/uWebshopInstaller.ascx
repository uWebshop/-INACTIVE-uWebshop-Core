<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="uWebshopInstaller.ascx.cs" Inherits="uWebshop.Package.Installer.uWebshopInstaller" %>
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
	    <p>If you need any help with your setup please visit our <a target="_blank" href="http://docs.uWebshop.com">documentation portal</a> or contact our <a href="http://support.uwebshop.com" title="uWebshop Support">support desk</a></p>
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
					<li>Our support desk is ready for your questions and can be reached via <a target="_blank" href="http://support.uWebshop.com">http://support.uWebshop.com</a></li>        
                    <li>Buy your commercial license and support packages at our website <a target="_blank" href="http://uwebshop.com/purchase/">http://uwebshop.com/purchase</a></li>        
                    <li>Become an uWebshop expert by attending a <a target="_blank" href="http://uwebshop.com/certification/">the uWebshop certification course</a>.</li>        
				</ul>
			</div>
		</div>
	</div>
</umb:pane>

<umb:pane ID="panel2" runat="server" visible="true">
	<div class="dashboardWrapper">
		<h2>Create a store (Optional)</h2>
		<img src="/App_Plugins/uWebshop/images/store32x32.png" alt="Create an uWebshop Store!" class="dashboardIcon" />
		<p>uWebshop uses the principle of 'stores'. You need at least one store in your uWebshop installation. This simple form will create your first store. You can create and remove as many stores as you like.</p>
		<p>If you like to create the store node yourself, or if you plan to install one of our demostore starterkits you can skip creating a store now.</p>
		<asp:Label ID="Label1" runat="server" AssociatedControlID="txtStoreAlias" Text="Name of the store:"></asp:Label>
		<asp:TextBox runat="server" ID="txtStoreAlias"/>
		<asp:Button CssClass="button" runat="server" ID="btnInstallStore" OnClick="BtnInstallStoreClick" Text="Create Store"/>
	</div>
</umb:pane>

<umb:pane ID="panel3" runat="server" visible="true">
	<div class="dashboardWrapper">
		<h2>Add store picker to your current website (Optional)</h2>
		<img src="/App_Plugins/uWebshop/images/storepicker32x32.png" alt="Create an uWebshop Store!" class="dashboardIcon" />
		<p>Adding a store picker to your existing website will allow your site to work with uWebshop. If you don't have a website yet, you can <strong><a href="/install/?installStep=skinning" target="_blank">install an Umbraco Starter Kit</a></strong> or create a website.</p>
		<p>If you have an existing site, this helper will help you to add the Store picker to the node where the store specific pages will be located under (Usually this will be the homepage).</p>
	    <p>If you only need one store and this store will use the root of your site as the store node, you don't need a store picker.</p>
		<asp:Label runat="server" ID="lblStorePicker" Text="Pick the node:" /><br/>
		<asp:PlaceHolder runat="server" ID="phInstallStorePicker"/><asp:Button runat="server" ID="btnInstallStorePicker" CssClass="button" Text="Install Store Picker" OnClick="BtnStorePickerCLick"/>
	</div>
</umb:pane>

