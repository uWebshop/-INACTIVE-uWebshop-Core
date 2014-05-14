namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IAddress
	{
		/// <summary>
		/// Gets the first name.
		/// </summary>
		/// <value>
		/// The first name.
		/// </value>
		string FirstName { get; }
		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		string Name { get; }
		/// <summary>
		/// Gets the last name.
		/// </summary>
		/// <value>
		/// The last name.
		/// </value>
		string LastName { get; }
		/// <summary>
		/// Gets the address1.
		/// </summary>
		/// <value>
		/// The address1.
		/// </value>
		string Address1 { get; }
		/// <summary>
		/// Gets the address2.
		/// </summary>
		/// <value>
		/// The address2.
		/// </value>
		string Address2 { get; }
		/// <summary>
		/// Gets the street.
		/// </summary>
		/// <value>
		/// The street.
		/// </value>
		string Street { get; }
		/// <summary>
		/// Gets the street number.
		/// </summary>
		/// <value>
		/// The street number.
		/// </value>
		string StreetNumber { get; }
		/// <summary>
		/// Gets the street number addition.
		/// </summary>
		/// <value>
		/// The street number addition.
		/// </value>
		string StreetNumberAddition { get; }
		/// <summary>
		/// Gets the city.
		/// </summary>
		/// <value>
		/// The city.
		/// </value>
		string City { get; }
		/// <summary>
		/// Gets the country.
		/// </summary>
		/// <value>
		/// The country.
		/// </value>
		string Country { get; }
		/// <summary>
		/// Gets the country code.
		/// </summary>
		/// <value>
		/// The country code.
		/// </value>
		string CountryCode { get; }
		/// <summary>
		/// Gets the region.
		/// </summary>
		/// <value>
		/// The region.
		/// </value>
		string Region { get; }
		/// <summary>
		/// Gets the zip code.
		/// </summary>
		/// <value>
		/// The zip code.
		/// </value>
		string ZipCode { get; }
		/// <summary>
		/// Gets the company.
		/// </summary>
		/// <value>
		/// The company.
		/// </value>
		string Company { get; }
		/// <summary>
		/// Gets the phone.
		/// </summary>
		/// <value>
		/// The phone.
		/// </value>
		string Phone { get; }
		/// <summary>
		/// Gets the email.
		/// </summary>
		/// <value>
		/// The email.
		/// </value>
		string Email { get; }

		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="fieldName">Name of the field.</param>
		/// <param name="ignoreCustomerIsShipping">If customer == shipping address, by default fields from shipping return empty, set to true to return saved value</param>
		/// <returns></returns>
		T GetValue<T>(string fieldName, bool ignoreCustomerIsShipping = false);
	}
}