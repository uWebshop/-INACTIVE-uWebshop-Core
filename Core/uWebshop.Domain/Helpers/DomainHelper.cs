using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using uWebshop.API;
using uWebshop.Common;
using uWebshop.Domain.BaseClasses;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain.Helpers
{
	/// <summary>
	/// Helper class with domain related functions
	/// </summary>
	public class DomainHelper
	{
		/// <summary>
		/// Gets all categories.
		/// </summary>
		/// <param name="includeDisabled">if set to <c>true</c> include disabled categories.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static IEnumerable<ICategory> GetAllCategories(bool includeDisabled = false, string storeAlias = null, string currencyCode = null)
		{
			return IO.Container.Resolve<ICategoryService>().GetAll(StoreHelper.GetLocalizationOrCurrent(storeAlias, currencyCode), includeDisabled);
		}

		/// <summary>
		/// Returns a list of all products
		/// </summary>
		/// <param name="includeDisabled">if set to <c>true</c> [include disabled].</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns>
		/// List of products
		/// </returns>
		public static IEnumerable<IProduct> GetAllProducts(bool includeDisabled = false, string storeAlias = null, string currencyCode = null)
		{
			return IO.Container.Resolve<IProductService>().GetAll(StoreHelper.GetLocalizationOrCurrent(storeAlias, currencyCode), includeDisabled);
		}

		/// <summary>
		/// Returns a list of all product variants
		/// </summary>
		/// <param name="includeDisabled">if set to <c>true</c> [include disabled].</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns>
		/// List of product variants
		/// </returns>
		public static IEnumerable<ProductVariant> GetAllProductVariants(bool includeDisabled = false, string storeAlias = null, string currencyCode = null)
		{
			return IO.Container.Resolve<IProductVariantService>().GetAll(StoreHelper.GetLocalizationOrCurrent(storeAlias, currencyCode), includeDisabled);
		}

		/// <summary>
		/// Returns a list of all products
		/// </summary>
		/// <returns>
		/// List of products
		/// </returns>
		public static IEnumerable<Store> GetAllStores()
		{
			return StoreHelper.StoreService.GetAllStores();
		}

		/// <summary>
		/// Returns a list of all products
		/// </summary>
		/// <param name="id">The unique identifier.</param>
		/// <returns>
		/// List of products
		/// </returns>
		public static Store StoreById(int id)
		{
			return StoreHelper.GetById(id);
		}

		/// <summary>
		/// Changes the store for current request.
		/// </summary>
		/// <param name="store">The store.</param>
		public static void ChangeStoreForCurrentRequest(Store store)
		{
			UwebshopRequest.Current.CurrentStore = store;
		}

		/// <summary>
		/// Get a category by it's NodeId
		/// </summary>
		/// <param name="categoryId">NodeId of the category</param>
		/// <returns>
		/// Category Object
		/// </returns>
		public static ICategory GetCategoryById(int categoryId)
		{
			return IO.Container.Resolve<ICategoryService>().GetById(categoryId, StoreHelper.CurrentLocalization);
		}

		/// <summary>
		/// Get a category by it's NodeId
		/// </summary>
		/// <param name="categoryId">NodeId of the category</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns>
		/// Category Object
		/// </returns>
		public static Category GetCategoryById(int categoryId, string storeAlias, string currencyCode = null)
		{
			return IO.Container.Resolve<ICategoryService>().GetById(categoryId, StoreHelper.GetLocalizationOrCurrent(storeAlias, currencyCode));
		}

		/// <summary>
		/// Gets the category with unique identifier.
		/// </summary>
		/// <param name="id">The unique identifier.</param>
		/// <returns></returns>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("use GetCategoryById")]
		public static ICategory GetCategoryWithId(int id)
		{
			return GetCategoryById(id);
		}

		/// <summary>
		/// Get random object by nodeTypeAlias
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="nodeTypeAlias">The node type alias.</param>
		/// <param name="count">The count.</param>
		/// <returns></returns>
		public static IEnumerable<T> GetRandomObjectsByAlias<T>(string nodeTypeAlias, int count)
		{
			var objects = GetObjectsByAlias<T>(nodeTypeAlias);

			return RandomizeGenericList(objects, count);
		}

		private static IEnumerable<T> RandomizeGenericList<T>(IEnumerable<T> list, int count)
		{
			var workList = list.ToList();
			var randomList = new List<T>();
			var random = new Random();

			while (workList.Count > 0)
			{
				int idx = random.Next(0, workList.Count);

				randomList.Add(workList[idx]);

				workList.RemoveAt(idx);
			}

			return randomList.Take(count);
		}

		/// <summary>
		/// Get object by nodeTypeAlias
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="nodeTypeAlias">The node type alias.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="startNodeId">The start node unique identifier.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static IEnumerable<T> GetObjectsByAlias<T>(string nodeTypeAlias, string storeAlias = null, int startNodeId = 0, string currencyCode = null)
		{
			return IO.Container.Resolve<ICMSEntityRepository>().GetObjectsByAlias<T>(nodeTypeAlias, StoreHelper.GetLocalizationOrCurrent(storeAlias, currencyCode), startNodeId);
		}

		/// <summary>
		/// Gets the products.
		/// </summary>
		/// <param name="categoryId">The category unique identifier.</param>
		/// <returns></returns>
		public static IEnumerable<IProduct> GetProducts(int categoryId)
		{
			return GetCategoryById(categoryId).Products;
		}

		internal static IEnumerable<int> ParseIntegersFromUwebshopProperty(string values)
		{
			if (!string.IsNullOrWhiteSpace(values) && !values.Any(char.IsLetter))
			{
				if (values.Contains(","))
				{
					return values.Split(',').Select(v => Common.Helpers.ParseInt(v));
				}
				if (values.Contains(" "))
				{
					return values.Split(' ').Select(v => Common.Helpers.ParseInt(v));
				}
				return new List<int> {Common.Helpers.ParseInt(values) };
			}
			return Enumerable.Empty<int>();
		}

		/// <summary>
		///     Convert the property to a multistore property usting the current store from the order or website
		/// </summary>
		/// <param name="propertyAlias"></param>
		/// <returns></returns>
		public static string MultiStorePropertyAlias(string propertyAlias)
		{
			var storeAlias = StoreHelper.GetCurrentStore().Alias;
			return !string.IsNullOrEmpty(storeAlias) ? StoreHelper.CreateMultiStorePropertyAlias(propertyAlias, storeAlias) : propertyAlias;
		}

		/// <summary>
		///     Serialize an object to XMLString
		/// </summary>
		/// <param name="obj"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static string SerializeObjectToXmlString<T>(T obj)
		{
			var currentThread = Thread.CurrentThread.CurrentCulture;
			var currentUIThread = Thread.CurrentThread.CurrentUICulture;

            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
			Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

			var xmls = new XmlSerializer(typeof (T));
            var xml = "";

            using (var sww = new StringWriterWithEncoding(Encoding.UTF8))
            {
                var settings = new XmlWriterSettings { Encoding = new UTF8Encoding(), Indent = true, IndentChars = "\t", NewLineChars = Environment.NewLine, ConformanceLevel = ConformanceLevel.Document };

                using (XmlWriter writer = XmlWriter.Create(sww,settings))
                {
                    xmls.Serialize(writer, obj);
                    xml = sww.ToString();
                }
            }

            Thread.CurrentThread.CurrentCulture = currentThread;
            Thread.CurrentThread.CurrentUICulture = currentUIThread;

            return xml;

        }

        public sealed class StringWriterWithEncoding : StringWriter
        {
            private readonly Encoding encoding;

            public StringWriterWithEncoding() { }

            public StringWriterWithEncoding(Encoding encoding)
            {
                this.encoding = encoding;
            }

            public override Encoding Encoding
            {
                get { return encoding; }
            }
        }

        /// <summary>
        /// Deserialize XMLstring to Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlString">The XML string.</param>
        /// <returns></returns>
        public static T DeserializeXmlStringToObject<T>(string xmlString)
		{
			var currentThread = Thread.CurrentThread.CurrentCulture;
			var currentUIThread = Thread.CurrentThread.CurrentUICulture;

			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
			Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

			var xmls = new XmlSerializer(typeof (T));

			using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(xmlString)))
			{
				var value = (T) xmls.Deserialize(ms);

				Thread.CurrentThread.CurrentCulture = currentThread;
				Thread.CurrentThread.CurrentUICulture = currentUIThread;

				return value;
			}
		}

		/// <summary>
		/// Gets the product by unique identifier.
		/// </summary>
		/// <param name="productId">The product unique identifier.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static IProduct GetProductById(int productId, string storeAlias = null, string currencyCode = null)
		{
            return IO.Container.Resolve<IProductService>().GetById(productId, StoreHelper.GetLocalizationOrCurrent(storeAlias, currencyCode));
		}

		/// <summary>
		/// Gets the product variant by unique identifier.
		/// </summary>
		/// <param name="productVariantGroupId">The product variant unique identifier.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static IProductVariantGroup GetProductVariantGroupById(int productVariantGroupId, string storeAlias = null, string currencyCode = null)
		{
			return IO.Container.Resolve<IProductVariantGroupService>().GetById(productVariantGroupId, StoreHelper.GetLocalizationOrCurrent(storeAlias, currencyCode));
		}

		/// <summary>
		/// Gets the product variant by unique identifier.
		/// </summary>
		/// <param name="productVariantId">The product variant unique identifier.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static ProductVariant GetProductVariantById(int productVariantId, string storeAlias = null, string currencyCode = null)
		{
			return IO.Container.Resolve<IProductVariantService>().GetById(productVariantId, StoreHelper.GetLocalizationOrCurrent(storeAlias, currencyCode));
		}

		internal static uWebshopEntity GetUwebShopNode()
		{
			return GetObjectsByAlias<uWebshopEntity>("uWebshop", Constants.NonMultiStoreAlias).FirstOrDefault();
		}

		internal static string BuildUrlFromTemplate(string productUrl, IUwebshopUmbracoEntity entity)
		{
			if (string.IsNullOrEmpty(productUrl)) return null;
			var productUrlPropertyAliasses = productUrl.Split(',');

			var urlFormat = string.Empty;
			foreach (var productUrlPropertyAlias in productUrlPropertyAliasses)
			{
				if (productUrlPropertyAlias.StartsWith("#"))
				{
					urlFormat += productUrlPropertyAlias.Skip(1);
				}
				else
				{
					urlFormat += StoreHelper.GetMultiStoreItem(entity.Id, productUrlPropertyAlias); // todo: this creates extra examine queries
				}
			}
			return urlFormat;
		}
	}
}