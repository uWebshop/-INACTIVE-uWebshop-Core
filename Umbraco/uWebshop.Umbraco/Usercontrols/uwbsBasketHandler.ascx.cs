using System;
using System.Linq;
using System.Threading;
using System.Web.UI;
using uWebshop.Domain;
using uWebshop.Domain.Businesslogic;
using System.Web;

namespace uWebshop.Web.Usercontrols
{
	public partial class UwbsBasketHandler : UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			//try
			//{	
				var redirectAfterHandle = new BasketRequestHandler().HandleBasketRequest(Request.Form, HttpContext.Current.Request.Url);
				if (redirectAfterHandle == null || !redirectAfterHandle.Any())
				{
					return;
				}

				string redirectUrl;
				var url = redirectAfterHandle.Last().Url;
				if (url == HttpContext.Current.Request.Url)
				{
					redirectUrl = HttpContext.Current.Request.RawUrl;
				}
				else
				{
					redirectUrl = url.AbsoluteUri;	
				}
				
				if ((Request.Form.AllKeys.Any() && (Request.Form.AllKeys.Any(x => x == "disableReload") || Request.Form.AllKeys.Any(x => x == "disableRedirect"))))
				{
					return;
				}

				var validateOrderReffererKey = Request.Form.AllKeys.FirstOrDefault(x => x.ToLower() == "backtoreferreronerror");
				if (validateOrderReffererKey != null && redirectAfterHandle.Any(x => x.Validated == false))
				{
					var validateOrderRefferer = Request.Form[validateOrderReffererKey];

					if ((validateOrderRefferer.ToLower() == "true" || validateOrderRefferer.ToLower() == "backtoreferreronerror" || validateOrderRefferer.ToLower() == "on" || validateOrderRefferer == "1") && HttpContext.Current.Request.UrlReferrer != null)
					{
						redirectUrl = redirectUrl.Replace(HttpContext.Current.Request.Url.AbsolutePath, HttpContext.Current.Request.UrlReferrer.AbsolutePath);
					}
				}

				Response.Redirect(redirectUrl, false);
				
			//}
			//catch (ThreadAbortException)
			//{
			//}
			////catch (Exception ex)
			//{
			//	//throw;
			//	//Log.Instance.LogError(ex, "Unhandled exception during execution of uWebshop basket handler");
			//}
		}
	}
}