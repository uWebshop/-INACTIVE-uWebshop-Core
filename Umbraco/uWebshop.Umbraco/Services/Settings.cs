using System;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;
using uWebshop.Umbraco.Repositories;
using umbraco.NodeFactory;

namespace uWebshop.Umbraco.Services
{
	internal class SettingsLoader
	{
		internal static ISettings GetSettings()
		{
			return CreateSettingsFromNode(new Node(Helpers.GetNodeIdForDocument(Settings.NodeAlias, "Settings")));
		}

		internal static Settings CreateSettingsFromNode(Node node)
		{
			if (node == null) throw new Exception("Trying to load data from null node");

			var product = new Settings();
			LoadDataFromNode(product, node);
			return product;
		}

		internal static void LoadDataFromNode(Settings settings, Node node)
		{
			Helpers.LoadUwebshopEntityPropertiesFromNode(settings, node);
			LoadDataFromPropertiesDictionary(settings, new UmbracoNodePropertyProvider(node));
		}

		private static void LoadDataFromPropertiesDictionary(Settings settings, IPropertyProvider fields)
		{
			int incomleteOrderLifetime = 360;
			string stringValue = null;
			if (fields.UpdateValueIfPropertyPresent("incompleteOrderLifetime", ref stringValue))
			{
				int.TryParse(stringValue, out incomleteOrderLifetime);
			}
			settings.IncompleteOrderLifetime = incomleteOrderLifetime;

			string property = null;
			fields.UpdateValueIfPropertyPresent("includingVat", ref property);
			settings.IncludingVat = property == "1" || property == "true";

			fields.UpdateValueIfPropertyPresent("lowercaseUrls", ref property);
			settings.UseLowercaseUrls = property == "1" || property == "true";
		}
	}
}