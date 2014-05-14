namespace uWebshop.Domain
{
	public interface IVatCalculationStrategy
	{
		int WithVat(bool pricesAreIncludingVAT, int originalTotal, decimal vat, int summedParts);
		int WithoutVat(bool pricesAreIncludingVAT, int originalTotal, decimal vat, int summedParts);
		int Vat(bool pricesIncludingVat, int originalTotal, decimal vat, int summedParts);
	}
}