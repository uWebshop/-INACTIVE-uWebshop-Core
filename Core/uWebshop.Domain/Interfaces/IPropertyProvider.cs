namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IPropertyProvider
	{
		/// <summary>
		/// Determines whether the specified property contains key.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <returns></returns>
		bool ContainsKey(string property);

		/// <summary>
		/// Updates the value difference property present.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		bool UpdateValueIfPropertyPresent(string property, ref string value);

		/// <summary>
		/// Gets the string value.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <returns></returns>
		string GetStringValue(string property);
	}
}