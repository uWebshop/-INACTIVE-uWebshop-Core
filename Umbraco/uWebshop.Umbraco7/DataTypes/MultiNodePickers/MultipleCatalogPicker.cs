using System.Collections.Generic;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.PropertyEditors;

namespace uWebshop.Umbraco7.DataTypes.MultiNodePickers
{
	[PropertyEditor("uWebshop.MultiContentPickerCatalog", "uWebshop Multiple Catalog Picker", "contentpicker")]
	public sealed class MultipleCatalogPicker : MultiNodeTreePickerPropertyEditor
	{
		public MultipleCatalogPicker()
		{
			DefaultPreValues = new Dictionary<string, object>
				{
					//{"multiPicker", "1"},
					{ "startNode",  
						new MultiNodePickerPreValues {
							type = "content",
							query = "//uwbsCatalog"
						}  
					},
					{ "filter",  "uwbsCategory, uwbsProduct, uwbsProductVariant" }
				};
		}

		protected override PreValueEditor CreatePreValueEditor()
		{
			return new MultiNodePickerPreValueEditor();
		}
	}
}
