using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using uWebshop.Domain;
using uWebshop.Domain.ContentTypes;

namespace uWebshop.Umbraco.Repositories
{
	public class DocTypeAliassesCodeGeneration
	{
		class type
		{
			public string name;
			public string clrTypeName;
			public string alias;
			public List<string> propertyAliasses;
		}

		public string RunT4Code()
		{
			var types = new List<type>();
			foreach (var type in Assembly.GetAssembly(typeof (Settings)).GetTypes().Where(x => Attribute.IsDefined(x, typeof (ContentTypeAttribute), false)).OrderBy(d => d.Name))
			{
				var attribute = (ContentTypeAttribute) type.GetCustomAttributes(typeof (ContentTypeAttribute), false).Single();
				
				var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => Attribute.IsDefined(x, typeof(ContentPropertyTypeAttribute), false)).Select(property => (ContentPropertyTypeAttribute)property.GetCustomAttributes(typeof(ContentPropertyTypeAttribute), false).Single());

				if (!attribute.InstallerOnly)
				{
					types.Add(new type {alias = attribute.Alias, name = type.Name.Replace("ContentType", string.Empty), clrTypeName = type.Name, propertyAliasses = properties.Select(p => p.Alias).ToList()});
				}
			}

			foreach (var type in types)
			{
				Write("internal interface I" + type.name + "AliassesService\r\n{\r\n");
				Write("\tstring ContentTypeAlias { get; }\r\n");
				foreach (var property in type.propertyAliasses)
				{
					Write("\tstring " + property + " { get; }\r\n");
				}
				Write("}\r\n\r\n");

				Write(@"
	internal class " + type.name + @"AliassesService : I" + type.name + @"AliassesService
	{
		public " + type.name + @"AliassesService(IContentTypeAliassesXmlService contentTypeAliassesXmlService)
		{
			_xml = contentTypeAliassesXmlService.Get()." + type.name + @";
		}

		private " + type.name + @"AliassesXML _xml;

		public string ContentTypeAlias { get { return _xml.ContentTypeAlias ?? """ + type.alias + @"""; } }
");
				foreach (var property in type.propertyAliasses)
				{
					Write("public string " + property + " { get { return _xml." + property + " ?? \"" + property + "\"; } }\r\n");
				}
				Write("}\r\n\r\n");

				Write("[XmlRoot(ElementName = \"" + type.name + "\")]\r\n");
				Write("public struct " + type.name + "AliassesXML {\r\n[XmlAttribute(\"alias\")]\r\npublic string ContentTypeAlias;\r\n");
				foreach (var property in type.propertyAliasses)
				{
					Write("\tpublic string " + property + ";\r\n");
				}
				Write("}\r\n\r\n");
			}

			Write("[XmlRoot(ElementName = \"Config\")]\r\npublic struct UwebshopAliassesXMLConfig\r\n{\r\n");
			foreach (var type in types)
			{
				Write("public " + type.name + "AliassesXML " + type.name + ";\r\n");
			}
			Write("}\r\n");

			Write("public static class ModuleFunctionality\r\n{\r\npublic static void Register(IIocContainerConfiguration container)\r\n{\r\n");
			foreach (var type in types)
			{
				Write("container.RegisterType<I" + type.name + "AliassesService, " + type.name + "AliassesService>();\r\n");
			}
			Write("}\r\n\r\n}public static class InitNodeAliasses{\r\n\r\npublic static void Initialize(UwebshopAliassesXMLConfig aliasses)\r\n{\r\n");
			foreach (var type in types)
			{
				Write(type.clrTypeName + ".NodeAlias = aliasses." + type.name + ".ContentTypeAlias ?? \"" + type.alias + "\";\r\n");
			}
			Write("}\r\n");

			Write("}\r\npublic static class GenerateXML {");
			Write("public static string GenerateXMLString()\r\n{\r\nvar xml = new UwebshopAliassesXMLConfig();\r\n");
			foreach (var type in types)
			{
				Write("xml." + type.name + ".ContentTypeAlias = \"" + type.alias + "\";\r\n");
				foreach (var property in type.propertyAliasses)
				{
					Write("xml." + type.name + "." + property + " = \"" + property + "\";\r\n");
				}
			}
			Write(@"	var settings = new XmlWriterSettings();
	settings.OmitXmlDeclaration = true;
	settings.ConformanceLevel = ConformanceLevel.Document;
	settings.CloseOutput = false;
	settings.Indent = true;
	using (var writer = new System.IO.StringWriter())
		{
			var writerr = XmlWriter.Create(writer, settings);
			var x = new System.Xml.Serialization.XmlSerializer(xml.GetType());
			
			x.Serialize(writerr, xml);
				
			writerr.Flush();
			writerr.Close();
			return writer.ToString();
		}
	}" + "\r\n}\r\n\r\n");

			return generatedCode.ToString();
		}
		public string GenerateXMLFile()
		{
			var p = new UwebshopAliassesXMLConfig();
			p.Product.ContentTypeAlias = "uwbsProduct";
			p.Product.title = "titel";
			p.Product.sku = "artikelnr";
			var x = new System.Xml.Serialization.XmlSerializer(p.GetType());
			using (var writer = new StringWriter())
			{
				x.Serialize(writer, p);
				return writer.ToString();
			}
		}

		private readonly StringBuilder generatedCode = new StringBuilder();

		private void Write(string s)
		{
			generatedCode.Append(s);
		}
	}
}