using System.Web.Mvc;
using uWebshop.Domain.Interfaces;
using Umbraco.Web.Models;

namespace uWebshop.Domain.Helpers
{
	/// <summary>
	/// Used to render an Email from a view
	/// </summary>
	internal class RenderEmailController : Umbraco.Web.Mvc.RenderMvcController
	{
		public ActionResult Index(RenderModel model)
		{
			// simple helper controller
			return base.Index(model);
		}
	}
}