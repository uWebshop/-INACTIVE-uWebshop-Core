namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface ICMSDocumentTypeService
	{
		/// <summary>
		/// Gets the by alias.
		/// </summary>
		/// <param name="alias">The alias.</param>
		/// <returns></returns>
		IDocumentTypeInfo GetByAlias(string alias);
	}
}