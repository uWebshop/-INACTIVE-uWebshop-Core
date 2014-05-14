using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using NUnit.Framework;
using uWebshop.API.XML;
using uWebshop.Common.Interfaces;
using uWebshop.Domain;
using uWebshop.Domain.Core;
using uWebshop.Domain.Interfaces;
using uWebshop.Umbraco;
using uWebshop.Umbraco.Repositories;

namespace uWebshop.Test.Domain.IoCContainerTests
{
	[TestFixture]
	public class DefaultTypeRegistrationTests
	{
		[Test]
		public void RegisterActualTypesAndResolveSomeKeyServices()
		{
			uWebshop.Domain.Core.Initialize.Reboot();
			(IO.Container as IoCContainer).RegisterType<ILoggingService, asfasf>();
			
			Console.WriteLine(Initialize.FinishedRegistrationLevel);
			Console.WriteLine(Initialize.FinishedInitializationLevel);
			IO.Container.Resolve<IUrlRewritingService>();
			IO.Container.Resolve<IProductService>();
			IO.Container.Resolve<IProductVariantService>();
		}
		class asfasf : ILoggingService
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

			uWebshop.Domain.Core.Initialize.Reboot();

			var dependencies = fakeContainer.TypeMap.Values.ToDictionary(type => type, type => type.GetConstructors().First().GetParameters().Select(p => p.ParameterType).ToList());

			foreach (var dependency in dependencies.OrderBy(k => k.Value.Count))
			{
				if (dependency.Value.Any())
					Console.WriteLine("" + dependency.Key.Name + " directly depends on " + string.Join(", ", dependency.Value.Select(p => p.Name)));
				else
					Console.WriteLine(dependency.Key.Name + " has no dependencies");
			}
			IO.Container = new IoCContainer();
		}
		
		[Test]
		public void asf()
		{
			var p = new UwebshopAliassesXMLConfig();
			//p.Product.ContentTypeAlias = "uwbsProduct";
			//p.Product.title = "titel";
			//p.Product.sku = "artikelnr";
			//var x = new System.Xml.Serialization.XmlSerializer(p.GetType());
			//x.Serialize(Console.Out, p);
		}
		
		[Test]
		public void XMldinges()
		{
			IOC.UnitTest();
			Basket.GetFulfillmentProviders();
		}

		//[Test]
		public void LoadingAssemblies()
		{
			Console.WriteLine(AppDomain.CurrentDomain.GetAssemblies().Count());
			AppDomain.CurrentDomain.GetAssemblies().OrderBy(a => a.FullName).ToList().ForEach(a => Console.WriteLine(a.FullName));
			Console.WriteLine();
			var targetType = typeof(IUwebshopAddon);
			var path = "C:/projects/UmbracoCms.6.1.4/bin";

			var files = Directory.GetFiles(path);
			var dlls = files.Select(filepath => new FileInfo(filepath)).Where(fileInfo => fileInfo.Name.StartsWith("uWebshop.") && fileInfo.Name.EndsWith(".dll")).ToList();
			if (dlls.Count == 0) throw new Exception("uWebshop dlls not found");
			var assemblies = dlls.Select(fileInfo => Assembly.LoadFrom(fileInfo.FullName)).Where(assembly => assembly != null).ToList();
			if (assemblies.Count == 0) throw new Exception("uWebshop dlls not loaded");
			Console.WriteLine(AppDomain.CurrentDomain.GetAssemblies().Count());
			AppDomain.CurrentDomain.GetAssemblies().OrderBy(a => a.FullName).ToList().ForEach(a => Console.WriteLine(a.FullName));

			var registeringServices = assemblies.SelectMany(assembly => assembly.GetTypes()).Where(type => targetType.IsAssignableFrom(type) && targetType != type).Select(type => (IUwebshopAddon)Activator.CreateInstance(type)).ToList();
	
			if (!registeringServices.Any()) throw new Exception("uWebshop Umbraco version specific dll's not found");
			if (registeringServices.Skip(1).Any()) throw new Exception("uWebshop Umbraco version specific dll's of both v4 and v6 found, please remove the incompatible dll");
			//registeringServices.First().Register(IO.Container);
		}

		private class FakeContainer : IIocContainer
		{
			public readonly Dictionary<Type, Type> TypeMap = new Dictionary<Type, Type>();

			public void RegisterType<T, T1>() where T1 : T
			{
				TypeMap.Add(typeof (T), typeof (T1));
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