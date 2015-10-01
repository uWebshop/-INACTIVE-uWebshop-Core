using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;
using Umbraco.Core.Macros;

namespace uWebshop.XSLTExtensions
{
	[XsltExtension("uWebshop.Catalog")]
	public class Catalog
	{
		// todo: cleanup (duplicates), remove BL

		public static string GetPropertyValueForCurrentShop(int nodeId, string propertyAlias)
		{
			return GetPropertyValueForCurrentShop(nodeId, propertyAlias, null);
		}

		/// <summary>
		/// Return the property value for the current shopalias
		/// </summary>
		/// <param name="nodeId"></param>
		/// <param name="propertyAlias"></param>
		/// <param name="storeAlias"></param>
		/// <returns></returns>
		public static string GetPropertyValueForCurrentShop(int nodeId, string propertyAlias, string storeAlias)
		{
			return StoreHelper.GetMultiStoreItem(nodeId, propertyAlias, storeAlias);
		}

		/// <summary>
		/// Returns unique products witch have one ore more tags in common with the given productId
		/// </summary>
		public static XPathNavigator GetMatchtingTagProducts(int productId)
		{
			var currentProduct = DomainHelper.GetProductById(productId);

			var productList = new List<Product>();

			var productService = IO.Container.Resolve<IProductService>();
			foreach (var p in from currentTag in currentProduct.Tags select productService.GetAll(StoreHelper.CurrentLocalization).Where(x => x.Tags.Contains(currentTag) && x.Id != productId) into matchedProducts from p in matchedProducts where !productList.Contains(p) select p)
			{
				productList.Add(p);
			}

			var stream1 = new MemoryStream();

			//Serialize the Record object to a memory stream using DataContractSerializer.
			var serializer = new DataContractSerializer(typeof (List<Product>));
			serializer.WriteObject(stream1, productList);
			stream1.Position = 0;

			var result = new StreamReader(stream1).ReadToEnd();

			var doc = new XmlDocument();
			doc.LoadXml(result);

			return doc.CreateNavigator();
		}

		/// <summary>
		/// Returns the full catalog including all categories, products and prices
		/// </summary>
		public static XPathNavigator GetCatalog()
		{
			//var catalogNode = DomainHelper.GetObjectsByAlias<umbraco.NodeFactory.Node>(Domain.Catalog.NodeAlias).FirstOrDefault();

			var categoryList = DomainHelper.GetAllCategories().Where(x => x.ParentNodeTypeAlias == Domain.Catalog.NodeAlias).ToList();

			var stream1 = new MemoryStream();

			//Serialize the Record object to a memory stream using DataContractSerializer.
			var serializer = new DataContractSerializer(typeof (List<Category>), "Categories", "");
			serializer.WriteObject(stream1, categoryList);
			stream1.Position = 0;
			var result = new StreamReader(stream1).ReadToEnd();

			var doc = new XmlDocument();
			doc.LoadXml(result);

			return doc.CreateNavigator();
		}

		/// <summary>
		/// Returns all the categories with products & prices
		/// </summary>
		public static XPathNavigator GetAllCategories()
		{
			var categoryList = DomainHelper.GetAllCategories().ToList();

			var stream1 = new MemoryStream();

			//Serialize the Record object to a memory stream using DataContractSerializer.
			var serializer = new DataContractSerializer(typeof (List<Category>), "Categories", "");
			serializer.WriteObject(stream1, categoryList);
			stream1.Position = 0;

			var result = new StreamReader(stream1).ReadToEnd();

			var doc = new XmlDocument();
			doc.LoadXml(result);


			return doc.CreateNavigator();
		}

		/// <summary>
		/// Returns all the products with prices
		/// </summary>
		public static XPathNavigator GetAllProducts()
		{
			var productList = IO.Container.Resolve<IProductService>().GetAll(StoreHelper.CurrentLocalization).ToList();

			var stream1 = new MemoryStream();

			//Serialize the Record object to a memory stream using DataContractSerializer.
			var serializer = new DataContractSerializer(typeof (List<Product>), "Products", "");
			serializer.WriteObject(stream1, productList);
			stream1.Position = 0;

			var result = new StreamReader(stream1).ReadToEnd();

			var doc = new XmlDocument();
			doc.LoadXml(result);


			return doc.CreateNavigator();
		}

		/// <summary>
		/// Returns all the pricingvariants
		/// </summary>
		public static XPathNavigator GetAllPricingVariants()
		{
			var pricingVariantList = DomainHelper.GetAllProductVariants().ToList();

			var stream1 = new MemoryStream();

			//Serialize the Record object to a memory stream using DataContractSerializer.
			var serializer = new DataContractSerializer(typeof (List<ProductVariant>), "PricingVariants", "");
			serializer.WriteObject(stream1, pricingVariantList);
			stream1.Position = 0;

			var result = new StreamReader(stream1).ReadToEnd();

			var doc = new XmlDocument();
			doc.LoadXml(result);


			return doc.CreateNavigator();
		}

		/// <summary>
		/// Returns the XML for a category item based on the URLname
		/// </summary>
		/// <param name="itemName">category</param>
		/// <param name="categoryUrlName"> </param>
		/// <returns></returns>
		public static XPathNavigator GetCategoryFromUrlName(string categoryUrlName)
		{
			// todo: move reference to IO to Domain.API
			var currentCategory = IO.Container.Resolve<ICatalogUrlResolvingService>().GetCategoryFromUrlName(categoryUrlName);

			return GetCategory(currentCategory.Id);
		}

		/// <summary>
		/// Returns the XML for a product item based on the URLname
		/// </summary>
		/// <param name="categoryUrlName"></param>
		/// <param name="productUrlName"></param>
		/// <returns></returns>
		public static XPathNavigator GetProductFromUrlName(string categoryUrlName, string productUrlName)
		{
			// todo: move reference to IO to Domain.API
			var currentProduct = IO.Container.Resolve<ICatalogUrlResolvingService>().GetProductFromUrlName(categoryUrlName, productUrlName);

			return GetProduct(currentProduct.Id);
		}

		/// <summary>
		/// Returns the catagory information for the given categoryId, with products & prices
		/// </summary>
		public static XPathNavigator GetCategory(int categoryId)
		{
			var category = DomainHelper.GetCategoryById(categoryId);

			var stream1 = new MemoryStream();

			//Serialize the Record object to a memory stream using DataContractSerializer.
			var serializer = new DataContractSerializer(typeof (Category));

			serializer.WriteObject(stream1, category);
			stream1.Position = 0;

			var result = new StreamReader(stream1).ReadToEnd();

			var doc = new XmlDocument();
			doc.LoadXml(result);


			return doc.CreateNavigator();
		}

		/// <summary>
		/// Returns the product information for the given productId, with prices
		/// </summary>
		public static XPathNavigator GetProduct(int productId)
		{
			var product = DomainHelper.GetProductById(productId);

			var stream1 = new MemoryStream();

			//Serialize the Record object to a memory stream using DataContractSerializer.
			var serializer = new DataContractSerializer(typeof (Product));
			serializer.WriteObject(stream1, product);
			stream1.Position = 0;

			var result = new StreamReader(stream1).ReadToEnd();

			var doc = new XmlDocument();
			doc.LoadXml(result);

			return doc.CreateNavigator();
		}

		/// <summary>
		/// Returns the pricingVariant information for the given pricingVariantId
		/// </summary>
		public static XPathNavigator GetPricingVariant(int pricingVariantId)
		{
			var pricingVariant = DomainHelper.GetProductVariantById(pricingVariantId);

			var stream1 = new MemoryStream();

			//Serialize the Record object to a memory stream using DataContractSerializer.
			var serializer = new DataContractSerializer(typeof (ProductVariant));
			serializer.WriteObject(stream1, pricingVariant);
			stream1.Position = 0;

			string result = new StreamReader(stream1).ReadToEnd();

			var doc = new XmlDocument();
			doc.LoadXml(result);

			return doc.CreateNavigator();
		}

		public static XPathNavigator GetAllShippingProviders()
		{
			var stream1 = new MemoryStream();

			//Serialize the Record object to a memory stream using DataContractSerializer.
			var serializer = new DataContractSerializer(typeof (List<ShippingProvider>), "ShippingProviders", "");
			serializer.WriteObject(stream1, ShippingProviderHelper.GetAllShippingProviders().ToList());
			stream1.Position = 0;

			string result = new StreamReader(stream1).ReadToEnd();

			var doc = new XmlDocument();
			doc.LoadXml(result);


			return doc.CreateNavigator();
		}

		/// <summary>
		/// Get all shipping providers for current order
		/// </summary>
		/// <returns></returns>
		public static XPathNavigator GetAllShippingProvidersForCurrentOrder()
		{
			var currentOrderInfo = OrderHelper.GetOrder();
			var doc = new XmlDocument();
			if (currentOrderInfo == null)
			{
				return doc.CreateNavigator();
			}

			var shippingProviderforOrderList = ShippingProviderHelper.GetShippingProvidersForOrder(currentOrderInfo);

			var stream1 = new MemoryStream();

			//Serialize the Record object to a memory stream using DataContractSerializer.
			var serializer = new DataContractSerializer(typeof (List<ShippingProvider>), "ShippingProviders", "");
			serializer.WriteObject(stream1, shippingProviderforOrderList);
			stream1.Position = 0;

			var result = new StreamReader(stream1).ReadToEnd();

			doc.LoadXml(result);

			return doc.CreateNavigator();
		}
	}
}