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


	<xsl:param name="storeAlias" />
	<xsl:param name="Title" />
	<xsl:param name="Description" />
	<xsl:param name="UserName" />
	<xsl:param name="Password" />

	<xsl:template match="/">
		<h1>
			<xsl:value-of select="$Title" />
		</h1>
		<xsl:value-of select="$Description" disable-output-escaping="yes" />

		<table>
			<tr>
				<th scope="row">UserName: </th>
				<td>
					<xsl:value-of select="$UserName" />
				</td>
			</tr>
			<tr>
				<th scope="row">Password: </th>
				<td>
					<xsl:value-of select="$Password" />
				</td>
			</tr>
		</table>

	</xsl:template>

</xsl:stylesheet>