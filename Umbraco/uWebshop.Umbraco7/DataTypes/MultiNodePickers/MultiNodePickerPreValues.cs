namespace uWebshop.Umbraco7.DataTypes.MultiNodePickers
{
    internal class MultiNodePickerPreValues
    {
        public string @type { get; set; }
        public string query { get; set; }
        public string maxNumber { get { return "-1"; } }
    }
}