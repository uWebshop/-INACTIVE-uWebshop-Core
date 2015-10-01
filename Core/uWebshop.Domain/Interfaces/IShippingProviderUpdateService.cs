namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IShippingProviderUpdateService
	{
	    /// <summary>
	    /// Updates the specified shipping provider method.
	    /// </summary>
	    /// <param name="shippingProviderMethod">The shipping provider method.</param>
	    /// <param name="order"></param>
	    void Update(ShippingProviderMethod shippingProviderMethod, OrderInfo order);
	}
}