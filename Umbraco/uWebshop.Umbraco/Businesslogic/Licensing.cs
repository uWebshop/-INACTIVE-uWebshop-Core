using System;
using System.Linq;
using System.Web.UI;
using PackageGarden.Licensing;
using UmbracoLicense = global::Umbraco.Licensing;
using System.Web;
using umbraco.uicontrols;

namespace uWebshop.Domain
{
	public class Licensing
	{
		#region PackageGarden holy grail

		// Holds the Instance
		private static Validator _packageGardenValidator;

		//Change this to your own contact information. This message will be used in exceptions
		private const string LicenseSupportInfo = "Please contact uWebshop at http://www.uWebshop.com";

		//Add the Product key from the generated product Licenses type in Umbraco here
		private const string ProductKey = "FbDdyHkCINfM8UTWdAXoq2ZfJjdhyW0a1jaisNjBMBlptPxbnUwrLhVgq/uPbj4eorTgJrY0d9cDOq6F4I6OvEKheHcm0NfQQrBG6ZOS+b7w7akGwZ0vuCueoPR5OyoTSmy9mGhs35w+PronjH7FkkmpXr0jS7DTiQKozG6jaYwTw6cUH0fpH8SwdmqC1midYTxc8GcLip4bVPRq1j1zPhSd9KaB0d6x/fiPMuPq7ZpwCUHt51O00pkQKiUpVqU8";

		//Add the Product parameters from the generated product Licenses type in Umbraco here
		private const string ProductParameters = "Gqh2vOTlHBMqfbyvMSzrTtf39SyiybTq07Va+fswgHjZEYsydnjYG4M8fQ7qi1HwtBBye6qXzgeul1Y7AFVTaxljrhZ1zfnn/xDmlEhXjKXOdd9xmTO5RoQZTgJauKbAe+iaeuH2AUDuM649kXCsjnjFlVxCKJhjNdWfLibu3hbbL593l2Y+3q6nQOrulRsPEn+WJ1lG/o2uw1j99/ve1cx5AUbUT7y5mlbHYB3lCQYmSTYOLhA/io8c86rznWn6qnaL2xoR84usSzzqwEerRw==";

		//The path how the validator can access the license License name format is always [productlicensetypename].lic
		private const string LicensefilePath = "~/bin/uWebshop2.lic";

		//The major version of the package
		private const int ProductVersion = 2;

		/// <summary>
		/// SingleTon so you can Access easily access the Validator in your Project by
		/// ProductLicense.Instance....
		/// </summary>
		public static Validator Instance
		{
			get
			{
				if (_packageGardenValidator == null)
				{
					_packageGardenValidator = new Validator(ProductParameters, ProductKey, LicensefilePath, LicenseSupportInfo, ProductVersion);
				}
				return _packageGardenValidator;
			}
		}

		#endregion

		#region umbraco deli licence nonsense

		/*
         *  THIS IS THE CONFIGURATION OF YOUR LICENSE. THIS SHOULD COME PRECONFIGURED FROM UMBRACO DELI
         *  LICENSE_PRODUCTNAME EQUALS THE UNIQUE IDENTIFIER OF YOUR PRODUCT ON THE DELI AND MAY NOT BE ALTERED
         *  LICENSE_PARAMETERS IS THE LICENSE VALIDATION KEY AND MUST ALSO REMAIN THE SAME
         *  LICENSE_CONTACTINFO IS THE INFORMATION SHOWN IN CASE OF INVALID LICENSES, YOU CAN CHANGE THIS TO FIT YOUR
         *  CONTACT AND SUPPORT NEEDS
         * */

		//Contact info to display on trial and error messages
		public static string LICENSE_CONTACTINFO = "Please contact uWebshop on either e-mail: info@uwebshop.com or via http://www.uWebshop.com";

		//product name, notice licensing will look for licenses named <productname>*.lic in /bin
		public static string LICENSE_PRODUCTNAME = "uwebshopuwebshop2";

		//DELI UNIQUE KEY, this must be configued to work with Deli licensing webservice
		public static string LICENSE_DELI_ID = "35344b90-dca9-4541-8f8b-14e00fc25c95";

		//should invalid licenses throw an exception?
		public static bool ThrowExceptionsOnError = false;

		//The license parameters which encrypts and decrypts the license
		public static string LICENSE_PARAMETERS = @"<EncryptedLicenseParameters>
		  <ProductName>uwebshopuwebshop2</ProductName>
		  <RSAKeyValue>
			<Modulus>jJkSaq6dfuOYdgXho2PaEWKRzM1YphKKa2L/PcDA/MDWCQHxGmcMLiJbIwbiLxnpR+Ue7oLa2BBb2nudCeIMGHamptiKOgHeTIzUDhQ+vqOk+Fs25+qkBeJQcqDXcG3TAnzXucBza7J6YSOFUiZ+PuZRrcNHH6bdn0C7ArUNXFE=</Modulus>
			<Exponent>AQAB</Exponent>
			<P>xKRCIW5h9U/g7SFGBQebraZ7A8EN2usA+fUOpVib+4mhQ14iEVkCHHZwXiDEwoCXJSQcRDNJUIbwSwZqMqVjUw==</P>
			<Q>twn2iDJpxyQjkP4O35F/mgTloSqxPvXBtlogP+8AyV1qU+8p1R6rCRyX854K9yHO4VW48UaruCvTUTdoU6RRSw==</Q>
			<DP>nern7N8vrgj7MpRRHgLxI/CZw/cLAG9P9my12VWi1su9hVOYemQHzQHU1dLtEOKh/0LTrHWfgBsN6MJ7ELc/Xw==</DP>
			<DQ>b7xmQfg8eHPIPm/JFpOUiKoqn7sXHm8ZxL655y14lnQvP3PFXrNtB4/r9qzo1rpNt2MEFk3k7/XY90BSsJSjhw==</DQ>
			<InverseQ>gH2KUI3SNA0AbHl3HiwUk94WXwQcCkM+ltJAjTyvpHAarVXte6B/tV4Fajf5Jgc6y0GoJOinH0c1irOCOLvKsg==</InverseQ>
			<D>QAz79t3VohjNO/cx891pWsIs6cAiwoVvybwvy1IkmQDAgRoBXKXVRq8gFbTtFcgYHkii7sVuLUBGmCH4SKhSGaX7xzZX4YR+BpWsfzDVpLYh3puNYeq5CcOeD91CiIpL9Q5dIe5MYWKPgkcOyBwPi51uF5mB/ghQPf+RiIHFYIE=</D>
		  </RSAKeyValue>
		  <DesignSignature>QPWYnAVxK6FP3XBxDEhOzXAGNntAm9IG3aXnteajvbvKv5N2NV9e0GT7xrZdyFsR/HUI3Y0c62Udp0guuCfaoQ+r9SyB2UacMuu0g8tEbIGAPclLzF5zfDqa80BiO1/6+Sv314YHel4TiWG/0o+nT5OCZ298QUKkp0v8h40cmLc=</DesignSignature>
		  <RuntimeSignature>I7XU8rsnAD+wHyUTNx49aC/E2ZEBbnpTsaCz+LH5xuszK3KQhX0VWlbCafLo3+6Ig22zLh9/spr0FfiqDZizs5M5fuAmIA5W21It/GeVQNo+TCyrl+IpOUd/LYTBMzwYSo8kuh+QFx9rX9/FdywIgEL6ieMCl1OBRLpHc4bMw8E=</RuntimeSignature>
		  <KeyStrength>7</KeyStrength>
		</EncryptedLicenseParameters>";

		#endregion

		/*
         * THIS IS A SAMPLE IMPLEMENTATION OF UMBRACO.LICENSING, YOU CAN CHANGE THE BELOW HELPERS IF YOU NEED TO
         * IT IS HOWEVER ONLY NEEDED IF YOU HAVE VERY SPECIFIC LICENSE VALIDATION NEEDS
         * SIMPLY MAKE YOUR PAGE/USERCONTROL/CLASSES INHERITE FROM A LICENSED CLASS, AND YOUR APPLICATION SHOULD
         * VALIDATE DELI LICENSES AUTOMATICLY
         */

		/* HELPERS TO DETERMINE LICENSE STATE */

		public static bool IsValid()
		{
			var valid = false;
			if (Instance.IsValid())
			{
				if (Instance.IsTrial())
				{
					if (!IsInvalid() && !IsTrial())
					{
						valid = true;
					}
				}
				else
				{
					valid = true;
				}
			}
			return valid;
		}

		public static bool IsInvalid()
		{
			var invalid = getValidator().IsInvalid();

			if (invalid && ThrowExceptionsOnError)
				throw new UmbracoLicense.Exceptions.InvalidLicenseException(string.Format("This license is invalid. {0}\n\nYour license has been approved for the following IPs/Domains:\n\n{1}", LICENSE_CONTACTINFO, getValidator().GetLicenseLimitations()));

			return invalid;
		}

		public static bool IsTrial()
		{
			return getValidator().IsTrial();
		}

		/// <summary>
		/// The default uWebshop Trial Message
		/// </summary>
		public static void uWebshopTrialMessage()
		{
			var page = HttpContext.Current.Handler as Page;

			if (IsValid())
			{
				return;
			}

			InjectTrialMessage("There is no (valid) uWebshop license found. Please visit <a href='http://www.uwebshop.com' target='_blank'>uWebshop.com</a> for more information", page);
		}


		/* HELPER TO INJECT TRIAL / ERROR MESSAGES INTO UMBRACO BACKOFFICE PAGES */

		public static void InjectTrialMessage(string trialMessage, Page page)
		{
			if (IsTrial())
			{
				injectMessage(trialMessage, "background:#FFF6BF;color:#514721;border-color:#FFD324; border-bottom: #FFD324 2px solid;", page);
			}
		}

		public static void InjectErrorMessage(string errorMessage, Page page)
		{
			if (getValidator().IsInvalid())
			{
				injectMessage(errorMessage, "background:#FFF6BF;color:red;border-color:red;", page);
			}
		}

		/*
         * Private helpers to help inject trial messages into asp.net pages control structure
         */

		private static void injectMessage(string message, string style, Page page)
		{
			//creates a pre-style DIV
			var trialText = string.Format("<div style=\"{1} margin-bottom: 20px; padding: 15px; font-size: 12px; text-align: center;\">{0}</div>", message, style);

			//Tries to find a standard Tabview
			var tabView = FindControlRecursive(page, "TabView1");
			if (tabView != null)
			{
				var tv = (TabView) tabView;
				foreach (TabPage p in tv.GetPanels())
				{
					var trialNotice = new LiteralControl(trialText) {ID = "trialMessage", ClientIDMode = ClientIDMode.Static};

					if (p.FindControl(trialNotice.ID) == null && page.FindControl(trialNotice.ID) == null)
					{
						p.Controls.AddAt(0, trialNotice);
					}
				}
			}
			else
				//tries to find a Pane
			{
				var trialNotice = new LiteralControl(trialText) {ID = "trialMessage", ClientIDMode = ClientIDMode.Static};
				var defaultPane = FindControlRecursive(page, "Panel1");
				if (defaultPane != null)
				{
					if (defaultPane.FindControl(trialNotice.ID) == null && page.FindControl(trialNotice.ID) == null)
					{
						defaultPane.Controls.AddAt(0, trialNotice);
					}
				}
				else
					//simply places the control at the beginning of the page
				{
					if (page.Form != null && page.FindControl(trialNotice.ID) == null)
					{
						page.Form.Controls.AddAt(0, trialNotice);
					}
				}
			}
		}

		private static Control FindControlRecursive(Control Root, string Id)
		{
			return Root.ID == Id ? Root : (from Control ctl in Root.Controls select FindControlRecursive(ctl, Id)).FirstOrDefault(FoundCtl => FoundCtl != null);
		}


		/*
         * THIS IS THE ACTUAL UMBRACO.LICENSING VALIDATOR, BASED ON PRODUCT NAME AND LICENSE PARAMETERS
         * DO NOT CHANGE THIS
         */
		private static UmbracoLicense.Validator _validator;

		public static UmbracoLicense.Validator getValidator()
		{
			return _validator ?? (_validator = new UmbracoLicense.Validator(LICENSE_PRODUCTNAME, LICENSE_PARAMETERS));
		}


		/*
         * THIS IS THE ACTUAL UMBRACO.LICENSING WEBSERVICE, BASED ON PRODUCT NAME AND LICENSE PARAMETERS
         * DO NOT CHANGE THIS
         */
		private static UmbracoLicense.Webservices.Licensing _licenseService;

		public static UmbracoLicense.Webservices.Licensing getLicenseService()
		{
			return _licenseService ?? (_licenseService = new UmbracoLicense.Webservices.Licensing {Url = "http://deli.umbraco.com/webservices/licensing.asmx"});
		}
	}

	//standard asp.net page
	public class LicensedPage : Page
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			if (Licensing.IsInvalid())
			{
				Licensing.InjectErrorMessage(string.Format("This license is invalid. {0}\n\nYour license has been approved for the following IPs/Domains:\n\n{1}", Licensing.LICENSE_CONTACTINFO, Licensing.getValidator().GetLicenseLimitations()).Replace("\n", "</br>"), base.Page);
			}
			else
			{
				//Inject a message to trial users
				Licensing.InjectTrialMessage("Your license is a trial, please contact: " + Licensing.LICENSE_CONTACTINFO, base.Page);
			}
		}
	}


	//This is a standard backoffice editing page, protected with umbraco user authentication
	public class LicensedEnsuredPage : umbraco.BasePages.UmbracoEnsuredPage
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			if (Licensing.IsInvalid())
			{
				Licensing.InjectErrorMessage(string.Format("This license is invalid. {0}\n\nYour license has been approved for the following IPs/Domains:\n\n{1}", Licensing.LICENSE_CONTACTINFO, Licensing.getValidator().GetLicenseLimitations()).Replace("\n", "</br>"), base.Page);
			}
			else
			{
				//Inject a message to trial users
				Licensing.InjectTrialMessage("Your license is a trial, please contact: " + Licensing.LICENSE_CONTACTINFO, base.Page);
			}
		}
	}

	//standard asp.net usercontrol
	public class LicensedUsercontrol : UserControl
	{
		public LicensedUsercontrol()
		{
			//test if the license is invalid, this one will not trow an exception, but only show an error
			//you there have to limit access to functionality and APIs in other ways
			Licensing.InjectErrorMessage(string.Format("This license is invalid. {0}\n\nYour license has been approved for the following IPs/Domains:\n\n{1}", Licensing.LICENSE_CONTACTINFO, Licensing.getValidator().GetLicenseLimitations()).Replace("\n", "</br>"), (Page) HttpContext.Current.Handler);

			//Inject a message to trial users
			Licensing.InjectTrialMessage("Your license is a trial, please contact: " + Licensing.LICENSE_CONTACTINFO, (Page) HttpContext.Current.Handler);
		}
	}
}