namespace uWebshop.Domain
{
	/// <summary>
	/// 
	/// </summary>
	public class CustomSale
	{
		/// <summary>
		///     The discount percentage of the sale
		/// </summary>
		public int DiscountPercentage { get; set; }

		/// <summary>
		///     The discount amount of the sale
		/// </summary>
		public decimal DiscountAmount { get; set; }

		/// <summary>
		///     The title of the sale
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		///     The description of the sale
		/// </summary>
		public string Description { get; set; }
	}

	//public class CustomVariantSale
	//{
	//    /// <summary>
	//    /// The discountpercentage of the variants sale
	//    /// </summary>
	//    public int DiscountPercentage { get; set; }
	//    public decimal DiscountAmount { get; set; }

	//    public string Title { get; set; }
	//    public string Description { get; set; }
	//}
}