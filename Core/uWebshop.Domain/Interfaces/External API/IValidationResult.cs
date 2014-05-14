namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IValidationResult
	{
		/// <summary>
		/// Gets the unique identifier.
		/// </summary>
		/// <value>
		/// The unique identifier.
		/// </value>
		int Id { get; }
		/// <summary>
		/// Gets the alias.
		/// </summary>
		/// <value>
		/// The alias.
		/// </value>
		string Alias { get; }
		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>
		/// The alias.
		/// </value>
		string Name { get; }
		/// <summary>
		/// Gets the key.
		/// </summary>
		/// <value>
		/// The key.
		/// </value>
		string Key { get; }
		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <value>
		/// The value.
		/// </value>
		string Value { get; }
	}
}