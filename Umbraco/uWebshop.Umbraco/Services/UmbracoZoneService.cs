using System;
using System.Collections.Generic;
using System.Linq;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Umbraco.Services
{
	// todo: move to Domain when possible
	internal class UmbracoZoneService : IZoneService
	{
		private readonly ICountryRepository _countryRepository;

		public UmbracoZoneService(ICountryRepository countryRepository)
		{
			_countryRepository = countryRepository;
		}

		/// <summary>
		/// Returns all the payment zones
		/// </summary>
		public List<Zone> GetAllPaymentZones(ILocalization localization)
		{
			var zoneNodes = IO.Container.Resolve<ICMSEntityRepository>().GetObjectsByAlias<Zone>(Zone.PaymentZoneNodeAlias, localization).ToList();
			return !zoneNodes.Any() ? GetFallBackZone(localization) : zoneNodes;
		}

		/// <summary>
		/// Returns all the shipping zones
		/// </summary>
		public List<Zone> GetAllShippingZones(ILocalization localization)
		{
			var zoneNodes = IO.Container.Resolve<ICMSEntityRepository>().GetObjectsByAlias<Zone>(Zone.ShippingZoneNodeAlias, localization).ToList();
			return !zoneNodes.Any() ? GetFallBackZone(localization) : zoneNodes;
		}

		public Zone GetByIdOrFallbackZone(int id, ILocalization localization)
		{
			return GetAllPaymentZones(localization).FirstOrDefault(z => z.Id == id) ?? GetAllShippingZones(localization).FirstOrDefault(z => z.Id == id) ?? FallBackZone(localization);
		}

		internal Zone FallBackZone(ILocalization localization)
		{
			return new Zone { CountryCodes = _countryRepository.GetAllCountries(localization).Select(c => c.Code).ToList(),
							  Id = -666,
							  CreateDate = DateTime.Now,
							  Name = "AllCountriesZone",
							  NodeTypeAlias = Zone.ShippingZoneNodeAlias,
							  ParentId = 0,
							  Path = "0,-666",
							  UpdateDate = DateTime.Now,
							  SortOrder = 0,
							  UrlName = "zone",
			};
		}

		public List<Zone> GetFallBackZone(ILocalization localization)
		{
			return new List<Zone> {FallBackZone(localization)};
		}
	}
}