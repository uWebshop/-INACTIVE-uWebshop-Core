using System.Runtime.Serialization;
using System.Xml.Serialization;
using Umbraco.Web;
using uWebshop.Domain.ContentTypes;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain.BaseClasses
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Name = "MultiStoreUwebshopContent", Namespace = "", IsReference = true)]
    public class MultiStoreUwebshopContent : uWebshopEntity
    {
        private int? _template;

        /// <summary>
        ///     Gets the title of the content
        /// </summary>
        [DataMember]
        [ContentPropertyType(Alias = "title", DataType = DataType.String, Tab = ContentTypeTab.Global, Name = "#Title", Description = "#TitleDescription", Mandatory = true, SortOrder = 1)]
        public string Title { get; set; }

        /// <summary>
        ///     Gets the URL of the content
        /// </summary>
        [XmlIgnore]
        [ContentPropertyType(Alias = "url", DataType = DataType.String, Tab = ContentTypeTab.Global, Name = "#Url", Description = "#UrlDescription", Mandatory = true, SortOrder = 2)]
        public string URL { get; set; }

        /// <summary>
        ///     Gets the short description of the content
        /// </summary>
        [DataMember]
        [ContentPropertyType(Alias = "metaDescription", DataType = DataType.TextboxMultiple, Tab = ContentTypeTab.Global, Name = "#MetaDescription", Description = "#MetaDescriptionDescription", Mandatory = false, SortOrder = 3)]
        public string MetaDescription { get; set; }

        /// <summary>
        ///     Gets the long description of the content
        /// </summary>
        [DataMember]
        [ContentPropertyType(Alias = "description", DataType = DataType.RichText, Tab = ContentTypeTab.Details, Name = "#Description", Description = "#DescriptionDescription", Mandatory = false, SortOrder = 6)]
        public string Description { get; set; }

        /// <summary>
        ///     Gets the template of the product
        /// </summary>
        [DataMember]
        public int Template
        {
            get
            {
                // template works differently in umbraco, if = 0 then get it from the contenttype
                if (_template.HasValue && _template.GetValueOrDefault() != 0) return _template.GetValueOrDefault();
                return (_template = Node.template).GetValueOrDefault();
            }
            set { }
        }
        internal void SetTemplate(int template)
        {
            _template = template;
        }

        /// <summary>
        ///     Is this content enabled?
        /// </summary>
        [DataMember]
        [ContentPropertyType(Alias = "disable", DataType = DataType.TrueFalse, Tab = ContentTypeTab.Global, Name = "#Disable", Description = "#DisableDescription", Mandatory = false, SortOrder = 7)]
        public override bool Disabled { get; set; }

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <param name="propertyAlias">The property alias.</param>
        /// <returns></returns>
        public string GetProperty(string propertyAlias)
        {
            if (!string.IsNullOrEmpty(propertyAlias))
            {
                if (Node != null)
                {
                    var property = Node.GetMultiStoreItem(propertyAlias);

                    if (property != null)
                    {
                        return property.Value;
                    }
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the propertyValue.
        /// </summary>
        /// <param name="propertyAlias">The property alias.</param>
        /// <returns></returns>
        public T GetPropertyValue<T>(string propertyAlias)
        {
            if (!string.IsNullOrEmpty(propertyAlias))
            {
                var helper = new UmbracoHelper(UmbracoContext.Current);

                var node = helper.TypedContent(Id);

                if (node != null && node.HasProperty(propertyAlias))
                {
                    var property = node.GetProperty(propertyAlias);

                    return property == null ? default(T) : (T)property.Value;
                }

            }

            return default(T);
        }
    }
}