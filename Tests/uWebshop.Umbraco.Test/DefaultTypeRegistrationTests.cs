using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using uWebshop.Common.Interfaces;
using uWebshop.Domain;
using uWebshop.Domain.Core;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Test.Domain.IoCContainerTests
{
	[TestFixture]
	public class DefaultTypeRegistrationTests
	{
		[Ignore]
		[Test]
		public void RegisterActualTypesAndResolveSomeKeyServices()
		{
			Initialize.Reboot();
			(IO.Container as IoCContainer).RegisterType<ILoggingService, StubLoggingService>();

			Console.WriteLine(Initialize.FinishedRegistrationLevel);
			Console.WriteLine(Initialize.FinishedInitializationLevel);
			IO.Container.Resolve<IUrlRewritingService>();
			IO.Container.Resolve<IProductService>();
			IO.Container.Resolve<IProductVariantService>();
		}
		class StubLoggingService : ILoggingService
		{
			public void LogError(Exception exception, string message = null)
			{
			}

			public void LogError(string message)
			{
			}

			public void LogWarning(string message)
			{
			}
            public void LogInfo(string message)
            {
            }

            public void LogDebug(string message)
			{
			}
		}

		[Ignore]
		[Test]
		public void ServicesDependencies()
		{
			var fakeContainer = new FakeContainer();
			IO.Container = fakeContainer;

			Initialize.Reboot();

			var dependencies = fakeContainer.TypeMap.Values.ToDictionary(type => type, type => type.GetConstructors().First().GetParameters().Select(p => p.ParameterType).ToList());

			foreach (var dependency in dependencies.OrderBy(k => k.Value.Count))
			{
				if (dependency.Value.Any())
				{
					Console.WriteLine("" + dependency.Key.Name + " directly depends on " + string.Join(", ", dependency.Value.Select(p => p.Name)));
				}
				else
				{
					Console.WriteLine(dependency.Key.Name + " has no dependencies");
				}
			}
			IO.Container = new IoCContainer();
		}

		private class FakeContainer : IIocContainer
		{
			public readonly Dictionary<Type, Type> TypeMap = new Dictionary<Type, Type>();

			public void RegisterType<T, T1>() where T1 : T
			{
				TypeMap.Add(typeof(T), typeof(T1));
			}

			public T Resolve<T>() where T : class
			{
				return null;
			}

			public void RegisterInstance<T, T1>(T1 instance) where T1 : T
			{
				throw new NotImplementedException();
			}

			public void SetDefaultServiceFactory(IServiceFactory serviceFactory)
			{
				throw new NotImplementedException();
			}
		}
	}
}