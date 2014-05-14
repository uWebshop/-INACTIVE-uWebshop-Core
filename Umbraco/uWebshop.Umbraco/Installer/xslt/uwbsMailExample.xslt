<?xml version="1.0" encoding="UTF-8"?>

<!DOCTYPE xsl:stylesheet [
    <!ENTITY nbsp "&#x00A0;">
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
	<xsl:param name="storeAlias" />
	<xsl:param name="Title" />
	<xsl:param name="Description" />

	<xsl:template match="/">
		<h1>
			<xsl:value-of select="$Title" />
		</h1>
		<xsl:value-of select="$Description" disable-output-escaping="yes" />
		<xsl:variable name="currencyCulture" select="OrderInfo/StoreInfo/CurrencyCulture" />
		<h2>
			Order: <xsl:value-of select="OrderInfo/OrderNumber" />
		</h2>
		<p>
			<strong>Date/Time Placed:</strong>&nbsp;<xsl:value-of select="OrderInfo/OrderDate" />
		</p>
		<xsl:if test="count(//ExtraInformation/Extra/*)">
			<table style="line-height: 1.5em;">
				<xsl:for-each select="//ExtraInformation/Extra/*">
					<tr>
						<th scope="row">
							<xsl:value-of select="uWebshop.Orders:ReplaceCharacters(name(), 'extra', string.empty)" />
						</th>
						<td>
							<xsl:value-of select="." />&nbsp;
						</td>
					</tr>
				</xsl:for-each>
			</table>
		</xsl:if>
		<table width="100%">
			<tr>
				<td valign="top" width="50%">
					<h2>Customer Details</h2>

					<xsl:if test="count(//CustomerInformation/Customer/*)">
						<table style="line-height: 1.5em;">
							<xsl:for-each select="//CustomerInformation/Customer/*">
								<tr>
									<th scope="row">
										<xsl:value-of select="uWebshop.Orders:ReplaceCharacters(name(), 'customer', string.empty)" />
									</th>
									<td>
										<xsl:value-of select="." />&nbsp;
									</td>
								</tr>
							</xsl:for-each>
						</table>
					</xsl:if>
				</td>
				<td valign="top" width="50%">
					<h2>Shipping Details</h2>
					<xsl:if test="count(//ShippingInformation/Shipping/*)">
						<table style="line-height: 1.5em;">
							<xsl:for-each select="//ShippingInformation/Shipping/*">
								<tr>
									<th scope="row">
										<xsl:value-of select="uWebshop.Orders:ReplaceCharacters(name(), 'shipping', string.empty)" />
									</th>
									<td>
										<xsl:value-of select="." />&nbsp;
									</td>
								</tr>
							</xsl:for-each>
						</table>
					</xsl:if>
				</td>
			</tr>
		</table>
		<h2>Order Details</h2>

		<table style="line-height: 2em;">
			<thead>
				<tr>
					<th scope="col">Product</th>
					<th scope="col">Itemcount</th>
					<th scope="col" style="text-align: right">Price</th>
					<th scope="col" style="text-align: right">Total</th>
				</tr>
			</thead>
			<tbody>
				<xsl:for-each select="OrderInfo/OrderLines/OrderLine">
					<tr>
						<th scope="row" style="vertical-align: top;">
							<xsl:value-of select="ProductInfo/Title" />
							<br />
							<xsl:for-each select="ProductInfo/ProductVariants/ProductVariantInfo">
								<xsl:value-of select="Title" />
							</xsl:for-each>
						</th>
						<td style="vertical-align: top;">
							<xsl:value-of select="ProductInfo/ItemCount" />
						</td>
						<td style="vertical-align: top; text-align: right">
							<xsl:value-of select="uWebshop.Orders:CentsToPrice(ProductInfo/PriceWithVatInCents, $currencyCulture, true)" />
						</td>
						<td style="vertical-align: top; text-align: right">
							<xsl:value-of select="uWebshop.Orders:CentsToPrice(OrderLineGrandTotalInCents, $currencyCulture, true)" />
						</td>
					</tr>
				</xsl:for-each>
			</tbody>
			<tfoot>
				<tr>

					<th scope="row" style="text-align: right" colspan="3">Subtotal</th>
					<td scope="col" style="text-align: right">
						<xsl:value-of select="uWebshop.Orders:CentsToPrice(OrderInfo/SubtotalInCents, $currencyCulture, 1)" />
					</td>
				</tr>
				<tr>
					<th scope="row" style="text-align: right" colspan="3">Vat</th>
					<td scope="col" style="text-align: right">
						<xsl:value-of select="uWebshop.Orders:CentsToPrice(OrderInfo/TotalVatInCents, $currencyCulture, 0)" />
					</td>
				</tr>
				<tr>
					<th scope="row" style="text-align: right" colspan="3">Grandtotal</th>
					<td scope="col" style="text-align: right">
						<h2>
							<xsl:value-of select="uWebshop.Orders:CentsToPrice(OrderInfo/GrandtotalInCents, $currencyCulture, 1)" />
						</h2>
					</td>
				</tr>
			</tfoot>
		</table>
		<table width="100%">
			<tr>
				<td valign="top" width="50%" style="line-height: 1.5em;">
					<h2>Shipping information</h2>
					<xsl:value-of select="OrderInfo/ShippingInfo/Title" />
					<br /><xsl:value-of select="OrderInfo/ShippingInfo/ShippingType" />
				</td>
				<td valign="top" width="50%" style="line-height: 1.5em;">
					<h2>Payment information</h2>
					<xsl:value-of select="OrderInfo/PaymentInfo/Title" />
					<br /><xsl:value-of select="OrderInfo/PaymentInfo/PaymentType" />
				</td>
			</tr>
		</table>

	</xsl:template>

</xsl:stylesheet>