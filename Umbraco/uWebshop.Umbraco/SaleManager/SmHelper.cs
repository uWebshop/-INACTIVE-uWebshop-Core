using Examine;
using Examine.SearchCriteria;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace uWebshop.Umbraco.SaleManager
{
    public static class SmHelper
    {
        private static readonly ILog Log =
        LogManager.GetLogger(
            MethodBase.GetCurrentMethod().DeclaringType
        );

        public static IEnumerable<SearchResult> GetAllCatalogItemsFromPath(string path)
        {
            var list = new List<SearchResult>();

            var pathArray = path.Split(',');

            var Ids = pathArray.Skip(3);

            foreach (var id in Ids)
            {
                var examineItem = GetNodeFromExamine(Convert.ToInt32(id));

                list.Add(examineItem);
            }

            return list;
        }
        public static SearchResult GetNodeFromExamine(int id)
        {

            var searcher = ExamineManager.Instance.SearchProviderCollection["ExternalSearcher"];

            if (searcher != null)
            {

                ISearchCriteria searchCriteria = searcher.CreateSearchCriteria();
                var query = searchCriteria.Id(id);
                var result = searcher.Search(query.Compile());

                if (result.Any())
                {
                    return result.FirstOrDefault();
                }
                else
                {
                    Log.Debug("GetNodeFromExamine Failed. Node with Id " + id + " not found.");
                }

            }

            return null;
        }
        public static bool IsItemDisabled(IEnumerable<SearchResult> items, string store)
        {

            foreach (var item in items)
            {
                if (item != null) {
                    var disableField = GetProperty(item, "disable", store);

                    if (!string.IsNullOrEmpty(disableField))
                    {
                        if (disableField == "1" || disableField.ToLower() == "true")
                        {
                            return true;
                        }
                    }
                } else
                {
                    return true;
                }
            }

            return false;
        }
        public static string GetProperty(SearchResult item, string field, string storeAlias)
        {

            var fieldExist = item.Fields.Any(x => x.Key == field + "_" + storeAlias);

            if (fieldExist)
            {
                // temp fix for 66north  2 disable fields. 'disable' && 'disable_IS'
                var value = item.Fields[field + "_" + storeAlias];

                if ((string.IsNullOrEmpty(value) || value == "0") && storeAlias.ToLower() == "is")
                {
                    value = item.Fields.Any(x => x.Key == field) ? item.Fields[field] : "";
                }

                return value;
            }
            else
            {
                return item.Fields.Any(x => x.Key == field) ? item.Fields[field] : "";
            }

        }
    }
}
