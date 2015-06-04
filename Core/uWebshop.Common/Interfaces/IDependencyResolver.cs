namespace uWebshop.Common.Interfaces
{
	public interface IDependencyResolver
	{
		/// <summary>
		/// Resolves this instance.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		T Resolve<T>() where T : class;
	}
}