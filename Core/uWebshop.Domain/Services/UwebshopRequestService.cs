using System.Web;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain.Services
{
	internal class UwebshopHttpContextRequestService : IUwebshopRequestService
	{
		public UwebshopRequest Current
		{
			get
			{
				const string cacheKey = "uwbsUwebshopRequest";
				var request = HttpContext.Current.Items[cacheKey] as UwebshopRequest;
				if (request == null)
				{
					request = new UwebshopRequest();
					HttpContext.Current.Items[cacheKey] = request;
				}
				return request;
			}
		}
	}
}