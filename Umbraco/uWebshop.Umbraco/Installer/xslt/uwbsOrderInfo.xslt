<?xml version="1.0" encoding="UTF-8"?>

<!DOCTYPE xsl:stylesheet [
	<!ENTITY nbsp   "&#160;">
	<!ENTITY copy   "&#169;">
	<!ENTITY reg    "&#174;">
	<!ENTITY trade  "&#8482;">
	<!ENTITY mdash  "&#8212;">
	<!ENTITY ldquo  "&#8220;">
	<!ENTITY rdquo  "&#8221;">
	<!ENTITY pound  "&#163;">
	<!ENTITY yen    "&#165;">
	<!ENTITY euro   "&#8364;">
]>
<xsl:stylesheet
	version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:msxml="urn:schemas-microsoft-com:xslt"
	xmlns:umbraco.library="urn:umbraco.library" xmlns:Exslt.ExsltCommon="urn:Exslt.ExsltCommon"
	xmlns:Exslt.ExsltDatesAndTimes="urn:Exslt.ExsltDatesAndTimes" xmlns:Exslt.ExsltMath="urn:Exslt.ExsltMath"
	xmlns:Exslt.ExsltRegularExpressions="urn:Exslt.ExsltRegularExpressions"
	xmlns:Exslt.ExsltStrings="urn:Exslt.ExsltStrings" xmlns:Exslt.ExsltSets="urn:Exslt.ExsltSets"
	xmlns:uWebshop.Catalog="urn:uWebshop.Catalog" xmlns:uWebshop.Navigation="urn:uWebshop.Navigation"
	xmlns:uWebshop.Node="urn:uWebshop.Node" xmlns:uWebshop.Orders="urn:uWebshop.Orders"
	exclude-result-prefixes="msxml umbraco.library Exslt.ExsltCommon Exslt.ExsltDatesAndTimes Exslt.ExsltMath Exslt.ExsltRegularExpressions Exslt.ExsltStrings Exslt.ExsltSets uWebshop.Catalog uWebshop.Navigation uWebshop.Node uWebshop.Orders ">


	<xsl:output method="xml" omit-xml-declaration="yes" />

	<xsl:param name="uniqueOrderId" />

	<xsl:template match="/">
		<style>

			.order table {
			border-collapse:collapse
			}

			.order .text-right {
			text-align: right !important;
			}

			.order tr.orderline {
			border-bottom: 1px solid #eee;
			}

			.order tr.orderlineheader {
			border-bottom: 4px solid #eee;
			}

			.order tr.orderlinefooter {
			border-top: 4px solid #eee;
			}

			.order td,
			.order th {
			padding: 5px;
			}

			.order tr.success {
			vertical-align: middle;
			}

			.order .success h3 {
			margin-bottom: 10px;
			}



		</style>
		<xsl:variable name="currencyCulture" select="OrderInfo/StoreInfo/CurrencyCulture" />
		<div class="order" style="margin: 20px; line-height: 1.5em;">
			<h1>
				<xsl:value-of select="OrderInfo/OrderNumber" />
			</h1>

			<address style="font-style: normal;">
				<strong>Billing</strong><br/>
				<xsl:value-of select="//CustomerInformation/Customer/customerFirstName" />&nbsp;<xsl:value-of select="//CustomerInformation/Customer/customerLastName" /><br/>
				<xsl:if test="//CustomerInformation/Customer/customerAddress1 !=''">
					<xsl:value-of select="//CustomerInformation/Customer/customerAddress1" />
					<br/>
				</xsl:if>
				<xsl:if test="//CustomerInformation/Customer/customerAddress2 !=''">
					<xsl:value-of select="//CustomerInformation/Customer/customerAddress2" />
					<br/>
				</xsl:if>
				<xsl:if test="//CustomerInformation/Customer/customerPostalCode !=''">
					<xsl:value-of select="//CustomerInformation/Customer/customerPostalCode" />,&nbsp;
				</xsl:if>
				<xsl:if test="//CustomerInformation/Customer/customerCity !=''">
					<xsl:value-of select="//CustomerInformation/Customer/customerCity" />
					<br/>
				</xsl:if>
				<xsl:if test="//CustomerInformation/Customer/customerCountry !=''">
					<xsl:value-of select="uWebshop.Orders:GetFullCountryNameFromCountry(//CustomerInformation/Customer/customerCountry)" />
					<br/>
				</xsl:if>
				<xsl:if test="//CustomerInformation/Customer/customerVATNumber !=''">
					<br/>
					<strong>VAT Number:</strong>&nbsp;<xsl:value-of select="//CustomerInformation/Customer/customerVATNumber" /><br/>
				</xsl:if>
				<a href="mailto:{//CustomerInformation/Customer/customerEmail}">
					<xsl:value-of select="//CustomerInformation/Customer/customerEmail" />
				</a>
			</address>
			<hr style="margin: 20px 0; border: 0; border-top: 1px solid #eee; border-bottom: 1px solid #fff;"/>
			<address style="font-style: normal;">
				<strong>Shipping</strong><br/>
				<xsl:value-of select="//ShippingInformation/Shipping/shippingFirstName" />&nbsp;<xsl:value-of select="//ShippingInformation/Shipping/shippingLastName" /><br/>
				<xsl:if test="//ShippingInformation/Shipping/shippingAddress1 !=''">
					<xsl:value-of select="//ShippingInformation/Shipping/shippingAddress1" />
					<br/>
				</xsl:if>
				<xsl:if test="//ShippingInformation/Shipping/shippingAddress2 !=''">
					<xsl:value-of select="//ShippingInformation/Shipping/shippingAddress2" />
					<br/>
				</xsl:if>
				<xsl:if test="//ShippingInformation/Shipping/shippingPostalCode !=''">
					<xsl:value-of select="//ShippingInformation/Shipping/shippingPostalCode" />,&nbsp;
				</xsl:if>
				<xsl:if test="//ShippingInformation/Shipping/shippingCity !=''">
					<xsl:value-of select="//ShippingInformation/Shipping/shippingCity" />
					<br/>
				</xsl:if>
				<xsl:if test="//ShippingInformation/Shipping/shippingCountry !=''">
					<xsl:value-of select="uWebshop.Orders:GetFullCountryNameFromCountry(//ShippingInformation/Shipping/shippingCountry)" />
					<br/>
				</xsl:if>
			</address>
			<hr style="margin: 20px 0; border: 0; border-top: 1px solid #eee; border-bottom: 1px solid #fff;"/>
			<p>
				<strong>Date/Time Placed:</strong>&nbsp;<xsl:value-of select="OrderInfo/OrderDate" />
			</p>
			<hr style="margin: 20px 0; border: 0; border-top: 1px solid #eee; border-bottom: 1px solid #fff;"/>
			<table class="table table-striped table-hover">
				<thead>
					<tr class="orderlineheader">
						<th scope="col">
							<p>Product</p>
						</th>
						<th scope="col">
							<p class="text-right">Itemcount</p>
						</th>
						<th scope="col">
							<p class="text-right">Price (ex vat)</p>
						</th>
						<th scope="col">
							<p class="text-right">Total (ex vat)</p>
						</th>
					</tr>
				</thead>
				<tbody>
					<xsl:for-each select="OrderInfo/OrderLines/OrderLine">
						<tr class="orderline">
							<th scope="row">
								<p>
									<nobr>
										<xsl:value-of select="ProductInfo/Title" />
									</nobr>
									<xsl:for-each select="ProductInfo/ProductVariants/ProductVariantInfo">
										<br />
										<span style="font-weight:100">
											<xsl:value-of select="Title" />
											<br/>
										</span>
									</xsl:for-each>

								</p>
							</th>
							<td>
								<p class="text-right">
									<xsl:value-of select="ProductInfo/ItemCount" />
								</p>
							</td>
							<td>
								<p class="text-right">
									<xsl:value-of select="uWebshop.Orders:CentsToPrice(ProductInfo/PriceWithoutVatInCents, $currencyCulture, 1)" />
								</p>
							</td>
							<td>
								<p class="text-right">
									<xsl:value-of select="uWebshop.Orders:CentsToPrice(OrderLineSubTotalInCents, $currencyCulture, 1)" />
								</p>
							</td>
						</tr>
					</xsl:for-each>
				</tbody>
				<tfoot>
					<xsl:if test="OrderInfo/VATCharged != 'false'">
						<tr class="orderlinefooter">

							<th scope="row" colspan="3">
								<p class="text-right">Subtotal</p>
							</th>
							<td scope="col">
								<p class="text-right">
									<xsl:value-of select="uWebshop.Orders:CentsToPrice(OrderInfo/SubtotalInCents, $currencyCulture, 1)" />
								</p>
							</td>
						</tr>
						<tr>
							<th scope="row" colspan="3">
								<p class="text-right">Vat</p>
							</th>
							<td scope="col">
								<p class="text-right">
									<xsl:value-of select="uWebshop.Orders:CentsToPrice(OrderInfo/TotalVatInCents, $currencyCulture, 1)" />
								</p>
							</td>
						</tr>
					</xsl:if>
					<tr  class="success">
						<th scope="col" colspan="4">
							<h3 style="font-size: 24.5px;" class="text-right">
								<xsl:value-of select="uWebshop.Orders:CentsToPrice(OrderInfo/ChargedAmountInCents, $currencyCulture, 1)" />
							</h3>
						</th>
					</tr>
					<xsl:if test="OrderInfo/VATCharged = 'false'">
						<tr>
							<td colspan="4">
								<p class="text-right">reverse charge</p>
							</td>
						</tr>
					</xsl:if>
				</tfoot>
			</table>

			<xsl:if test="OrderInfo/ShippingInfo/Title != ''">
				<h4>Shipping choice</h4>
				<xsl:value-of select="OrderInfo/ShippingInfo/Title" />
				<br />
				<xsl:value-of select="OrderInfo/ShippingInfo/ShippingType" />
				<hr style="margin: 20px 0; border: 0; border-top: 1px solid #eee; border-bottom: 1px solid #fff;"/>
			</xsl:if>
			<xsl:if test="OrderInfo/PaymentInfo/Title != ''">
				<h4>Payment provider</h4>
				<xsl:value-of select="OrderInfo/PaymentInfo/Title" />
				<xsl:if test="OrderInfo/PaymentInfo/PaymentType != 'Unknown'">
					<br />
					<xsl:value-of select="OrderInfo/PaymentInfo/PaymentType" />
				</xsl:if>
			</xsl:if>
			<hr style="margin: 20px 0; border: 0; border-top: 1px solid #eee; border-bottom: 1px solid #fff;"/>

		</div>
	</xsl:template>

</xsl:stylesheet>