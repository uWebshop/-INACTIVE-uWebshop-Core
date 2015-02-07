using System.Collections.Generic;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.PropertyEditors;

namespace uWebshop.Umbraco7.DataTypes.MultiNodePickers
{
	[PropertyEditor("uWebshop.MultiContentPickerCategories", "uWebshop Multiple Category Picker", "contentpicker")]
	public sealed class MultipleCategoryPicker : MultiNodeTreePickerPropertyEditor
	{
		public MultipleCategoryPicker()
		{
			DefaultPreValues = new Dictionary<string, object>
				{
					//{"multiPicker", "1"},
					{ "startNode",  
						new MultiNodePickerPreValues {
							type = "content",
							query = "//uwbsCategoryRepository"
						}  
					},
					{ "filter",  "uwbsCategory" }
				};
		}

		protected override global::Umbraco.Core.PropertyEditors.PreValueEditor CreatePreValueEditor()
		{
			return new MultiNodePickerPreValueEditor();
		}
	}
}
