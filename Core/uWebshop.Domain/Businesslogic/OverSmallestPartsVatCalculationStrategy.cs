namespace uWebshop.Domain
{
	class OverSmallestPartsVatCalculationStrategy : IVatCalculationStrategy
	{
		public int WithVat(bool pricesAreIncludingVAT, int originalTotal, decimal vat, int summedParts)
		{
			return summedParts;
		}

		public int WithoutVat(bool pricesAreIncludingVAT, int originalTotal, decimal vat, int summedParts)
		{
			return summedParts;
		}

		public int Vat(bool pricesIncludingVat, int originalTotal, decimal vat, int summedParts)
		{
			return summedParts;
		}
	}
}