using System.Collections.Generic;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.PropertyEditors;

namespace uWebshop.Umbraco7.DataTypes.MultiNodePickers
{
    [PropertyEditor("uWebshop.MultiContentPickerImages", "uWebshop Multiple Image Picker", "mediapicker")]
    public sealed class MultipleImagePicker : MultipleMediaPickerPropertyEditor
    {
        public MultipleImagePicker()
        {
            DefaultPreValues = new Dictionary<string, object>
            {
                {"multiPicker", "1"}
            };
        }
    }

}
