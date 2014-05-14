using uWebshop.Common.Interfaces;

namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	internal interface IIocContainer : IIocContainerConfiguration, IDependencyResolver
	{
		/// <summary>
		/// Sets the default service factory.
		/// </summary>
		/// <param name="serviceFactory">The service factory.</param>
		void SetDefaultServiceFactory(IServiceFactory serviceFactory);
	}

	internal interface IDependencyResolver
	{
		/// <summary>
		/// Resolves this instance.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		T Resolve<T>() where T : class;
	}
}