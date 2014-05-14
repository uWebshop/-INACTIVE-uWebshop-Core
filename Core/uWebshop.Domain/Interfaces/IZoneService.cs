using System.Collections.Generic;

namespace uWebshop.Domain.Interfaces
{
	internal interface IZoneService
	{
		/// <summary>
		/// Returns all the payment zones
		/// </summary>
		/// <param name="localization">The localization.</param>
		/// <returns></returns>
		List<Zone> GetAllPaymentZones(ILocalization localization);

		/// <summary>
		/// Returns all the shipping zones
		/// </summary>
		/// <param name="localization">The localization.</param>
		/// <returns></returns>
		List<Zone> GetAllShippingZones(ILocalization localization);

		/// <summary>
		/// Returns the zone matching with the id or the fallback zone (all countries) when not found
		/// </summary>
		/// <param name="id">The unique identifier.</param>
		/// <param name="localization">The localization.</param>
		/// <returns></returns>
		Zone GetByIdOrFallbackZone(int id, ILocalization localization);

        List<Zone> GetFallBackZone(ILocalization localization);
	}
}