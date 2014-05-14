namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IDocumentProperty
	{
		/// <summary>
		/// Gets or sets the Name.
		/// </summary>
		/// <value>
		/// The alias.
		/// </value>
		string Name { get; }

		/// <summary>
		/// Gets or sets the alias.
		/// </summary>
		/// <value>
		/// The alias.
		/// </value>
		string Alias { get; }

		/// <summary>
		/// Gets or sets the validation regular expression.
		/// </summary>
		/// <value>
		/// The validation regular expression.
		/// </value>
		string ValidationRegularExpression { get; }

		/// <summary>
		/// Gets or sets a value indicating whether [mandatory].
		/// </summary>
		/// <value>
		///   <c>true</c> if [mandatory]; otherwise, <c>false</c>.
		/// </value>
		bool Mandatory { get; }
	}
}