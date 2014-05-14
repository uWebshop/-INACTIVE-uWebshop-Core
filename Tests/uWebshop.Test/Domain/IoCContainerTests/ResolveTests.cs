using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using uWebshop.Domain;

namespace uWebshop.Test.Domain.IoCContainerTests
{
	[TestFixture]
	public class ResolveTests
	{
		private IoCContainer _container;

		[SetUp]
		public void Setup()
		{
			_container = new IoCContainer();
		}

		[ExpectedException(typeof (Exception))]
		[Test]
		public void Resolve_UnregisteredTypeRequested_ShouldThrowException()
		{
			_container.Resolve<ResolveTests>();
		}

		[Test]
		public void asb()
		{
			_container.SetDefaultServiceFactory(new MockServiceFactory());

			_container.Resolve<ResolveTests>();
		}

		[Test]
		public void Resolve_RegisteredTypeWithNoDependenciesRequested_ShouldGiveNewInstanceOfRegisteredImplementation()
		{
			_container.RegisterType<ILeafType, LeafType>();

			var actual = _container.Resolve<ILeafType>().GetType();

			Assert.AreEqual(typeof (LeafType), actual);
		}

		[ExpectedException(typeof (Exception))]
		[Test]
		public void Resolve_RegisteredTypeWithUnregisteredDependencyRequested_ShouldThrowException()
		{
			_container.RegisterType<INonLeafType, NonLeafType>();

			var actual = _container.Resolve<INonLeafType>().GetType();
		}

		private interface ILeafType
		{
		}

		private class LeafType : ILeafType
		{
		}

		private interface INonLeafType
		{
		}

		private class NonLeafType : INonLeafType
		{
			public NonLeafType(ILeafType dependency)
			{
			}
		}
	}
}