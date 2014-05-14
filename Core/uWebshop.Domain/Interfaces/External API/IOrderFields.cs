namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IOrderFields
	{
		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="fieldName">Name of the field.</param>
		/// <returns></returns>
		T GetValue<T>(string fieldName);
	}
}
