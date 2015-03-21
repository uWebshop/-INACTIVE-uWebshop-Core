using System.Collections.Generic;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.PropertyEditors;

namespace uWebshop.Umbraco7.DataTypes.MultiNodePickers
{
	[PropertyEditor("uWebshop.MultiContentPickerPaymentZones", "uWebshop Payment Zone Picker", "contentpicker")]
	public sealed class MultiplePaymentZonePicker : MultiNodeTreePickerPropertyEditor
	{
		public MultiplePaymentZonePicker()
		{
			DefaultPreValues = new Dictionary<string, object>
				{
					//{"multiPicker", "1"},
					{ "startNode",  
						new MultiNodePickerPreValues {
							type = "content",
							query = "//uwbsPaymentProviderZoneSection"
						}  
					},
					{ "filter",  "uwbsPaymentProviderZone" }
				};
		}

		protected override PreValueEditor CreatePreValueEditor()
		{
			return new MultiNodePickerPreValueEditor();
		}

	}

	[PropertyEditor("uWebshop.MultiContentPickerShippingZones", "uWebshop Shipping Zone Picker", "contentpicker")]
	public sealed class MultipleShippingZonePicker : MultiNodeTreePickerPropertyEditor
	{
		public MultipleShippingZonePicker()
		{
			DefaultPreValues = new Dictionary<string, object>
				{
					//{"multiPicker", "1"},
					{ "startNode",  
						new MultiNodePickerPreValues {
							type = "content",
							query = "//uwbsShippingProviderZoneSection"
						}  
					},
					{ "filter",  "uwbsShippingProviderZone" }
				};
		}

		protected override PreValueEditor CreatePreValueEditor()
		{
			return new MultiNodePickerPreValueEditor();
		}

	}
}
