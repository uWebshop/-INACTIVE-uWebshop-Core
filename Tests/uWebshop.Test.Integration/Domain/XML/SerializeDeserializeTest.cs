using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using uWebshop.Common;
using uWebshop.DataAccess;
using uWebshop.DataAccess.Pocos;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;
using uWebshop.Test.Mocks;

namespace uWebshop.Test.Integration
{
	[TestFixture]
	public class SerializeDeserializeTest
	{
		[SetUp]
		public void Setup()
		{
			IOC.IntegrationTest();
		}
		[Test]
		public void ThatOriginalPriceInProductInfoRemainsTheSame()
		{
			IOC.SettingsService.ExclVat();
			var productInfo = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(100, 1);
			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateOrderInfo(productInfo);

			var serializedString = DomainHelper.SerializeObjectToXmlString(productInfo);
			var deserializedProductInfo = DomainHelper.DeserializeXmlStringToObject<ProductInfo>(serializedString);
			deserializedProductInfo.Order = orderInfo;
			deserializedProductInfo.Weight = 10;
			deserializedProductInfo.IsDiscounted = false;
			Console.WriteLine(deserializedProductInfo.PriceWithoutVatInCents);
			serializedString = DomainHelper.SerializeObjectToXmlString(productInfo);
			deserializedProductInfo = DomainHelper.DeserializeXmlStringToObject<ProductInfo>(serializedString);
			deserializedProductInfo.Order = orderInfo;
			deserializedProductInfo.IsDiscounted = false;
			Console.WriteLine(deserializedProductInfo.PriceWithoutVatInCents);

			Assert.AreEqual(productInfo.OriginalPriceInCents, deserializedProductInfo.OriginalPriceInCents);
			Assert.AreEqual(productInfo.ProductPriceWithoutVatInCents, deserializedProductInfo.ProductPriceWithoutVatInCents);
		}

		[Test]
		public void ThatReadingBackXMLDoenstChangeValuesFromNewXMLFromClientWithFailingGrandTotal()
		{
			IOC.SettingsService.InclVat();
			var deserializedOrderInfo = OrderInfo.CreateOrderInfoFromOrderData(new uWebshopOrderData() { OrderInfo = newXMLFromClientWithFailingGrandTotal, });
			Assert.NotNull(deserializedOrderInfo);

			Console.WriteLine(deserializedOrderInfo.VatCalculationStrategy.GetType().Name);

			var orderline = deserializedOrderInfo.OrderLines.First();

			Assert.AreEqual(16995 + 675, deserializedOrderInfo.GrandtotalInCents);
			Assert.AreEqual(16995, orderline.GrandTotalInCents);
			Assert.AreEqual(675, deserializedOrderInfo.ShippingProviderAmountInCents);
		}

		private string newXMLFromClientWithFailingGrandTotal = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Order xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
 <XMLVersion>100</XMLVersion>
 <PaidDate xsi:nil=""true"" />
 <ConfirmDate>2013-02-01T17:13:51.3074152+01:00</ConfirmDate>
 <IncludingVAT>true</IncludingVAT>
 <VATCharged>true</VATCharged>
 <PaymentProviderPrice>0</PaymentProviderPrice>
 <ShippingProviderPrice>675</ShippingProviderPrice>
 <RegionalVatAmount>0</RegionalVatAmount>
 <CouponCodes />
 <CorrespondingOrderDocumentId>0</CorrespondingOrderDocumentId>
 <OrderLines>
  <OrderLine>
   <OriginalProductId>9415</OriginalProductId>
   <Quantity>1</Quantity>
   <Title>Cambio 7/8 broek Rihanna luipaardprint</Title>
   <SKU>2811.21.94</SKU>
   <Weight>0</Weight>
   <Length>0</Length>
   <Height>0</Height>
   <Width>0</Width>
   <OriginalPrice>16995</OriginalPrice>
   <RangesString>0|0|0</RangesString>
   <Vat>21</Vat>
   <ImageId>0</ImageId>
   <DiscountAmount>0</DiscountAmount>
   <DiscountPercentage>0</DiscountPercentage>
   <ProductVariants>
	<ProductVariantInfo>
	 <Id>9443</Id>
	 <Title>40</Title>
	 <ChangedOn>2013-02-01T17:13:23.8064887+01:00</ChangedOn>
	 <Group />
	 <Weight>0</Weight>
	 <Length>0</Length>
	 <Height>0</Height>
	 <Width>0</Width>
	 <PriceInCents>0</PriceInCents>
	 <Vat>21</Vat>
	 <PriceWithVatInCents>0</PriceWithVatInCents>
	 <PriceWithoutVatInCents>0</PriceWithoutVatInCents>
	 <PriceWithoutVat>0</PriceWithoutVat>
	 <PriceWithVat>0</PriceWithVat>
	</ProductVariantInfo>
   </ProductVariants>
  </OrderLine>
 </OrderLines>
 <CustomerInfo>
  <CustomerId>0</CustomerId>
  <CountryCode>NL</CountryCode>
  <CountryName>Netherlands</CountryName>
  <ShippingCountryCode>NL</ShippingCountryCode>
  <ShippingCountryName>Netherlands</ShippingCountryName>
  <RegionName />
  <CustomerInformation>
   <Customer>
	<customerEmail><![CDATA[g.vandiggelen@wvdmedia.nl]]></customerEmail>
	<customerGender><![CDATA[M]]></customerGender>
	<customerFirstName><![CDATA[Gerben]]></customerFirstName>
	<customerLastName><![CDATA[Diggelen]]></customerLastName>
	<customerStreet><![CDATA[Klardendalseweg]]></customerStreet>
	<customerStreetNumber><![CDATA[11]]></customerStreetNumber>
	<customerPostalCode><![CDATA[6822GC]]></customerPostalCode>
	<customerCity><![CDATA[Arnhem]]></customerCity>
	<customerBirthdayDay><![CDATA[4]]></customerBirthdayDay>
	<customerBirthdayMonth><![CDATA[2]]></customerBirthdayMonth>
	<customerBirthdayYear><![CDATA[1996]]></customerBirthdayYear>
	<customerPhone><![CDATA[1624734869]]></customerPhone>
	<customerCountry><![CDATA[NL]]></customerCountry>
   </Customer>
  </CustomerInformation>
  <ShippingInformation>
   <Shipping>
	<shippingSameAddress><![CDATA[1]]></shippingSameAddress>
   </Shipping>
  </ShippingInformation>
 </CustomerInfo>
 <ShippingInfo>
  <Id>1250</Id>
  <Title>Bezorging per pakketpost</Title>
  <MethodId>1252</MethodId>
  <MethodTitle>Bezorging per pakketpost </MethodTitle>
  <TransactionMethod>QueryString</TransactionMethod>
  <ShippingType>Shipping</ShippingType>
 </ShippingInfo>
 <PaymentInfo>
  <Id>1307</Id>
  <Title>Mollie</Title>
  <MethodId>0761</MethodId>
  <MethodTitle>ASN Bank</MethodTitle>
  <TransactionMethod>QueryString</TransactionMethod>
  <PaymentType>OnlinePayment</PaymentType>
 </PaymentInfo>
 <StoreInfo>
  <LanguageCode>nl-NL</LanguageCode>
  <Alias>store</Alias>
  <CountryCode>NL</CountryCode>
  <Culture>nl-NL</Culture>
  <CurrencyCulture>nl-NL</CurrencyCulture>
 </StoreInfo>
 <Discounts />
 <ChargedAmount>17670</ChargedAmount>
</Order>";
	}

	[TestFixture]
	[Ignore]
	public class SerializeDeserializePre21CompatibilityTest
	{
		private Mock<IOrderService> orderHelperMock;

		[SetUp]
		public void Setup()
		{
			IOC.UnitTest();
			orderHelperMock = new Mock<IOrderService>();
		}

		[Test]
		public void ThatReadingBackXMLDoenstChangeValuesFromV21()
		{
			IOC.SettingsService.InclVat();
			IOC.VatCalculationStrategy.OverTotal();

			//var deserializedOrderInfo = DomainHelper.DeserializeXmlStringToObject<OrderInfo>(oldOrderXMLV21);
			var deserializedOrderInfo = OrderInfo.CreateOrderInfoFromOrderData(new uWebshopOrderData() { OrderInfo = oldOrderXMLV21 });
			Assert.NotNull(deserializedOrderInfo);

			Assert.AreEqual(11355, deserializedOrderInfo.SubtotalInCents);
			Assert.AreEqual(10736, deserializedOrderInfo.OrderLineTotalWithoutVatInCents);
			Assert.AreEqual(13740, deserializedOrderInfo.GrandtotalInCents);
			Assert.AreEqual(12990, deserializedOrderInfo.OrderLineTotalInCents);
		}

		private readonly string oldOrderXMLV21 = @"<?xml version=""1.0"" encoding=""utf-8""?>
<OrderInfo xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
	<OrderNumber>0061</OrderNumber>
	<OrderDate>dinsdag 15 januari 2013 13:05</OrderDate>
	<UniqueOrderId>d43cc01a-f21d-43ff-914b-3a72dc956d29</UniqueOrderId>
	<Paid xsi:nil=""true"" />
	<OrderLines>
		<OrderLine>
			<ProductInfo>
				<Id>1208</Id>
				<ProductVariants>
					<ProductVariantInfo>
						<Id>1538</Id>
						<Title>M</Title>
						<ChangedOn>2013-01-15T13:04:43.7265198+01:00</ChangedOn>
						<Group />
						<Weight>0</Weight>
						<Length>0</Length>
						<Height>0</Height>
						<Width>0</Width>
						<PriceInCents>0</PriceInCents>
						<Vat>21</Vat>
						<PriceWithVatInCents>0</PriceWithVatInCents>
						<PriceWithoutVatInCents>0</PriceWithoutVatInCents>
						<PriceWithoutVat>0</PriceWithoutVat>
						<PriceWithVat>0</PriceWithVat>
					</ProductVariantInfo>
				</ProductVariants>
				<Title>Blouse Hugo Boss</Title>
				<ChangedOn>2013-01-15T13:04:43.7265198+01:00</ChangedOn>
				<SKU>52112.90.229</SKU>
				<Weight>0</Weight>
				<Length>0</Length>
				<Height>0</Height>
				<Width>0</Width>
				<OriginalPriceInCents>12990</OriginalPriceInCents>
				<Vat>21</Vat>
				<Text />
				<ImageId>0</ImageId>
				<DiscountAmountInCents>0</DiscountAmountInCents>
				<DiscountPercentage>0</DiscountPercentage>
				<RangesString>0|0|0</RangesString>
				<ItemCount>1</ItemCount>
				<ProductRangePriceInCents>12990</ProductRangePriceInCents>
				<Ranges>
					<Range>
						<From>0</From>
						<To>0</To>
						<ProductId>1208</ProductId>
						<PriceInCents>0</PriceInCents>
						<Vat>21</Vat>
						<DiscountedPriceInCents>0</DiscountedPriceInCents>
					</Range>
				</Ranges>
				<IsDiscounted>false</IsDiscounted>
				<PriceInCents>12990</PriceInCents>
				<VatAmount>22.54</VatAmount>
				<ProductPriceWithVat>129.9</ProductPriceWithVat>
				<ProductPriceWithoutVat>107.36</ProductPriceWithoutVat>
				<ProductRangePriceWithoutVat>107.36</ProductRangePriceWithoutVat>
				<ProductRangePriceWithVat>129.9</ProductRangePriceWithVat>
				<DiscountedPriceWithVat>129.9</DiscountedPriceWithVat>
				<DiscountedPriceWithoutVat>107.36</DiscountedPriceWithoutVat>
				<DiscountedVat>22.54</DiscountedVat>
				<PriceWithVat>129.9</PriceWithVat>
				<PriceWithoutVat>107.36</PriceWithoutVat>
				<ProductPriceWithVatInCents>12990</ProductPriceWithVatInCents>
				<ProductPriceWithoutVatInCents>10736</ProductPriceWithoutVatInCents>
				<ProductRangePriceWithVatInCents>12990</ProductRangePriceWithVatInCents>
				<ProductRangePriceWithoutVatInCents>10736</ProductRangePriceWithoutVatInCents>
				<PriceWithVatInCents>12990</PriceWithVatInCents>
				<PriceWithoutVatInCents>10736</PriceWithoutVatInCents>
				<DiscountedVatInCents>2254</DiscountedVatInCents>
				<VatAmountInCents>2254</VatAmountInCents>
			</ProductInfo>
			<OrderLineId>1</OrderLineId>
			<OrderLineWeight>0</OrderLineWeight>
			<OrderLineAmountInCents>12990</OrderLineAmountInCents>
			<OrderDiscountInCents>0</OrderDiscountInCents>
			<OrderLineVat>21</OrderLineVat>
			<OrderLineVatAmountInCents>2254</OrderLineVatAmountInCents>
			<OrderLineVatAmountAfterOrderDiscountInCents>2254</OrderLineVatAmountAfterOrderDiscountInCents>
			<OrderLineSubTotal>107.36</OrderLineSubTotal>
			<OrderLineVatAmount>22.54</OrderLineVatAmount>
			<OrderLineGrandTotal>129.9</OrderLineGrandTotal>
			<OrderLineSubTotalInCents>10736</OrderLineSubTotalInCents>
			<OrderLineGrandTotalInCents>12990</OrderLineGrandTotalInCents>
		</OrderLine>
	</OrderLines>
	<CouponCodes />
	<CustomerInfo>
		<CustomerId>0</CustomerId>
		<CountryCode>NL</CountryCode>
		<CountryName>Netherlands</CountryName>
		<ShippingCountryCode>NL</ShippingCountryCode>
		<ShippingCountryName>Netherlands</ShippingCountryName>
		<RegionName />
		<CustomerInformation>
			<Customer>
				<customerEmail>g.vandiggelen@wvdmedia.nl</customerEmail>
				<customerGender><![CDATA[M]]></customerGender>
				<customerFirstName>Gerben</customerFirstName>
				<customerLastName>Diggelen</customerLastName>
				<customerStreet><![CDATA[Klardendalseweg]]></customerStreet>
				<customerStreetNumber><![CDATA[1111]]></customerStreetNumber>
				<customerPostalCode><![CDATA[6822GC]]></customerPostalCode>
				<customerCity><![CDATA[Arnhem]]></customerCity>
				<customerBirthdayDay><![CDATA[3]]></customerBirthdayDay>
				<customerBirthdayMonth><![CDATA[6]]></customerBirthdayMonth>
				<customerBirthdayYear><![CDATA[1994]]></customerBirthdayYear>
				<customerPhone><![CDATA[31787600266]]></customerPhone>
				<customerCountry>NL</customerCountry>
			</Customer>
		</CustomerInformation>
		<ShippingInformation>
			<Shipping>
				<shippingSameAddress><![CDATA[1]]></shippingSameAddress>
			</Shipping>
		</ShippingInformation>
	</CustomerInfo>
	<ShippingInfo>
		<Id>1250</Id>
		<Title>Bezorging per pakketpost</Title>
		<MethodId>1252</MethodId>
		<MethodTitle>Bezorging per pakketpost </MethodTitle>
		<TransactionMethod>QueryString</TransactionMethod>
		<ShippingType>Shipping</ShippingType>
	</ShippingInfo>
	<PaymentInfo>
		<Id>1326</Id>
		<Title>Mollie</Title>
		<MethodId>9999</MethodId>
		<MethodTitle>TBM Bank</MethodTitle>
		<TransactionMethod>QueryString</TransactionMethod>
		<PaymentType>OnlinePayment</PaymentType>
	</PaymentInfo>
	<StoreInfo>
		<LanguageCode>nl-NL</LanguageCode>
		<Alias>store</Alias>
		<CountryCode>NL</CountryCode>
		<Culture>nl-NL</Culture>
		<CurrencyCulture>nl-NL</CurrencyCulture>
	</StoreInfo>
	<ShippingProviderAmountInCents>750</ShippingProviderAmountInCents>
	<PaymentProviderPriceInCents>0</PaymentProviderPriceInCents>
	<OrderNodeId>2586</OrderNodeId>
	<ConfirmDate>2013-01-15T13:05:06.6835016+01:00</ConfirmDate>
	<Status>WaitingForPayment</Status>
	<ShippingCostsMightBeOutdated>true</ShippingCostsMightBeOutdated>
	<ShippingProviderVatAmountInCents>130</ShippingProviderVatAmountInCents>
	<PaymentProviderVatAmountInCents>0</PaymentProviderVatAmountInCents>
	<IsDiscounted>True</IsDiscounted>
	<OrderValidationErrors />
	<RegionalVatInCents>0</RegionalVatInCents>
	<TotalVatInCents>2385</TotalVatInCents>
	<OrderTotalInCents>13740</OrderTotalInCents>
	<GrandtotalWithoutVatInCents>11356</GrandtotalWithoutVatInCents>
	<GrandtotalInCents>13740</GrandtotalInCents>
	<VatTotal>2254</VatTotal>
	<OrderLineTotalInCents>12990</OrderLineTotalInCents>
	<CustomerEmail>g.vandiggelen@wvdmedia.nl</CustomerEmail>
	<CustomerLastName>Diggelen</CustomerLastName>
	<CustomerFirstName>Gerben</CustomerFirstName>
	<CustomerCountry>NL</CustomerCountry>
	<Grandtotal>137.4</Grandtotal>
	<OrderLineWithVatTotal>129.9</OrderLineWithVatTotal>
	<OrderLineWithoutVatTotal>107.36</OrderLineWithoutVatTotal>
	<GrandtotalWithoutVat>113.56</GrandtotalWithoutVat>
	<TotalVat>23.85</TotalVat>
	<ShippingProviderVatAmount>1.3</ShippingProviderVatAmount>
	<ShippingProviderCostsWithoutVat>6.2</ShippingProviderCostsWithoutVat>
	<ShippingProviderCostsWithVat>7.5</ShippingProviderCostsWithVat>
	<PaymentProviderVatAmount>0</PaymentProviderVatAmount>
	<PaymentProviderCostsWithoutVat>0</PaymentProviderCostsWithoutVat>
	<PaymentProviderCostsWithVat>0</PaymentProviderCostsWithVat>
	<DiscountAmount>0</DiscountAmount>
	<DiscountAmountWithVat>0</DiscountAmountWithVat>
	<DiscountAmountWithoutVat>0</DiscountAmountWithoutVat>
	<RegionalVat>0</RegionalVat>
	<Subtotal>113.55</Subtotal>
	<DiscountAmounWithVattInCents>0</DiscountAmounWithVattInCents>
	<DiscountAmountWithoutVatInCents>0</DiscountAmountWithoutVatInCents>
	<OrderLineTotalWithVatInCents>12990</OrderLineTotalWithVatInCents>
	<SubtotalInCents>11355</SubtotalInCents>
	<OrderLineTotalWithoutVatInCents>10736</OrderLineTotalWithoutVatInCents>
	<PaymentProviderCostsWithoutVatInCents>0</PaymentProviderCostsWithoutVatInCents>
	<PaymentProviderCostsWithVatInCents>0</PaymentProviderCostsWithVatInCents>
	<ShippingProviderCostsWithoutVatInCents>620</ShippingProviderCostsWithoutVatInCents>
	<ShippingProviderCostsWithVatInCents>750</ShippingProviderCostsWithVatInCents>
</OrderInfo>";

		[Test]
		public void ThatReadingBackXMLDoenstChangeValuesFrompreV21()
		{
			IOC.SettingsService.ExclVat();
			var deserializedOrderInfo = OrderInfo.CreateOrderInfoFromOrderData(new uWebshopOrderData() { OrderInfo = oldOrderXMLpreV21 });
			Assert.NotNull(deserializedOrderInfo);

			Assert.AreEqual(4000, deserializedOrderInfo.SubtotalInCents);
			Assert.AreEqual(4000, deserializedOrderInfo.OrderLineTotalWithoutVatInCents);
			Assert.AreEqual(4760, deserializedOrderInfo.GrandtotalInCents);
			Assert.AreEqual(4000, deserializedOrderInfo.OrderLineTotalInCents);
		}

		private readonly string oldOrderXMLpreV21 = @"<?xml version=""1.0"" encoding=""utf-8""?>
<OrderInfo xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
	<OrderNumber>0001</OrderNumber>
	<OrderDate>maandag 22 oktober 2012 16:19</OrderDate>
	<UniqueOrderId>4da2a6bd-31ef-42e1-b649-8427492869e6</UniqueOrderId>
	<OrderLines>
		<OrderLine>
			<ProductInfo>
				<Id>1186</Id>
				<ProductVariants />
				<Title>Flynn, Errol - The Life And Career</Title>
				<ChangedOn>2012-10-22T16:18:23.6638919+02:00</ChangedOn>
				<SKU>BK 9322</SKU>
				<Weight>0</Weight>
				<Length>0</Length>
				<Height>0</Height>
				<Width>0</Width>
				<OriginalPriceInCents>4000</OriginalPriceInCents>
				<ProductRangePriceInCents>4000</ProductRangePriceInCents>
				<Vat>19</Vat>
				<Text />
				<ImageId>0</ImageId>
				<ItemCount>1</ItemCount>
				<DiscountInCents>0</DiscountInCents>
				<DiscountedVatInCents>760</DiscountedVatInCents>
				<VatAmountInCents>760</VatAmountInCents>
				<VatAmountAfterOrderDiscountInCents>760</VatAmountAfterOrderDiscountInCents>
				<PriceInCents>4000</PriceInCents>
				<VatAmount>7.6</VatAmount>
				<ProductPriceWithVat>47.6</ProductPriceWithVat>
				<ProductPriceWithoutVat>40</ProductPriceWithoutVat>
				<ProductRangePriceWithoutVat>40</ProductRangePriceWithoutVat>
				<ProductRangePriceWithVat>47.6</ProductRangePriceWithVat>
				<DiscountedPriceWithVat>47.6</DiscountedPriceWithVat>
				<DiscountedPriceWithoutVat>40</DiscountedPriceWithoutVat>
				<DiscountedVat>7.6</DiscountedVat>
				<PriceWithVat>47.6</PriceWithVat>
				<PriceWithoutVat>40</PriceWithoutVat>
				<ProductPriceWithVatInCents>4760</ProductPriceWithVatInCents>
				<ProductPriceWithoutVatInCents>4000</ProductPriceWithoutVatInCents>
				<ProductRangePriceWithVatInCents>4760</ProductRangePriceWithVatInCents>
				<ProductRangePriceWithoutVatInCents>4000</ProductRangePriceWithoutVatInCents>
				<PriceWithVatInCents>4760</PriceWithVatInCents>
				<PriceWithoutVatInCents>4000</PriceWithoutVatInCents>
				<DiscountedPriceWithVatInCents>4760</DiscountedPriceWithVatInCents>
				<DiscountedPriceWithoutVatInCents>4000</DiscountedPriceWithoutVatInCents>
			</ProductInfo>
			<OrderlineWeight>0</OrderlineWeight>
			<OrderLineAmountInCents>4000</OrderLineAmountInCents>
			<OrderDiscountInCents>0</OrderDiscountInCents>
			<OrderLineVat>19</OrderLineVat>
			<OrderLineVatAmountInCents>760</OrderLineVatAmountInCents>
			<OrderLineVatAmountAfterOrderDiscountInCents>760</OrderLineVatAmountAfterOrderDiscountInCents>
			<OrderLineSubTotal>40</OrderLineSubTotal>
			<OrderLineVatAmount>7.6</OrderLineVatAmount>
			<OrderLineGrandTotal>47.6</OrderLineGrandTotal>
			<OrderLineSubTotalInCents>4000</OrderLineSubTotalInCents>
			<OrderLineGrandTotalInCents>4760</OrderLineGrandTotalInCents>
		</OrderLine>
	</OrderLines>
	<CouponCodes />
	<CustomerInfo>
		<customerId>0</customerId>
		<CountryCode>NL</CountryCode>
		<CountryName>Netherlands</CountryName>
		<ShippingCountryCode>NL</ShippingCountryCode>
		<ShippingCountryName>Netherlands</ShippingCountryName>
		<RegionName />
		<CustomerInformation>
			<Customer>
				<customerEmail><![CDATA[sasa@sadg5.nl]]></customerEmail>
			</Customer>
		</CustomerInformation>
	</CustomerInfo>
	<ShippingInfo>
		<Id>0</Id>
		<TransactionMethod>QueryString</TransactionMethod>
		<ShippingType>Unknown</ShippingType>
	</ShippingInfo>
	<PaymentInfo>
		<Id>0</Id>
		<TransactionMethod>QueryString</TransactionMethod>
		<PaymentType>Unknown</PaymentType>
	</PaymentInfo>
	<StoreInfo>
		<LanguageCode>en-US</LanguageCode>
		<Alias>TestRC8</Alias>
		<CountryCode>NL</CountryCode>
		<Culture>en-US</Culture>
		<CurrencyCulture>en-US</CurrencyCulture>
	</StoreInfo>
	<ShippingProviderAmountInCents>0</ShippingProviderAmountInCents>
	<PaymentProviderPriceInCents>0</PaymentProviderPriceInCents>
	<OrderNodeId>1707</OrderNodeId>
	<Status>Confirmed</Status>
	<ShippingCostsMightBeOutdated>true</ShippingCostsMightBeOutdated>
	<ShippingProviderVatAmountInCents>0</ShippingProviderVatAmountInCents>
	<PaymentProviderVatAmountInCents>0</PaymentProviderVatAmountInCents>
	<IsDiscounted>False</IsDiscounted>
	<OrderValidationErrors>
		<OrderValidationError>
			<DicionaryItem>Shipping_bla</DicionaryItem>
		</OrderValidationError>
	</OrderValidationErrors>
	<RegionalVatInCents>0</RegionalVatInCents>
	<TotalVatInCents>760</TotalVatInCents>
	<OrderTotalInCents>4000</OrderTotalInCents>
	<GrandtotalWithoutVatInCents>4000</GrandtotalWithoutVatInCents>
	<VatTotal>760</VatTotal>
	<OrderLineTotalInCents>4000</OrderLineTotalInCents>
	<Grandtotal>47.6</Grandtotal>
	<OrderLineWithVatTotal>47.6</OrderLineWithVatTotal>
	<OrderLineWithoutVatTotal>40</OrderLineWithoutVatTotal>
	<GrandtotalWithoutVat>40</GrandtotalWithoutVat>
	<TotalVat>7.6</TotalVat>
	<ShippingProviderVatAmount>0</ShippingProviderVatAmount>
	<ShippingProviderCostsWithoutVat>0</ShippingProviderCostsWithoutVat>
	<ShippingProviderCostsWithVat>0</ShippingProviderCostsWithVat>
	<PaymentProviderVatAmount>0</PaymentProviderVatAmount>
	<PaymentProviderCostsWithoutVat>0</PaymentProviderCostsWithoutVat>
	<PaymentProviderCostsWithVat>0</PaymentProviderCostsWithVat>
	<DiscountAmount>0</DiscountAmount>
	<RegionalVat>0</RegionalVat>
	<Subtotal>40</Subtotal>
	<OrderLineTotalWithVatInCents>4760</OrderLineTotalWithVatInCents>
	<SubtotalInCents>4000</SubtotalInCents>
	<GrandtotalInCents>4760</GrandtotalInCents>
	<OrderLineTotalWithoutVatInCents>4000</OrderLineTotalWithoutVatInCents>
	<PaymentProviderCostsWithoutVatInCents>0</PaymentProviderCostsWithoutVatInCents>
	<PaymentProviderCostsWithVatInCents>0</PaymentProviderCostsWithVatInCents>
	<ShippingProviderCostsWithoutVatInCents>0</ShippingProviderCostsWithoutVatInCents>
	<ShippingProviderCostsWithVatInCents>0</ShippingProviderCostsWithVatInCents>
</OrderInfo>";


		[Test]
		public void ThatReadingBackXMLDoenstChangeValuesFrompreV21vanKlant()
		{
			IOC.SettingsService.InclVat();
			var deserializedOrderInfo = OrderInfo.CreateOrderInfoFromOrderData(new uWebshopOrderData() { OrderInfo = oldOrderXMLpre21vanKlant });
			Assert.NotNull(deserializedOrderInfo);
			var orderline = deserializedOrderInfo.OrderLines.First();
			var product = orderline.ProductInfo;

			Assert.AreEqual(675, deserializedOrderInfo.ShippingProviderAmountInCents);
			Assert.AreEqual(8942, product.Id);
			Assert.AreEqual(28995, product.OriginalPriceInCents);
			Assert.AreEqual(14497, product.PriceInCents); // hmm   				<PriceInCents>14497</PriceInCents>
			//Assert.AreEqual(0, product.DiscountAmountInCents); => huh?!
#pragma warning disable 612,618
			Assert.AreEqual(28995, product.ProductRangePriceWithVatInCents); // sic!
#pragma warning restore 612,618
			Assert.AreEqual(1, product.ItemCount);

			Assert.AreEqual(14497, orderline.AmountInCents);
			Assert.AreEqual(11981, orderline.SubTotalInCents);
			Assert.AreEqual(14497, orderline.GrandTotalInCents);
			Assert.AreEqual(21, orderline.Vat);

			//Assert.AreEqual(0, deserializedOrderInfo.ShippingProviderCostsWithVatInCents); // dit gaat fout omdat de XML kapot is..
			//Assert.AreEqual(12539, deserializedOrderInfo.SubtotalInCents);
			//Assert.AreEqual(11981, deserializedOrderInfo.OrderLineTotalWithoutVatInCents);
			//Assert.AreEqual(15172, deserializedOrderInfo.GrandtotalInCents);
			//Assert.AreEqual(14497, deserializedOrderInfo.OrderLineTotalInCents);
		}

		private readonly string oldOrderXMLpre21vanKlant = @"<?xml version=""1.0"" encoding=""utf-8""?>
<OrderInfo xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
	<OrderNumber>0085</OrderNumber>
	<OrderDate>zondag 20 januari 2013 12:08</OrderDate>
	<UniqueOrderId>2f11dc6d-ea93-4b4b-9c56-c2ff5fdef526</UniqueOrderId>
	<OrderLines>
		<OrderLine>
			<ProductInfo>
				<Id>8942</Id>
				<ProductVariants>
					<ProductVariantInfo>
						<Id>8960</Id>
						<Title>38</Title>
						<ChangedOn>2013-01-20T12:02:33.487875+01:00</ChangedOn>
						<Group />
						<Weight>0</Weight>
						<Length>0</Length>
						<Height>0</Height>
						<Width>0</Width>
						<PriceInCents>0</PriceInCents>
						<Vat>21</Vat>
						<PriceWithVatInCents>0</PriceWithVatInCents>
						<PriceWithoutVatInCents>0</PriceWithoutVatInCents>
						<PriceWithoutVat>0</PriceWithoutVat>
						<PriceWithVat>0</PriceWithVat>
					</ProductVariantInfo>
				</ProductVariants>
				<Title>Goldbergh vest zwart</Title>
				<ChangedOn>2013-01-20T12:02:33.487875+01:00</ChangedOn>
				<SKU>2271.1.34</SKU>
				<Weight>0</Weight>
				<Length>0</Length>
				<Height>0</Height>
				<Width>0</Width>
				<OriginalPriceInCents>28995</OriginalPriceInCents>
				<ProductRangePriceInCents>28995</ProductRangePriceInCents>
				<Vat>21</Vat>
				<Text>0</Text>
				<ImageId>0</ImageId>
				<ItemCount>1</ItemCount>
				<DiscountInCents>0</DiscountInCents>
				<DiscountedVatInCents>2516</DiscountedVatInCents>
				<VatAmountInCents>2516</VatAmountInCents>
				<PriceInCents>14497</PriceInCents>
				<VatAmount>25.16</VatAmount>
				<ProductPriceWithVat>289.95</ProductPriceWithVat>
				<ProductPriceWithoutVat>239.63</ProductPriceWithoutVat>
				<ProductRangePriceWithoutVat>239.63</ProductRangePriceWithoutVat>
				<ProductRangePriceWithVat>289.95</ProductRangePriceWithVat>
				<DiscountedPriceWithVat>144.97</DiscountedPriceWithVat>
				<DiscountedPriceWithoutVat>119.81</DiscountedPriceWithoutVat>
				<DiscountedVat>25.16</DiscountedVat>
				<PriceWithVat>144.97</PriceWithVat>
				<PriceWithoutVat>119.81</PriceWithoutVat>
				<ProductPriceWithVatInCents>28995</ProductPriceWithVatInCents>
				<ProductPriceWithoutVatInCents>23963</ProductPriceWithoutVatInCents>
				<ProductRangePriceWithVatInCents>28995</ProductRangePriceWithVatInCents>
				<ProductRangePriceWithoutVatInCents>23963</ProductRangePriceWithoutVatInCents>
				<PriceWithVatInCents>14497</PriceWithVatInCents>
				<PriceWithoutVatInCents>11981</PriceWithoutVatInCents>
				<DiscountedPriceWithVatInCents>14497</DiscountedPriceWithVatInCents>
				<DiscountedPriceWithoutVatInCents>11981</DiscountedPriceWithoutVatInCents>
			</ProductInfo>
			<OrderlineWeight>0</OrderlineWeight>
			<OrderLineAmountInCents>14497</OrderLineAmountInCents>
			<OrderDiscountInCents>0</OrderDiscountInCents>
			<OrderLineVat>21</OrderLineVat>
			<OrderLineVatAmountInCents>2516</OrderLineVatAmountInCents>
			<OrderLineVatAmountAfterOrderDiscountInCents>2516</OrderLineVatAmountAfterOrderDiscountInCents>
			<OrderLineSubTotal>119.81</OrderLineSubTotal>
			<OrderLineVatAmount>25.16</OrderLineVatAmount>
			<OrderLineGrandTotal>144.97</OrderLineGrandTotal>
			<OrderLineSubTotalInCents>11981</OrderLineSubTotalInCents>
			<OrderLineGrandTotalInCents>14497</OrderLineGrandTotalInCents>
		</OrderLine>
	</OrderLines>
	<CouponCodes />
	<CustomerInfo>
		<CountryCode>NL</CountryCode>
		<CountryName>Netherlands</CountryName>
		<ShippingCountryCode>NL</ShippingCountryCode>
		<ShippingCountryName>Netherlands</ShippingCountryName>
		<RegionName />
		<CustomerInformation>
			<Customer>
				<customerEmail><![CDATA[N.provily@planet.nl]]></customerEmail>
				<customerGender><![CDATA[V]]></customerGender>
				<customerFirstName><![CDATA[Nel]]></customerFirstName>
				<customerLastName><![CDATA[Provily]]></customerLastName>
				<customerStreet><![CDATA[Ensahlaan]]></customerStreet>
				<customerStreetNumber><![CDATA[12]]></customerStreetNumber>
				<customerPostalCode><![CDATA[3723HV]]></customerPostalCode>
				<customerCity><![CDATA[Bilthoven]]></customerCity>
				<customerBirthdayDay><![CDATA[2]]></customerBirthdayDay>
				<customerBirthdayMonth><![CDATA[10]]></customerBirthdayMonth>
				<customerBirthdayYear><![CDATA[1958]]></customerBirthdayYear>
				<customerPhone><![CDATA[06 55751740]]></customerPhone>
				<customerCountry><![CDATA[NL]]></customerCountry>
			</Customer>
		</CustomerInformation>
		<ShippingInformation>
			<Shipping>
				<shippingSameAddress><![CDATA[1]]></shippingSameAddress>
			</Shipping>
		</ShippingInformation>
	</CustomerInfo>
	<ShippingInfo>
		<Id>1250</Id>
		<Title>Bezorging per pakketpost</Title>
		<MethodId>1252</MethodId>
		<MethodTitle>Bezorging per pakketpost </MethodTitle>
		<TransactionMethod>QueryString</TransactionMethod>
		<ShippingType>Shipping</ShippingType>
	</ShippingInfo>
	<PaymentInfo>
		<Id>1307</Id>
		<Title>Mollie</Title>
		<MethodId>0721</MethodId>
		<MethodTitle>ING</MethodTitle>
		<Url>https://ideal.ing.nl/internetbankieren/SesamLoginServlet?sessie=ideal&amp;trxid=0030000465229359&amp;random=d20553ec0fee227e</Url>
		<TransactionId>7d12107fbbda06a2f01b4633939e2af4</TransactionId>
		<TransactionMethod>QueryString</TransactionMethod>
		<PaymentType>OnlinePayment</PaymentType>
	</PaymentInfo>
	<StoreInfo>
		<LanguageCode>nl-NL</LanguageCode>
		<Alias>store</Alias>
		<CountryCode>NL</CountryCode>
		<Culture>nl-NL</Culture>
		<CurrencyCulture>nl-NL</CurrencyCulture>
	</StoreInfo>
	<ShippingProviderAmountInCents>675</ShippingProviderAmountInCents>
	<PaymentProviderPriceInCents>0</PaymentProviderPriceInCents>
	<OrderNodeId>8985</OrderNodeId>
	<Status>Confirmed</Status>
	<ShippingCostsMightBeOutdated>true</ShippingCostsMightBeOutdated>
	<ShippingProviderVatAmountInCents>0</ShippingProviderVatAmountInCents>
	<PaymentProviderVatAmountInCents>0</PaymentProviderVatAmountInCents>
	<IsDiscounted>True</IsDiscounted>
	<OrderValidationErrors />
	<RegionalVatInCents>0</RegionalVatInCents>
	<TotalVatInCents>2633</TotalVatInCents>
	<OrderTotalInCents>15172</OrderTotalInCents>
	<GrandtotalWithoutVatInCents>11981</GrandtotalWithoutVatInCents>
	<GrandtotalInCents>15172</GrandtotalInCents>
	<VatTotal>2516</VatTotal>
	<OrderLineTotalInCents>14497</OrderLineTotalInCents>
	<Grandtotal>151.72</Grandtotal>
	<OrderLineWithVatTotal>144.97</OrderLineWithVatTotal>
	<OrderLineWithoutVatTotal>119.81</OrderLineWithoutVatTotal>
	<GrandtotalWithoutVat>119.81</GrandtotalWithoutVat>
	<TotalVat>26.33</TotalVat>
	<ShippingProviderVatAmount>0</ShippingProviderVatAmount>
	<ShippingProviderCostsWithoutVat>0</ShippingProviderCostsWithoutVat>
	<ShippingProviderCostsWithVat>0</ShippingProviderCostsWithVat>
	<PaymentProviderVatAmount>0</PaymentProviderVatAmount>
	<PaymentProviderCostsWithoutVat>0</PaymentProviderCostsWithoutVat>
	<PaymentProviderCostsWithVat>0</PaymentProviderCostsWithVat>
	<DiscountAmount>0</DiscountAmount>
	<DiscountAmountWithVat>0</DiscountAmountWithVat>
	<DiscountAmountWithoutVat>0</DiscountAmountWithoutVat>
	<RegionalVat>0</RegionalVat>
	<Subtotal>125.39</Subtotal>
	<DiscountAmounWithVattInCents>0</DiscountAmounWithVattInCents>
	<DiscountAmountWithoutVatInCents>0</DiscountAmountWithoutVatInCents>
	<OrderLineTotalWithVatInCents>14497</OrderLineTotalWithVatInCents>
	<SubtotalInCents>12539</SubtotalInCents>
	<OrderLineTotalWithoutVatInCents>11981</OrderLineTotalWithoutVatInCents>
	<PaymentProviderCostsWithoutVatInCents>0</PaymentProviderCostsWithoutVatInCents>
	<PaymentProviderCostsWithVatInCents>0</PaymentProviderCostsWithVatInCents>
	<ShippingProviderCostsWithoutVatInCents>0</ShippingProviderCostsWithoutVatInCents>
	<ShippingProviderCostsWithVatInCents>0</ShippingProviderCostsWithVatInCents>
</OrderInfo>";

		//[Test]
		//public void ASDGASDG()
		//{
		//				//var allOrders = uWebshopOrders.GetAllOrderInfos().Select(OrderHelper.)

		//	return (IEnumerable<T>)uWebshopOrders.GetAllOrderInfos(/*filter*/).Select(OrderInfo.CreateOrderInfoFromOrderData);


		//	IReadRepository repository = new UwebshopDefaultRepository();
		//	var orders = repository.GetAll<OrderInfo>().Where(orderinfo => orderinfo != null);//.Select(orderInfo => new OrderData(orderInfo));

		//	foreach (var orderInfo in orders)
		//	{
		//		uWebshopOrders.StoreOrder(orderInfo.ToOrderData());
		//	}
		//}


		[Test]
		public void ThatReadingBackXMLDoenstChangeValuesFrompreV21FalendeDemoShop()
		{
			IOC.SettingsService.InclVat();
			var deserializedOrderInfo = OrderInfo.CreateOrderInfoFromOrderData(new uWebshopOrderData() { OrderInfo = xmlThatReadingBackXMLDoenstChangeValuesFrompreV21FalendeDemoShop });

			Assert.NotNull(deserializedOrderInfo);

			var orderline = deserializedOrderInfo.OrderLines.First();
			var product = orderline.ProductInfo;

			Assert.AreEqual(15600, deserializedOrderInfo.GrandtotalInCents);
			Assert.AreEqual(10600, orderline.GrandTotalInCents);
			Assert.AreEqual(5000, deserializedOrderInfo.ShippingProviderAmountInCents);
		}


		private string xmlThatReadingBackXMLDoenstChangeValuesFrompreV21FalendeDemoShop = @"<?xml version=""1.0"" encoding=""utf-8""?>
<OrderInfo xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
	<OrderNumber>ToyStore0001</OrderNumber>
	<OrderDate>vrijdag 11 januari 2013 13:40</OrderDate>
	<UniqueOrderId>4caeedf3-40fa-48ed-9634-15d296812e3f</UniqueOrderId>
	<Paid>true</Paid>
	<OrderLines>
		<OrderLine>
			<ProductInfo>
				<Id>1165</Id>
				<ProductVariants>
					<ProductVariantInfo>
						<Id>1678</Id>
						<Title>Blue</Title>
						<ChangedOn>2013-01-11T13:40:20.2174765+01:00</ChangedOn>
						<Group>Colors</Group>
						<Weight>0</Weight>
						<Length>0</Length>
						<Height>0</Height>
						<Width>0</Width>
						<PriceInCents>100</PriceInCents>
						<Vat>0</Vat>
						<PriceWithVatInCents>100</PriceWithVatInCents>
						<PriceWithoutVatInCents>100</PriceWithoutVatInCents>
						<PriceWithoutVat>1</PriceWithoutVat>
						<PriceWithVat>1</PriceWithVat>
					</ProductVariantInfo>
					<ProductVariantInfo>
						<Id>1948</Id>
						<Title>XL</Title>
						<ChangedOn>2013-01-11T13:40:20.2174765+01:00</ChangedOn>
						<Group>Size</Group>
						<Weight>0</Weight>
						<Length>0</Length>
						<Height>0</Height>
						<Width>0</Width>
						<PriceInCents>500</PriceInCents>
						<Vat>0</Vat>
						<PriceWithVatInCents>500</PriceWithVatInCents>
						<PriceWithoutVatInCents>500</PriceWithoutVatInCents>
						<PriceWithoutVat>5</PriceWithoutVat>
						<PriceWithVat>5</PriceWithVat>
					</ProductVariantInfo>
				</ProductVariants>
				<Title>Toy Boat</Title>
				<ChangedOn>2013-01-11T13:40:20.2174765+01:00</ChangedOn>
				<SKU>Toy Boat</SKU>
				<Weight>0</Weight>
				<Length>0</Length>
				<Height>0</Height>
				<Width>0</Width>
				<OriginalPriceInCents>10000</OriginalPriceInCents>
				<Vat>0</Vat>
				<Text />
				<ImageId>0</ImageId>
				<DiscountAmountInCents>0</DiscountAmountInCents>
				<DiscountPercentage>0</DiscountPercentage>
				<RangesString>0|0|0</RangesString>
				<ItemCount>1</ItemCount>
				<ProductRangePriceInCents>10600</ProductRangePriceInCents>
				<Ranges>
					<Range>
						<From>0</From>
						<To>0</To>
						<ProductId>1165</ProductId>
						<PriceInCents>0</PriceInCents>
						<Vat>0</Vat>
						<DiscountedPriceInCents>0</DiscountedPriceInCents>
					</Range>
				</Ranges>
				<IsDiscounted>false</IsDiscounted>
				<PriceInCents>10600</PriceInCents>
				<VatAmount>0</VatAmount>
				<ProductPriceWithVat>100</ProductPriceWithVat>
				<ProductPriceWithoutVat>100</ProductPriceWithoutVat>
				<ProductRangePriceWithoutVat>106</ProductRangePriceWithoutVat>
				<ProductRangePriceWithVat>106</ProductRangePriceWithVat>
				<DiscountedPriceWithVat>106</DiscountedPriceWithVat>
				<DiscountedPriceWithoutVat>106</DiscountedPriceWithoutVat>
				<DiscountedVat>0</DiscountedVat>
				<PriceWithVat>106</PriceWithVat>
				<PriceWithoutVat>106</PriceWithoutVat>
				<ProductPriceWithVatInCents>10000</ProductPriceWithVatInCents>
				<ProductPriceWithoutVatInCents>10000</ProductPriceWithoutVatInCents>
				<ProductRangePriceWithVatInCents>10600</ProductRangePriceWithVatInCents>
				<ProductRangePriceWithoutVatInCents>10600</ProductRangePriceWithoutVatInCents>
				<PriceWithVatInCents>10600</PriceWithVatInCents>
				<PriceWithoutVatInCents>10600</PriceWithoutVatInCents>
				<DiscountedVatInCents>0</DiscountedVatInCents>
				<VatAmountInCents>0</VatAmountInCents>
			</ProductInfo>
			<OrderLineId>0</OrderLineId>
			<OrderLineWeight>0</OrderLineWeight>
			<OrderLineAmountInCents>10600</OrderLineAmountInCents>
			<OrderDiscountInCents>0</OrderDiscountInCents>
			<OrderLineVat>0</OrderLineVat>
			<OrderLineVatAmountInCents>0</OrderLineVatAmountInCents>
			<OrderLineVatAmountAfterOrderDiscountInCents>0</OrderLineVatAmountAfterOrderDiscountInCents>
			<OrderLineSubTotal>106</OrderLineSubTotal>
			<OrderLineVatAmount>0</OrderLineVatAmount>
			<OrderLineGrandTotal>106</OrderLineGrandTotal>
			<OrderLineSubTotalInCents>10600</OrderLineSubTotalInCents>
			<OrderLineGrandTotalInCents>10600</OrderLineGrandTotalInCents>
		</OrderLine>
	</OrderLines>
	<CouponCodes />
	<CustomerInfo>
		<CustomerId>0</CustomerId>
		<CountryCode>AL</CountryCode>
		<CountryName>Albania</CountryName>
		<ShippingCountryCode>AL</ShippingCountryCode>
		<ShippingCountryName>Albania</ShippingCountryName>
		<RegionName />
		<CustomerInformation>
			<Customer>
				<customerEmail>arnold.visser@me.com</customerEmail>
				<customerFirstName>Arnold</customerFirstName>
				<customerLastName>Visser</customerLastName>
				<customerStreet><![CDATA[Potbeker 5]]></customerStreet>
				<customerPostalCode><![CDATA[5432 MK]]></customerPostalCode>
				<customerCity><![CDATA[Cuijk]]></customerCity>
				<customerCountry>AL</customerCountry>
			</Customer>
		</CustomerInformation>
		<ShippingInformation>
			<Shipping />
		</ShippingInformation>
		<ExtraInformation>
			<Extra />
		</ExtraInformation>
	</CustomerInfo>
	<ShippingInfo>
		<Id>1700</Id>
		<Title>TnT Fixed Fee</Title>
		<MethodId>1720</MethodId>
		<MethodTitle>Express</MethodTitle>
		<TransactionMethod>QueryString</TransactionMethod>
		<ShippingType>Pickup</ShippingType>
	</ShippingInfo>
	<PaymentInfo>
		<Id>1692</Id>
		<Title>iDealProfessional</Title>
		<MethodId>121</MethodId>
		<MethodTitle>Issuer Simulation</MethodTitle>
		<Url>https://abnamro-test.ideal-payment.de/ideal/issuerSim.do?trxid=0030000019423249&amp;ideal=prob</Url>
		<TransactionId>0030000019423249</TransactionId>
		<TransactionMethod>QueryString</TransactionMethod>
		<PaymentType>OnlinePayment</PaymentType>
	</PaymentInfo>
	<StoreInfo>
		<LanguageCode>en-US</LanguageCode>
		<Alias>Toystore</Alias>
		<CountryCode>NL</CountryCode>
		<Culture>en-US</Culture>
		<CurrencyCulture>nl-NL</CurrencyCulture>
	</StoreInfo>
	<ShippingProviderAmountInCents>5000</ShippingProviderAmountInCents>
	<PaymentProviderPriceInCents>0</PaymentProviderPriceInCents>
	<OrderNodeId>2168</OrderNodeId>
	<ConfirmDate>2013-01-11T13:40:34.240914+01:00</ConfirmDate>
	<Status>ReadyForDispatch</Status>
	<ShippingCostsMightBeOutdated>true</ShippingCostsMightBeOutdated>
	<ShippingProviderVatAmountInCents>0</ShippingProviderVatAmountInCents>
	<PaymentProviderVatAmountInCents>0</PaymentProviderVatAmountInCents>
	<DiscountAmountInCents>0</DiscountAmountInCents>
	<IsDiscounted>True</IsDiscounted>
	<OrderValidationErrors />
	<RegionalVatInCents>0</RegionalVatInCents>
	<TotalVatInCents>0</TotalVatInCents>
	<OrderTotalInCents>15600</OrderTotalInCents>
	<GrandtotalWithoutVatInCents>15600</GrandtotalWithoutVatInCents>
	<GrandtotalInCents>15600</GrandtotalInCents>
	<VatTotal>0</VatTotal>
	<OrderLineTotalInCents>10600</OrderLineTotalInCents>
	<CustomerEmail>arnold.visser@me.com</CustomerEmail>
	<CustomerLastName>Visser</CustomerLastName>
	<CustomerFirstName>Arnold</CustomerFirstName>
	<CustomerCountry>AL</CustomerCountry>
	<Grandtotal>156</Grandtotal>
	<OrderLineWithVatTotal>106</OrderLineWithVatTotal>
	<OrderLineWithoutVatTotal>106</OrderLineWithoutVatTotal>
	<GrandtotalWithoutVat>156</GrandtotalWithoutVat>
	<TotalVat>0</TotalVat>
	<ShippingProviderVatAmount>0</ShippingProviderVatAmount>
	<ShippingProviderCostsWithoutVat>50</ShippingProviderCostsWithoutVat>
	<ShippingProviderCostsWithVat>50</ShippingProviderCostsWithVat>
	<PaymentProviderVatAmount>0</PaymentProviderVatAmount>
	<PaymentProviderCostsWithoutVat>0</PaymentProviderCostsWithoutVat>
	<PaymentProviderCostsWithVat>0</PaymentProviderCostsWithVat>
	<DiscountAmount>0</DiscountAmount>
	<DiscountAmountWithVat>0</DiscountAmountWithVat>
	<DiscountAmountWithoutVat>0</DiscountAmountWithoutVat>
	<RegionalVat>0</RegionalVat>
	<Subtotal>156</Subtotal>
	<DiscountAmounWithVattInCents>0</DiscountAmounWithVattInCents>
	<DiscountAmountWithoutVatInCents>0</DiscountAmountWithoutVatInCents>
	<OrderLineTotalWithVatInCents>10600</OrderLineTotalWithVatInCents>
	<SubtotalInCents>15600</SubtotalInCents>
	<OrderLineTotalWithoutVatInCents>10600</OrderLineTotalWithoutVatInCents>
	<PaymentProviderCostsWithoutVatInCents>0</PaymentProviderCostsWithoutVatInCents>
	<PaymentProviderCostsWithVatInCents>0</PaymentProviderCostsWithVatInCents>
	<ShippingProviderCostsWithoutVatInCents>5000</ShippingProviderCostsWithoutVatInCents>
	<ShippingProviderCostsWithVatInCents>5000</ShippingProviderCostsWithVatInCents>
</OrderInfo>";


		[Test]
		public void RecreateBug20130117()
		{
			IOC.IntegrationTest();
			// not really clear what goes wrong, trying:
			//   create with order incomplete, get discount amount, set to readyfordispatch, serialize, deserialize, get discountamount

			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo(DefaultFactoriesAndSharedFunctionality.CreateProductInfo(995, 1));
			DefaultFactoriesAndSharedFunctionality.SetDiscountsOnOrderInfo(orderInfo, DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithAmount(100, DiscountOrderCondition.None, 0));
			Assert.AreEqual(100, orderInfo.DiscountAmountInCents);
			Assert.AreEqual(100, orderInfo.DiscountAmountWithVatInCents);
			Assert.AreEqual(895, orderInfo.OrderTotalInCents);

			orderInfo.Status = OrderStatus.ReadyForDispatch;

			var serializedString = DomainHelper.SerializeObjectToXmlString(orderInfo);
			var deserializedOrderInfo = OrderInfo.CreateOrderInfoFromOrderData(new uWebshopOrderData() { OrderInfo = serializedString });
			//var deserializedOrderInfo = DomainHelper.DeserializeXmlStringToObject<OrderInfo>(serializedString);

			Assert.AreEqual(100, deserializedOrderInfo.DiscountAmountInCents);
			Assert.AreEqual(100, deserializedOrderInfo.DiscountAmountWithVatInCents);
			Assert.AreEqual(895, deserializedOrderInfo.OrderTotalInCents);
		}

		[Test]
		public void ThatReadingBackXMLHandlesProductDiscountsAndProductVariantsCorrectly()
		{
			// ORDER IS ACTUALLY BROKEN!!! NOT A TRUE 2.0 ORDER
			IOC.SettingsService.InclVat();
			var deserializedOrderInfo = OrderInfo.CreateOrderInfoFromOrderData(new uWebshopOrderData() { OrderInfo = oldOrderXMLpreV21WithVariantsAndDiscount });
			Assert.NotNull(deserializedOrderInfo);


			var productInfo = deserializedOrderInfo.OrderLines.Single().ProductInfo;
			Assert.AreEqual(11990, productInfo.PriceInCents);
			Assert.AreEqual(12990, productInfo.ProductRangePriceInCents);
			Assert.AreEqual(1000, productInfo.DiscountAmountInCents);
			Assert.AreEqual(11990, deserializedOrderInfo.OrderLineTotalInCents);

			Assert.AreEqual(750, deserializedOrderInfo.ShippingProviderCostsWithVatInCents);

			//Assert.AreEqual(11355, deserializedOrderInfo.SubtotalInCents);
			Assert.AreEqual(12740, deserializedOrderInfo.GrandtotalInCents);
		}

		private readonly string oldOrderXMLpreV21WithVariantsAndDiscount = @"<?xml version=""1.0"" encoding=""utf-8""?>
<OrderInfo xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
	<OrderNumber>0061</OrderNumber>
	<OrderDate>dinsdag 15 januari 2013 13:05</OrderDate>
	<UniqueOrderId>d43cc01a-f21d-43ff-914b-3a72dc956d29</UniqueOrderId>
	<Paid xsi:nil=""true"" />
	<OrderLines>
		<OrderLine>
			<ProductInfo>
				<Id>1208</Id>
				<ProductVariants>
					<ProductVariantInfo>
						<Id>1678</Id>
						<Title>Blue</Title>
						<ChangedOn>2013-01-23T11:15:30.459664+01:00</ChangedOn>
						<Group>Colors</Group>
						<Weight>0</Weight>
						<Length>0</Length>
						<Height>0</Height>
						<Width>0</Width>
						<PriceInCents>100</PriceInCents>
						<Vat>0</Vat>
						<PriceWithVatInCents>100</PriceWithVatInCents>
						<PriceWithoutVatInCents>100</PriceWithoutVatInCents>
						<PriceWithoutVat>1</PriceWithoutVat>
						<PriceWithVat>1</PriceWithVat>
					</ProductVariantInfo>
					<ProductVariantInfo>
						<Id>1948</Id>
						<Title>XL</Title>
						<ChangedOn>2013-01-23T11:15:30.4655234+01:00</ChangedOn>
						<Group>Size</Group>
						<Weight>0</Weight>
						<Length>0</Length>
						<Height>0</Height>
						<Width>0</Width>
						<PriceInCents>500</PriceInCents>
						<Vat>0</Vat>
						<PriceWithVatInCents>500</PriceWithVatInCents>
						<PriceWithoutVatInCents>500</PriceWithoutVatInCents>
						<PriceWithoutVat>5</PriceWithoutVat>
						<PriceWithVat>5</PriceWithVat>
					</ProductVariantInfo>
				</ProductVariants>
				<Title>Blouse Hugo Boss</Title>
				<ChangedOn>2013-01-15T13:04:43.7265198+01:00</ChangedOn>
				<SKU>52112.90.229</SKU>
				<Weight>0</Weight>
				<Length>0</Length>
				<Height>0</Height>
				<Width>0</Width>
				<OriginalPriceInCents>12990</OriginalPriceInCents>
				<Vat>21</Vat>
				<Text />
				<ImageId>0</ImageId>
				<DiscountAmountInCents>0</DiscountAmountInCents>
				<DiscountPercentage>0</DiscountPercentage>
				<RangesString>0|0|0</RangesString>
				<ItemCount>1</ItemCount>
				<ProductRangePriceInCents>12990</ProductRangePriceInCents>
				<Ranges>
					<Range>
						<From>0</From>
						<To>0</To>
						<ProductId>1208</ProductId>
						<PriceInCents>0</PriceInCents>
						<Vat>21</Vat>
						<DiscountedPriceInCents>0</DiscountedPriceInCents>
					</Range>
				</Ranges>
				<IsDiscounted>false</IsDiscounted>
				<PriceInCents>11990</PriceInCents>
				<VatAmount>22.54</VatAmount>
				<ProductPriceWithVat>129.9</ProductPriceWithVat>
				<ProductPriceWithoutVat>107.36</ProductPriceWithoutVat>
				<ProductRangePriceWithoutVat>107.36</ProductRangePriceWithoutVat>
				<ProductRangePriceWithVat>129.9</ProductRangePriceWithVat>
				<DiscountedPriceWithVat>129.9</DiscountedPriceWithVat>
				<DiscountedPriceWithoutVat>107.36</DiscountedPriceWithoutVat>
				<DiscountedVat>22.54</DiscountedVat>
				<PriceWithVat>129.9</PriceWithVat>
				<PriceWithoutVat>107.36</PriceWithoutVat>
				<ProductPriceWithVatInCents>12990</ProductPriceWithVatInCents>
				<ProductPriceWithoutVatInCents>10736</ProductPriceWithoutVatInCents>
				<ProductRangePriceWithVatInCents>12990</ProductRangePriceWithVatInCents>
				<ProductRangePriceWithoutVatInCents>10736</ProductRangePriceWithoutVatInCents>
				<PriceWithVatInCents>12990</PriceWithVatInCents>
				<PriceWithoutVatInCents>10736</PriceWithoutVatInCents>
				<DiscountedVatInCents>2254</DiscountedVatInCents>
				<VatAmountInCents>2254</VatAmountInCents>
			</ProductInfo>
			<OrderLineId>1</OrderLineId>
			<OrderLineWeight>0</OrderLineWeight>
			<OrderLineAmountInCents>12990</OrderLineAmountInCents>
			<OrderDiscountInCents>0</OrderDiscountInCents>
			<OrderLineVat>21</OrderLineVat>
			<OrderLineVatAmountInCents>2254</OrderLineVatAmountInCents>
			<OrderLineVatAmountAfterOrderDiscountInCents>2254</OrderLineVatAmountAfterOrderDiscountInCents>
			<OrderLineSubTotal>107.36</OrderLineSubTotal>
			<OrderLineVatAmount>22.54</OrderLineVatAmount>
			<OrderLineGrandTotal>129.9</OrderLineGrandTotal>
			<OrderLineSubTotalInCents>10736</OrderLineSubTotalInCents>
			<OrderLineGrandTotalInCents>12990</OrderLineGrandTotalInCents>
		</OrderLine>
	</OrderLines>
	<CouponCodes />
	<CustomerInfo>
		<CustomerId>0</CustomerId>
		<CountryCode>NL</CountryCode>
		<CountryName>Netherlands</CountryName>
		<ShippingCountryCode>NL</ShippingCountryCode>
		<ShippingCountryName>Netherlands</ShippingCountryName>
		<RegionName />
		<CustomerInformation>
			<Customer>
				<customerEmail>g.vandiggelen@wvdmedia.nl</customerEmail>
				<customerGender><![CDATA[M]]></customerGender>
				<customerFirstName>Gerben</customerFirstName>
				<customerLastName>Diggelen</customerLastName>
				<customerStreet><![CDATA[Klardendalseweg]]></customerStreet>
				<customerStreetNumber><![CDATA[1111]]></customerStreetNumber>
				<customerPostalCode><![CDATA[6822GC]]></customerPostalCode>
				<customerCity><![CDATA[Arnhem]]></customerCity>
				<customerBirthdayDay><![CDATA[3]]></customerBirthdayDay>
				<customerBirthdayMonth><![CDATA[6]]></customerBirthdayMonth>
				<customerBirthdayYear><![CDATA[1994]]></customerBirthdayYear>
				<customerPhone><![CDATA[31787600266]]></customerPhone>
				<customerCountry>NL</customerCountry>
			</Customer>
		</CustomerInformation>
		<ShippingInformation>
			<Shipping>
				<shippingSameAddress><![CDATA[1]]></shippingSameAddress>
			</Shipping>
		</ShippingInformation>
	</CustomerInfo>
	<ShippingInfo>
		<Id>1250</Id>
		<Title>Bezorging per pakketpost</Title>
		<MethodId>1252</MethodId>
		<MethodTitle>Bezorging per pakketpost </MethodTitle>
		<TransactionMethod>QueryString</TransactionMethod>
		<ShippingType>Shipping</ShippingType>
	</ShippingInfo>
	<PaymentInfo>
		<Id>1326</Id>
		<Title>Mollie</Title>
		<MethodId>9999</MethodId>
		<MethodTitle>TBM Bank</MethodTitle>
		<TransactionMethod>QueryString</TransactionMethod>
		<PaymentType>OnlinePayment</PaymentType>
	</PaymentInfo>
	<StoreInfo>
		<LanguageCode>nl-NL</LanguageCode>
		<Alias>store</Alias>
		<CountryCode>NL</CountryCode>
		<Culture>nl-NL</Culture>
		<CurrencyCulture>nl-NL</CurrencyCulture>
	</StoreInfo>
	<ShippingProviderAmountInCents>750</ShippingProviderAmountInCents>
	<PaymentProviderPriceInCents>0</PaymentProviderPriceInCents>
	<OrderNodeId>2586</OrderNodeId>
	<ConfirmDate>2013-01-15T13:05:06.6835016+01:00</ConfirmDate>
	<Status>WaitingForPayment</Status>
	<ShippingCostsMightBeOutdated>true</ShippingCostsMightBeOutdated>
	<ShippingProviderVatAmountInCents>130</ShippingProviderVatAmountInCents>
	<PaymentProviderVatAmountInCents>0</PaymentProviderVatAmountInCents>
	<IsDiscounted>True</IsDiscounted>
	<OrderValidationErrors />
	<RegionalVatInCents>0</RegionalVatInCents>
	<TotalVatInCents>2385</TotalVatInCents>
	<OrderTotalInCents>13740</OrderTotalInCents>
	<GrandtotalWithoutVatInCents>11356</GrandtotalWithoutVatInCents>
	<GrandtotalInCents>13740</GrandtotalInCents>
	<VatTotal>2254</VatTotal>
	<OrderLineTotalInCents>12990</OrderLineTotalInCents>
	<CustomerEmail>g.vandiggelen@wvdmedia.nl</CustomerEmail>
	<CustomerLastName>Diggelen</CustomerLastName>
	<CustomerFirstName>Gerben</CustomerFirstName>
	<CustomerCountry>NL</CustomerCountry>
	<Grandtotal>137.4</Grandtotal>
	<OrderLineWithVatTotal>129.9</OrderLineWithVatTotal>
	<OrderLineWithoutVatTotal>107.36</OrderLineWithoutVatTotal>
	<GrandtotalWithoutVat>113.56</GrandtotalWithoutVat>
	<TotalVat>23.85</TotalVat>
	<ShippingProviderVatAmount>1.3</ShippingProviderVatAmount>
	<ShippingProviderCostsWithoutVat>6.2</ShippingProviderCostsWithoutVat>
	<ShippingProviderCostsWithVat>7.5</ShippingProviderCostsWithVat>
	<PaymentProviderVatAmount>0</PaymentProviderVatAmount>
	<PaymentProviderCostsWithoutVat>0</PaymentProviderCostsWithoutVat>
	<PaymentProviderCostsWithVat>0</PaymentProviderCostsWithVat>
	<DiscountAmount>0</DiscountAmount>
	<DiscountAmountWithVat>0</DiscountAmountWithVat>
	<DiscountAmountWithoutVat>0</DiscountAmountWithoutVat>
	<RegionalVat>0</RegionalVat>
	<Subtotal>113.55</Subtotal>
	<DiscountAmounWithVattInCents>0</DiscountAmounWithVattInCents>
	<DiscountAmountWithoutVatInCents>0</DiscountAmountWithoutVatInCents>
	<OrderLineTotalWithVatInCents>12990</OrderLineTotalWithVatInCents>
	<SubtotalInCents>11355</SubtotalInCents>
	<OrderLineTotalWithoutVatInCents>10736</OrderLineTotalWithoutVatInCents>
	<PaymentProviderCostsWithoutVatInCents>0</PaymentProviderCostsWithoutVatInCents>
	<PaymentProviderCostsWithVatInCents>0</PaymentProviderCostsWithVatInCents>
	<ShippingProviderCostsWithoutVatInCents>620</ShippingProviderCostsWithoutVatInCents>
	<ShippingProviderCostsWithVatInCents>750</ShippingProviderCostsWithVatInCents>
</OrderInfo>";


		[Test]
		public void ThatReadingBackXMLHandlesCouponDiscountCorrectly()
		{
			IOC.SettingsService.InclVat();
			IOC.OrderDiscountService.Actual();
			IOC.DiscountCalculationService.Actual();
			var deserializedOrderInfo = OrderInfo.CreateOrderInfoFromOrderData(new uWebshopOrderData() {OrderInfo = xmlWithFailingCouponDiscount});
			Assert.NotNull(deserializedOrderInfo);

			Assert.AreEqual(1000, deserializedOrderInfo.DiscountAmountInCents);
			Assert.AreEqual(18980, deserializedOrderInfo.GrandtotalInCents);
			Assert.AreEqual(15686, deserializedOrderInfo.SubtotalInCents);
		}

		private string xmlWithFailingCouponDiscount = @"<?xml version=""1.0"" encoding=""utf-8""?>
<OrderInfo xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
	<OrderNumber>0016</OrderNumber>
	<OrderDate>dinsdag 15 januari 2013 13:27</OrderDate>
	<UniqueOrderId>6b4e037e-470e-4bdb-9fa6-517112ad7ac9</UniqueOrderId>
	<OrderLines>
		<OrderLine>
			<ProductInfo>
				<Id>1337</Id>
				<ProductVariants>
					<ProductVariantInfo>
						<Id>1883</Id>
						<Title>M</Title>
						<ChangedOn>2013-01-15T13:27:03.259125+01:00</ChangedOn>
						<Group />
						<Weight>0</Weight>
						<Length>0</Length>
						<Height>0</Height>
						<Width>0</Width>
						<PriceInCents>0</PriceInCents>
						<Vat>21</Vat>
						<PriceWithVatInCents>0</PriceWithVatInCents>
						<PriceWithoutVatInCents>0</PriceWithoutVatInCents>
						<PriceWithoutVat>0</PriceWithoutVat>
						<PriceWithVat>0</PriceWithVat>
					</ProductVariantInfo>
				</ProductVariants>
				<Title>Bertoni vest</Title>
				<ChangedOn>2013-01-15T13:27:03.259125+01:00</ChangedOn>
				<SKU>52112.90.314</SKU>
				<Weight>0</Weight>
				<Length>0</Length>
				<Height>0</Height>
				<Width>0</Width>
				<OriginalPriceInCents>6990</OriginalPriceInCents>
				<ProductRangePriceInCents>6990</ProductRangePriceInCents>
				<Vat>21</Vat>
				<Text>0</Text>
				<ImageId>0</ImageId>
				<ItemCount>1</ItemCount>
				<DiscountInCents>500</DiscountInCents>
				<DiscountedVatInCents>1213</DiscountedVatInCents>
				<VatAmountInCents>1213</VatAmountInCents>
				<PriceInCents>6990</PriceInCents>
				<VatAmount>12.13</VatAmount>
				<ProductPriceWithVat>69.9</ProductPriceWithVat>
				<ProductPriceWithoutVat>57.77</ProductPriceWithoutVat>
				<ProductRangePriceWithoutVat>57.77</ProductRangePriceWithoutVat>
				<ProductRangePriceWithVat>69.9</ProductRangePriceWithVat>
				<DiscountedPriceWithVat>69.9</DiscountedPriceWithVat>
				<DiscountedPriceWithoutVat>57.77</DiscountedPriceWithoutVat>
				<DiscountedVat>12.13</DiscountedVat>
				<PriceWithVat>69.9</PriceWithVat>
				<PriceWithoutVat>57.77</PriceWithoutVat>
				<ProductPriceWithVatInCents>6990</ProductPriceWithVatInCents>
				<ProductPriceWithoutVatInCents>5777</ProductPriceWithoutVatInCents>
				<ProductRangePriceWithVatInCents>6990</ProductRangePriceWithVatInCents>
				<ProductRangePriceWithoutVatInCents>5777</ProductRangePriceWithoutVatInCents>
				<PriceWithVatInCents>6990</PriceWithVatInCents>
				<PriceWithoutVatInCents>5777</PriceWithoutVatInCents>
				<DiscountedPriceWithVatInCents>6990</DiscountedPriceWithVatInCents>
				<DiscountedPriceWithoutVatInCents>5777</DiscountedPriceWithoutVatInCents>
			</ProductInfo>
			<OrderlineWeight>0</OrderlineWeight>
			<OrderLineAmountInCents>6990</OrderLineAmountInCents>
			<OrderDiscountInCents>500</OrderDiscountInCents>
			<OrderLineVat>21</OrderLineVat>
			<OrderLineVatAmountInCents>1213</OrderLineVatAmountInCents>
			<OrderLineVatAmountAfterOrderDiscountInCents>1126</OrderLineVatAmountAfterOrderDiscountInCents>
			<OrderLineSubTotal>57.77</OrderLineSubTotal>
			<OrderLineVatAmount>12.13</OrderLineVatAmount>
			<OrderLineGrandTotal>69.9</OrderLineGrandTotal>
			<OrderLineSubTotalInCents>5777</OrderLineSubTotalInCents>
			<OrderLineGrandTotalInCents>6990</OrderLineGrandTotalInCents>
		</OrderLine>
		<OrderLine>
			<ProductInfo>
				<Id>1747</Id>
				<ProductVariants>
					<ProductVariantInfo>
						<Id>1817</Id>
						<Title>N31</Title>
						<ChangedOn>2013-01-15T13:27:24.634125+01:00</ChangedOn>
						<Group />
						<Weight>0</Weight>
						<Length>0</Length>
						<Height>0</Height>
						<Width>0</Width>
						<PriceInCents>0</PriceInCents>
						<Vat>21</Vat>
						<PriceWithVatInCents>0</PriceWithVatInCents>
						<PriceWithoutVatInCents>0</PriceWithoutVatInCents>
						<PriceWithoutVat>0</PriceWithoutVat>
						<PriceWithVat>0</PriceWithVat>
					</ProductVariantInfo>
				</ProductVariants>
				<Title>Cast Iron Broek groen</Title>
				<ChangedOn>2013-01-15T13:27:24.634125+01:00</ChangedOn>
				<SKU>52540.90.81</SKU>
				<Weight>0</Weight>
				<Length>0</Length>
				<Height>0</Height>
				<Width>0</Width>
				<OriginalPriceInCents>12990</OriginalPriceInCents>
				<ProductRangePriceInCents>12990</ProductRangePriceInCents>
				<Vat>21</Vat>
				<Text>0</Text>
				<ImageId>0</ImageId>
				<ItemCount>1</ItemCount>
				<DiscountInCents>500</DiscountInCents>
				<DiscountedVatInCents>2254</DiscountedVatInCents>
				<VatAmountInCents>2254</VatAmountInCents>
				<PriceInCents>12990</PriceInCents>
				<VatAmount>22.54</VatAmount>
				<ProductPriceWithVat>129.9</ProductPriceWithVat>
				<ProductPriceWithoutVat>107.36</ProductPriceWithoutVat>
				<ProductRangePriceWithoutVat>107.36</ProductRangePriceWithoutVat>
				<ProductRangePriceWithVat>129.9</ProductRangePriceWithVat>
				<DiscountedPriceWithVat>129.9</DiscountedPriceWithVat>
				<DiscountedPriceWithoutVat>107.36</DiscountedPriceWithoutVat>
				<DiscountedVat>22.54</DiscountedVat>
				<PriceWithVat>129.9</PriceWithVat>
				<PriceWithoutVat>107.36</PriceWithoutVat>
				<ProductPriceWithVatInCents>12990</ProductPriceWithVatInCents>
				<ProductPriceWithoutVatInCents>10736</ProductPriceWithoutVatInCents>
				<ProductRangePriceWithVatInCents>12990</ProductRangePriceWithVatInCents>
				<ProductRangePriceWithoutVatInCents>10736</ProductRangePriceWithoutVatInCents>
				<PriceWithVatInCents>12990</PriceWithVatInCents>
				<PriceWithoutVatInCents>10736</PriceWithoutVatInCents>
				<DiscountedPriceWithVatInCents>12990</DiscountedPriceWithVatInCents>
				<DiscountedPriceWithoutVatInCents>10736</DiscountedPriceWithoutVatInCents>
			</ProductInfo>
			<OrderlineWeight>0</OrderlineWeight>
			<OrderLineAmountInCents>12990</OrderLineAmountInCents>
			<OrderDiscountInCents>500</OrderDiscountInCents>
			<OrderLineVat>21</OrderLineVat>
			<OrderLineVatAmountInCents>2254</OrderLineVatAmountInCents>
			<OrderLineVatAmountAfterOrderDiscountInCents>2167</OrderLineVatAmountAfterOrderDiscountInCents>
			<OrderLineSubTotal>107.36</OrderLineSubTotal>
			<OrderLineVatAmount>22.54</OrderLineVatAmount>
			<OrderLineGrandTotal>129.9</OrderLineGrandTotal>
			<OrderLineSubTotalInCents>10736</OrderLineSubTotalInCents>
			<OrderLineGrandTotalInCents>12990</OrderLineGrandTotalInCents>
		</OrderLine>
	</OrderLines>
	<CouponCodes>
		<string>gerben</string>
	</CouponCodes>
	<CustomerInfo>
		<CountryCode>NL</CountryCode>
		<CountryName>Netherlands</CountryName>
		<ShippingCountryCode>NL</ShippingCountryCode>
		<ShippingCountryName>Netherlands</ShippingCountryName>
		<RegionName />
		<CustomerInformation>
			<Customer>
				<customerEmail><![CDATA[g.vandiggelen@wvdmedia.nl]]></customerEmail>
				<customerGender><![CDATA[M]]></customerGender>
				<customerFirstName><![CDATA[Gerben]]></customerFirstName>
				<customerLastName><![CDATA[Diggelen]]></customerLastName>
				<customerStreet><![CDATA[Klardendalseweg]]></customerStreet>
				<customerStreetNumber><![CDATA[11]]></customerStreetNumber>
				<customerPostalCode><![CDATA[6822GC]]></customerPostalCode>
				<customerCity><![CDATA[Arnhem]]></customerCity>
				<customerBirthdayDay><![CDATA[2]]></customerBirthdayDay>
				<customerBirthdayMonth><![CDATA[5]]></customerBirthdayMonth>
				<customerBirthdayYear><![CDATA[1997]]></customerBirthdayYear>
				<customerPhone><![CDATA[1624734869]]></customerPhone>
				<customerCountry><![CDATA[NL]]></customerCountry>
			</Customer>
		</CustomerInformation>
		<ShippingInformation>
			<Shipping />
		</ShippingInformation>
	</CustomerInfo>
	<ShippingInfo>
		<Id>1249</Id>
		<Title>Afhalen in de winkel</Title>
		<MethodId>1253</MethodId>
		<MethodTitle>Afhalen in de winkel in Sliedrecht</MethodTitle>
		<TransactionMethod>QueryString</TransactionMethod>
		<ShippingType>Pickup</ShippingType>
	</ShippingInfo>
	<PaymentInfo>
		<Id>1307</Id>
		<Title>Mollie</Title>
		<MethodId>9999</MethodId>
		<MethodTitle>TBM Bank</MethodTitle>
		<Url>https://www.mollie.nl/partners/ideal-test-bank?order_nr=M0968511M105Y7B6&amp;transaction_id=b380efbfbeb0d451daa24ea72c3eb48d&amp;trxid=0000096851110576</Url>
		<TransactionId>b380efbfbeb0d451daa24ea72c3eb48d</TransactionId>
		<TransactionMethod>QueryString</TransactionMethod>
		<PaymentType>OnlinePayment</PaymentType>
	</PaymentInfo>
	<StoreInfo>
		<LanguageCode>nl-NL</LanguageCode>
		<Alias>store</Alias>
		<CountryCode>NL</CountryCode>
		<Culture>nl-NL</Culture>
		<CurrencyCulture>nl-NL</CurrencyCulture>
	</StoreInfo>
	<ShippingProviderAmountInCents>0</ShippingProviderAmountInCents>
	<PaymentProviderPriceInCents>0</PaymentProviderPriceInCents>
	<OrderNodeId>2280</OrderNodeId>
	<Status>ReadyForDispatch</Status>
	<ShippingCostsMightBeOutdated>true</ShippingCostsMightBeOutdated>
	<ShippingProviderVatAmountInCents>0</ShippingProviderVatAmountInCents>
	<PaymentProviderVatAmountInCents>0</PaymentProviderVatAmountInCents>
	<IsDiscounted>True</IsDiscounted>
	<OrderValidationErrors />
	<RegionalVatInCents>0</RegionalVatInCents>
	<TotalVatInCents>3294</TotalVatInCents>
	<OrderTotalInCents>18980</OrderTotalInCents>
	<GrandtotalWithoutVatInCents>15513</GrandtotalWithoutVatInCents>
	<GrandtotalInCents>18980</GrandtotalInCents>
	<VatTotal>3293</VatTotal>
	<OrderLineTotalInCents>19980</OrderLineTotalInCents>
	<Grandtotal>189.8</Grandtotal>
	<OrderLineWithVatTotal>199.8</OrderLineWithVatTotal>
	<OrderLineWithoutVatTotal>165.12</OrderLineWithoutVatTotal>
	<GrandtotalWithoutVat>155.13</GrandtotalWithoutVat>
	<TotalVat>32.94</TotalVat>
	<ShippingProviderVatAmount>0</ShippingProviderVatAmount>
	<ShippingProviderCostsWithoutVat>0</ShippingProviderCostsWithoutVat>
	<ShippingProviderCostsWithVat>0</ShippingProviderCostsWithVat>
	<PaymentProviderVatAmount>0</PaymentProviderVatAmount>
	<PaymentProviderCostsWithoutVat>0</PaymentProviderCostsWithoutVat>
	<PaymentProviderCostsWithVat>0</PaymentProviderCostsWithVat>
	<DiscountAmount>10</DiscountAmount>
	<DiscountAmountWithVat>10</DiscountAmountWithVat>
	<DiscountAmountWithoutVat>8.26</DiscountAmountWithoutVat>
	<RegionalVat>0</RegionalVat>
	<Subtotal>156.86</Subtotal>
	<DiscountAmounWithVattInCents>1000</DiscountAmounWithVattInCents>
	<DiscountAmountWithoutVatInCents>826</DiscountAmountWithoutVatInCents>
	<OrderLineTotalWithVatInCents>19980</OrderLineTotalWithVatInCents>
	<SubtotalInCents>15686</SubtotalInCents>
	<OrderLineTotalWithoutVatInCents>16512</OrderLineTotalWithoutVatInCents>
	<PaymentProviderCostsWithoutVatInCents>0</PaymentProviderCostsWithoutVatInCents>
	<PaymentProviderCostsWithVatInCents>0</PaymentProviderCostsWithVatInCents>
	<ShippingProviderCostsWithoutVatInCents>0</ShippingProviderCostsWithoutVatInCents>
	<ShippingProviderCostsWithVatInCents>0</ShippingProviderCostsWithVatInCents>
</OrderInfo>";

		[Test]
		public void ThatReadingBackXMLHandlesProductDiscountsAndProductVariantsCorrectlyOwnDB()
		{
			IOC.SettingsService.InclVat();
			IOC.VatCalculationStrategy.OverTotal();

			var deserializedOrderInfo = OrderInfo.CreateOrderInfoFromOrderData(new uWebshopOrderData()
		    {
		        OrderInfo = orderInfo20OwnDB
		    });
			Assert.NotNull(deserializedOrderInfo);


			var productInfo = deserializedOrderInfo.OrderLines.First().ProductInfo;
			Assert.AreEqual(5000, productInfo.PriceInCents);
			Assert.AreEqual(10000, productInfo.ProductRangePriceInCents);
			Assert.AreEqual(5000, productInfo.DiscountAmountInCents);

			var productInfo2 = deserializedOrderInfo.OrderLines.Last().ProductInfo;
			Assert.AreEqual(6000, productInfo2.PriceInCents);
			Assert.AreEqual(11000, productInfo2.ProductRangePriceInCents);
			Assert.AreEqual(5000, productInfo2.DiscountAmountInCents);

			Assert.AreEqual(56000, deserializedOrderInfo.OrderLineTotalInCents);

			Assert.AreEqual(0, deserializedOrderInfo.ShippingProviderCostsWithVatInCents);

			Assert.AreEqual(46281, deserializedOrderInfo.SubtotalInCents);
			Assert.AreEqual(56000, deserializedOrderInfo.GrandtotalInCents);
		}

		private string orderInfo20OwnDB = @"<?xml version=""1.0"" encoding=""utf-8""?>
<OrderInfo xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
<OrderNumber>[INCOMPLETE]-0001</OrderNumber>
	<OrderDate>Thursday, February 21, 2013 2:52 PM</OrderDate>
	<UniqueOrderId>3072c0f8-6d7b-47cc-8a4e-2157415f299d</UniqueOrderId>
	<OrderLines>
		<OrderLine>
			<ProductInfo>
				<Id>1147</Id>
				<ProductVariants />
				<Title>Test Product</Title>
				<ChangedOn>2013-02-21T14:51:18.713961+01:00</ChangedOn>
				<SKU>Test Product</SKU>
				<Weight>0</Weight>
				<Length>0</Length>
				<Height>0</Height>
				<Width>0</Width>
				<OriginalPriceInCents>10000</OriginalPriceInCents>
				<ProductRangePriceInCents>10000</ProductRangePriceInCents>
				<Vat>21</Vat>
				<Text />
				<ImageId>0</ImageId>
				<ItemCount>10</ItemCount>
				<DiscountInCents>0</DiscountInCents>
				<DiscountedVatInCents>868</DiscountedVatInCents>
				<VatAmountInCents>868</VatAmountInCents>
				<PriceInCents>5000</PriceInCents>
				<VatAmount>8.68</VatAmount>
				<ProductPriceWithVat>100</ProductPriceWithVat>
				<ProductPriceWithoutVat>82.64</ProductPriceWithoutVat>
				<ProductRangePriceWithoutVat>82.64</ProductRangePriceWithoutVat>
				<ProductRangePriceWithVat>100</ProductRangePriceWithVat>
				<DiscountedPriceWithVat>50</DiscountedPriceWithVat>
				<DiscountedPriceWithoutVat>41.32</DiscountedPriceWithoutVat>
				<DiscountedVat>8.68</DiscountedVat>
				<PriceWithVat>50</PriceWithVat>
				<PriceWithoutVat>41.32</PriceWithoutVat>
				<ProductPriceWithVatInCents>10000</ProductPriceWithVatInCents>
				<ProductPriceWithoutVatInCents>8264</ProductPriceWithoutVatInCents>
				<ProductRangePriceWithVatInCents>10000</ProductRangePriceWithVatInCents>
				<ProductRangePriceWithoutVatInCents>8264</ProductRangePriceWithoutVatInCents>
				<PriceWithVatInCents>5000</PriceWithVatInCents>
				<PriceWithoutVatInCents>4132</PriceWithoutVatInCents>
				<DiscountedPriceWithVatInCents>5000</DiscountedPriceWithVatInCents>
				<DiscountedPriceWithoutVatInCents>4132</DiscountedPriceWithoutVatInCents>
			</ProductInfo>
			<OrderlineWeight>0</OrderlineWeight>
			<OrderLineAmountInCents>50000</OrderLineAmountInCents>
			<OrderDiscountInCents>0</OrderDiscountInCents>
			<OrderLineVat>21</OrderLineVat>
			<OrderLineVatAmountInCents>8680</OrderLineVatAmountInCents>
			<OrderLineVatAmountAfterOrderDiscountInCents>8680</OrderLineVatAmountAfterOrderDiscountInCents>
			<OrderLineSubTotal>413.22</OrderLineSubTotal>
			<OrderLineVatAmount>86.8</OrderLineVatAmount>
			<OrderLineGrandTotal>500</OrderLineGrandTotal>
			<OrderLineSubTotalInCents>41322</OrderLineSubTotalInCents>
			<OrderLineGrandTotalInCents>50000</OrderLineGrandTotalInCents>
		</OrderLine>
		<OrderLine>
			<ProductInfo>
				<Id>1147</Id>
				<ProductVariants>
					<ProductVariantInfo>
						<Id>1148</Id>
						<Title>Test Variant</Title>
						<ChangedOn>2013-02-21T14:52:46.2012657+01:00</ChangedOn>
						<Group>Varianten</Group>
						<Weight>0</Weight>
						<Length>0</Length>
						<Height>0</Height>
						<Width>0</Width>
						<PriceInCents>1000</PriceInCents>
						<Vat>21</Vat>
						<PriceWithVatInCents>1000</PriceWithVatInCents>
						<PriceWithoutVatInCents>826</PriceWithoutVatInCents>
						<PriceWithoutVat>8.26</PriceWithoutVat>
						<PriceWithVat>10</PriceWithVat>
					</ProductVariantInfo>
				</ProductVariants>
				<Title>Test Product</Title>
				<ChangedOn>2013-02-21T14:52:46.2012657+01:00</ChangedOn>
				<SKU>Test Product</SKU>
				<Weight>0</Weight>
				<Length>0</Length>
				<Height>0</Height>
				<Width>0</Width>
				<OriginalPriceInCents>10000</OriginalPriceInCents>
				<ProductRangePriceInCents>11000</ProductRangePriceInCents>
				<Vat>21</Vat>
				<Text />
				<ImageId>0</ImageId>
				<ItemCount>1</ItemCount>
				<DiscountInCents>0</DiscountInCents>
				<DiscountedVatInCents>1041</DiscountedVatInCents>
				<VatAmountInCents>1041</VatAmountInCents>
				<PriceInCents>6000</PriceInCents>
				<VatAmount>10.41</VatAmount>
				<ProductPriceWithVat>100</ProductPriceWithVat>
				<ProductPriceWithoutVat>82.64</ProductPriceWithoutVat>
				<ProductRangePriceWithoutVat>90.91</ProductRangePriceWithoutVat>
				<ProductRangePriceWithVat>110</ProductRangePriceWithVat>
				<DiscountedPriceWithVat>60</DiscountedPriceWithVat>
				<DiscountedPriceWithoutVat>49.59</DiscountedPriceWithoutVat>
				<DiscountedVat>10.41</DiscountedVat>
				<PriceWithVat>60</PriceWithVat>
				<PriceWithoutVat>49.59</PriceWithoutVat>
				<ProductPriceWithVatInCents>10000</ProductPriceWithVatInCents>
				<ProductPriceWithoutVatInCents>8264</ProductPriceWithoutVatInCents>
				<ProductRangePriceWithVatInCents>11000</ProductRangePriceWithVatInCents>
				<ProductRangePriceWithoutVatInCents>9091</ProductRangePriceWithoutVatInCents>
				<PriceWithVatInCents>6000</PriceWithVatInCents>
				<PriceWithoutVatInCents>4959</PriceWithoutVatInCents>
				<DiscountedPriceWithVatInCents>6000</DiscountedPriceWithVatInCents>
				<DiscountedPriceWithoutVatInCents>4959</DiscountedPriceWithoutVatInCents>
			</ProductInfo>
			<OrderlineWeight>0</OrderlineWeight>
			<OrderLineAmountInCents>6000</OrderLineAmountInCents>
			<OrderDiscountInCents>0</OrderDiscountInCents>
			<OrderLineVat>21</OrderLineVat>
			<OrderLineVatAmountInCents>1041</OrderLineVatAmountInCents>
			<OrderLineVatAmountAfterOrderDiscountInCents>1041</OrderLineVatAmountAfterOrderDiscountInCents>
			<OrderLineSubTotal>49.59</OrderLineSubTotal>
			<OrderLineVatAmount>10.41</OrderLineVatAmount>
			<OrderLineGrandTotal>60</OrderLineGrandTotal>
			<OrderLineSubTotalInCents>4959</OrderLineSubTotalInCents>
			<OrderLineGrandTotalInCents>6000</OrderLineGrandTotalInCents>
		</OrderLine>
	</OrderLines>
	<CouponCodes />
	<CustomerInfo>
		<CountryCode>NL</CountryCode>
		<CountryName>Netherlands</CountryName>
		<ShippingCountryCode>NL</ShippingCountryCode>
		<ShippingCountryName>Netherlands</ShippingCountryName>
		<RegionName />
	</CustomerInfo>
	<ShippingInfo>
		<Id>0</Id>
		<TransactionMethod>QueryString</TransactionMethod>
		<ShippingType>Unknown</ShippingType>
	</ShippingInfo>
	<PaymentInfo>
		<Id>0</Id>
		<TransactionMethod>QueryString</TransactionMethod>
		<PaymentType>Unknown</PaymentType>
	</PaymentInfo>
	<StoreInfo>
		<LanguageCode>en-US</LanguageCode>
		<Alias>ToyStore</Alias>
		<CountryCode>NL</CountryCode>
		<Culture>en-US</Culture>
		<CurrencyCulture>en-US</CurrencyCulture>
	</StoreInfo>
	<ShippingProviderAmountInCents>0</ShippingProviderAmountInCents>
	<PaymentProviderPriceInCents>0</PaymentProviderPriceInCents>
	<OrderNodeId>0</OrderNodeId>
	<Status>Incomplete</Status>
	<ShippingCostsMightBeOutdated>true</ShippingCostsMightBeOutdated>
	<ShippingProviderVatAmountInCents>0</ShippingProviderVatAmountInCents>
	<PaymentProviderVatAmountInCents>0</PaymentProviderVatAmountInCents>
	<IsDiscounted>False</IsDiscounted>
	<OrderValidationErrors />
	<RegionalVatInCents>0</RegionalVatInCents>
	<TotalVatInCents>9719</TotalVatInCents>
	<OrderTotalInCents>56000</OrderTotalInCents>
	<GrandtotalWithoutVatInCents>46281</GrandtotalWithoutVatInCents>
	<GrandtotalInCents>56000</GrandtotalInCents>
	<VatTotal>9721</VatTotal>
	<OrderLineTotalInCents>56000</OrderLineTotalInCents>
	<Grandtotal>560</Grandtotal>
	<OrderLineWithVatTotal>560</OrderLineWithVatTotal>
	<OrderLineWithoutVatTotal>462.81</OrderLineWithoutVatTotal>
	<GrandtotalWithoutVat>462.81</GrandtotalWithoutVat>
	<TotalVat>97.19</TotalVat>
	<ShippingProviderVatAmount>0</ShippingProviderVatAmount>
	<ShippingProviderCostsWithoutVat>0</ShippingProviderCostsWithoutVat>
	<ShippingProviderCostsWithVat>0</ShippingProviderCostsWithVat>
	<PaymentProviderVatAmount>0</PaymentProviderVatAmount>
	<PaymentProviderCostsWithoutVat>0</PaymentProviderCostsWithoutVat>
	<PaymentProviderCostsWithVat>0</PaymentProviderCostsWithVat>
	<DiscountAmount>0</DiscountAmount>
	<DiscountAmountWithVat>0</DiscountAmountWithVat>
	<DiscountAmountWithoutVat>0</DiscountAmountWithoutVat>
	<RegionalVat>0</RegionalVat>
	<Subtotal>462.81</Subtotal>
	<DiscountAmounWithVattInCents>0</DiscountAmounWithVattInCents>
	<DiscountAmountWithoutVatInCents>0</DiscountAmountWithoutVatInCents>
	<OrderLineTotalWithVatInCents>56000</OrderLineTotalWithVatInCents>
	<SubtotalInCents>46281</SubtotalInCents>
	<OrderLineTotalWithoutVatInCents>46281</OrderLineTotalWithoutVatInCents>
	<PaymentProviderCostsWithoutVatInCents>0</PaymentProviderCostsWithoutVatInCents>
	<PaymentProviderCostsWithVatInCents>0</PaymentProviderCostsWithVatInCents>
	<ShippingProviderCostsWithoutVatInCents>0</ShippingProviderCostsWithoutVatInCents>
	<ShippingProviderCostsWithVatInCents>0</ShippingProviderCostsWithVatInCents>
</OrderInfo>";
	}
}