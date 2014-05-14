using System;
using System.Collections.Generic;
using System.Linq;
using uWebshop.Common;
using uWebshop.Domain.BaseClasses;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain
{
	/// <summary>
	///     Class representing a payment provider in Umbraco
	/// </summary>
	public class DummyShippingProvider : uWebshopEntity, IShippingProvider
	{
		private List<ShippingProviderMethod> _ShippingMethods;

		/// <summary>
		///     Title of the dummy shippingprovider
		/// </summary>
		public string Title
		{
			get { return Node.GetMultiStoreItem("title") != null ? Node.GetMultiStoreItem("title").Value : Node.Name; }
		}

		/// <summary>
		///     Nodename of the dummy shippingprovider
		/// </summary>
		public string ShippingProviderNodeName
		{
			get { return Node.Name; }
		}

		/// <summary>
		///     Gets a list of payment methods
		/// </summary>
		public IEnumerable<ShippingProviderMethod> ShippingMethods
		{
			get
			{
				if (_ShippingMethods == null)
				{
					LoadShippingMethods();
				}

				return _ShippingMethods;
			}
			private set { _ShippingMethods = value.ToList(); }
		}

		/// <summary>
		///     Gets a list with the shipping provider types
		/// </summary>
		public ShippingProviderType ShippingProviderType
		{
			get
			{
				if (Node.GetProperty("providerType").Value != null)
				{
					string preValue = Node.GetProperty("providerType").Value.Replace(" ", "_");

					return (ShippingProviderType) Enum.Parse(typeof (ShippingProviderType), preValue);
				}

				return ShippingProviderType.Unknown;
			}
		}

		/// <summary>
		///     Initializes a new instance of the uWebshop.Domain.DummyShippingProvider class
		/// </summary>
		/// <param name="id">NodeId of the shipping provider</param>
		public DummyShippingProvider(int id) : base(id)
		{
		}

		private void LoadShippingMethods()
		{
			List<IShippingProvider> shippingProviders = ShippingProviderHelper.GetAllShippingProvidersIncludingCustomProviders();

			//if (shippingProviders == null || shippingProviders.Count <= 0) return;
			foreach (var shippingProvider in shippingProviders.Where(shippingProvider => shippingProvider.GetName() == Name))
			{
				_ShippingMethods = shippingProvider.GetAllShippingMethods(0).ToList();

				break;
			}
		}

		/// <summary>
		/// Gets the description.
		/// </summary>
		/// <value>
		/// The description.
		/// </value>
		public string Description
		{
			get { return "Dummy Shipping Provider"; }
		}

		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <returns></returns>
		public string GetName()
		{
			return Title;
		}

		/// <summary>
		/// Gets all shipping methods.
		/// </summary>
		/// <param name="id">The unique identifier.</param>
		/// <returns></returns>
		public IEnumerable<ShippingProviderMethod> GetAllShippingMethods(int id)
		{
			return Enumerable.Empty<ShippingProviderMethod>();
		}
	}
}