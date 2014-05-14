using uWebshop.Domain.Helpers;

namespace uWebshop.Domain
{
	class OverTotalVatCalculationStrategy : IVatCalculationStrategy
	{
		public int WithVat(bool pricesAreIncludingVAT, int originalTotal, decimal vat, int summedParts)
		{
			return pricesAreIncludingVAT ? originalTotal : VatCalculator.WithVat(originalTotal, vat);
		}

		public int WithoutVat(bool pricesAreIncludingVAT, int originalTotal, decimal vat, int summedParts)
		{
			return pricesAreIncludingVAT ? VatCalculator.WithoutVat(originalTotal, vat) : originalTotal;
		}

		public int Vat(bool pricesIncludingVat, int originalTotal, decimal vat, int summedParts)
		{
			return VatCalculator.VatAmountFromOriginal(pricesIncludingVat, originalTotal, vat);
		}
	}
}