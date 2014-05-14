namespace uWebshop.Domain.Interfaces
{
	internal interface IUrlLocalizationService
	{
		string LocalizeCatalogUrl(string catalogUrl, ILocalization localization);
	}
}