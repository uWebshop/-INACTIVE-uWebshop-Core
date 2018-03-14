using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using umbraco.businesslogic;
using umbraco.interfaces;
using Umbraco.Web.Mvc;

namespace uWebshop.Umbraco.SaleManager
{
    [Application("saleManager", "SaleManager", "icon-uwebshop-statistics", 10)]
    [PluginController("SaleManager")]
    public class SmApplication : IApplication
    {
    }
}
