using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using uWebshop.Domain.Interfaces;
using uWebshop.Newtonsoft.Json;

namespace uWebshop.API
{
	[DataContract(Namespace = "")]
	internal class VariantGroupAdaptor : IProductVariantGroup
	{
		private readonly IProductVariantGroup _source;
        
		public VariantGroupAdaptor(IProductVariantGroup source)
		{
			_source = source;
		}

        [JsonProperty]
        public IEnumerable<IProductVariant> Variants { get { return _source.Variants.Select(v => new VariantAdaptor(v)); } }
        [JsonProperty]
        public string Title { get { return _source.Title; } }
        [JsonProperty]
        public string Description { get { return _source.Description; } }
        [JsonProperty]
        public bool Required { get { return _source.Required; } }
        [JsonProperty]
        public int Id { get { return _source.Id; } }
        [JsonProperty]
        public string TypeAlias { get { return _source.TypeAlias; } }
        [JsonProperty]
        public bool Disabled { get { return _source.Disabled; } }
        public DateTime CreateDate { get { return _source.CreateDate; } }
        [JsonProperty]
        public DateTime UpdateDate { get { return _source.UpdateDate; } }
        [JsonProperty]
        public int SortOrder { get { return _source.SortOrder; } }
        [JsonProperty]
        public string NodeTypeAlias { get { return _source.NodeTypeAlias; } }
        [JsonProperty]
        public string Name { get { return _source.Name; } }
        [JsonProperty]
        public int ParentId { get { return _source.ParentId; } }
        [JsonProperty]
        public string Path { get { return _source.Path; } }
	}
}