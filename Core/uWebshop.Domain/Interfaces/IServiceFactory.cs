namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IServiceFactory
	{
		/// <summary>
		/// Builds this instance.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		T Build<T>() where T : class;
	}
}