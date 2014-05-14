using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using uWebshop.Domain.Interfaces;

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

		public IEnumerable<IProductVariant> Variants { get { return _source.Variants.Select(v => new VariantAdaptor(v)); } }
		public string Title { get { return _source.Title; } }
	    public string Description { get { return _source.Description; } }
	    public bool Required { get { return _source.Required; } }
		public int Id { get { return _source.Id; } }
		public string TypeAlias { get { return _source.TypeAlias; } }
		public bool Disabled { get { return _source.Disabled; } }
        public DateTime CreateDate { get { return _source.CreateDate; } }
        public DateTime UpdateDate { get { return _source.UpdateDate; } }
        public int SortOrder { get { return _source.SortOrder; } }
        public string NodeTypeAlias { get { return _source.NodeTypeAlias; } }
        public string Name { get { return _source.Name; } }
        public int ParentId { get { return _source.ParentId; } }
        public string Path { get { return _source.Path; } }
	}
}