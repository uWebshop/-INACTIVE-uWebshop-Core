namespace uWebshop.Common.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICoupon
    {
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        /// <value>
        /// The unique identifier.
        /// </value>
        int DiscountId { get; }

        /// <summary>
        /// Gets or sets the coupon code.
        /// </summary>
        /// <value>
        /// The coupon code.
        /// </value>
        string CouponCode { get; }

        /// <summary>
        /// Gets or sets the number available.
        /// </summary>
        /// <value>
        /// The number available.
        /// </value>
        int NumberAvailable { get; }
    }
}