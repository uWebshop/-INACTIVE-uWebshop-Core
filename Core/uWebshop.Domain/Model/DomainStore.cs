using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uWebshop.Domain.Model
{
    public class DomainStore
    {
        public DomainStore(int storeId, string domainUrl, string storeAlias)
        {
            StoreId = storeId;
            DomainUrl = domainUrl;
            StoreAlias = storeAlias;
        }

        public int StoreId { get; set; }
        public string DomainUrl { get; set; }
        public string StoreAlias { get; set; }
    }
}
