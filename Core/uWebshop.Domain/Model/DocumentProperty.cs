using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain.Businesslogic
{
	/// <summary>
	/// 
	/// </summary>
	public class DocumentProperty : IDocumentProperty
	{
		/// <summary>
		/// Gets or sets the Name.
		/// </summary>
		/// <value>
		/// The alias.
		/// </value>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the alias.
		/// </summary>
		/// <value>
		/// The alias.
		/// </value>
		public string Alias { get; set; }

		/// <summary>
		/// Gets or sets the validation regular expression.
		/// </summary>
		/// <value>
		/// The validation regular expression.
		/// </value>
		public string ValidationRegularExpression { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [mandatory].
		/// </summary>
		/// <value>
		///   <c>true</c> if [mandatory]; otherwise, <c>false</c>.
		/// </value>
		public bool Mandatory { get; set; }


		
	}
}