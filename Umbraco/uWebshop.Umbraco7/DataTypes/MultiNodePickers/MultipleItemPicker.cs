using System.Collections.Generic;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.PropertyEditors;

namespace uWebshop.Umbraco7.DataTypes.MultiNodePickers
{
    [PropertyEditor("uWebshop.MultiNodePicker", "uWebshop MultiNodePicker", "contentpicker")]
    public sealed class MultipleItemPicker : MultiNodeTreePickerPropertyEditor
    {
        public MultipleItemPicker()
        {
            DefaultPreValues = new Dictionary<string, object>
                {
                    //{"multiPicker", "1"},
                    { "startNode",  
                        new MultiNodePickerPreValues {
                            type = "content",
                            query = "//*"
                        }  
                    }
                };
        }

        protected override global::Umbraco.Core.PropertyEditors.PreValueEditor CreatePreValueEditor()
        {
            return new MultiNodePickerPreValueEditor();
        }
    }

}
