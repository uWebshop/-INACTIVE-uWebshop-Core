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
}