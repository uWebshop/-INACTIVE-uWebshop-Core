using System.Collections.Generic;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.PropertyEditors;

namespace uWebshop.Umbraco7.DataTypes.MultiNodePickers
{
	[PropertyEditor("uWebshop.MultiContentPickerProducts", "uWebshop Multiple Product Picker", "contentpicker")]
	public sealed class MultipleProductPicker : MultiNodeTreePickerPropertyEditor
	{
		public MultipleProductPicker()
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
					{ "filter",  "uwbsProduct" }
				};
		}

		protected override global::Umbraco.Core.PropertyEditors.PreValueEditor CreatePreValueEditor()
		{
			return new MultiNodePickerPreValueEditor();
		}
	}

}
