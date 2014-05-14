using System.Collections.Generic;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;

namespace uWebshop.Umbraco7.DataTypes.MultiNodePickers
{
    internal class MultiNodePickerPreValueEditor : PreValueEditor
    {
        [PreValueField("startNode", "Node type", "treesource")]
        public string StartNode { get; set; }

        [PreValueField("filter", "Filter out items with type", "textstring", Description = "Seperate with comma")]
        public string Filter { get; set; }

        [PreValueField("minNumber", "Minimum number of items", "number")]
        public string MinNumber { get; set; }

        [PreValueField("maxNumber", "Maximum number of items", "number")]
        public string MaxNumber { get; set; }

        // No longer required for v7, but Umbraco still wants to convert it. So...
        public override IDictionary<string, object> ConvertDbToEditor(IDictionary<string, object> defaultPreVals, PreValueCollection persistedPreVals)
        {
            return defaultPreVals;
        }

    }
}
