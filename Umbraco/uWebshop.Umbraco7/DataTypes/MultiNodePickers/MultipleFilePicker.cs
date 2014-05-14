using System.Collections.Generic;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.PropertyEditors;

namespace uWebshop.Umbraco7.DataTypes.MultiNodePickers
{
    [PropertyEditor("uWebshop.MultiContentPickerFiles", "uWebshop Multiple File Picker", "mediapicker")]
    public sealed class MultipleFilePicker : MultipleMediaPickerPropertyEditor
    {
        public MultipleFilePicker()
        {
            DefaultPreValues = new Dictionary<string, object>
                {
                    {"multiPicker", "1"}
                };
        }
    }

}
