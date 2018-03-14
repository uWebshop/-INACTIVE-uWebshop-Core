using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web;
using umbraco.BusinessLogic.Actions;
using Umbraco.Core;
using Umbraco.Web;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;

namespace uWebshop.Umbraco.SaleManager
{
    [Tree("saleManager", "saleManager", "Sale Manager")]
    [PluginController("SaleManager")]
    public class SmTreeController : TreeController
    {
        protected override TreeNodeCollection GetTreeNodes(string id, FormDataCollection queryStrings)
        {

            if (id == Constants.System.Root.ToInvariantString())
            {
                var tree = new TreeNodeCollection();

                tree.Add(CreateTreeNode("dashboard", id, queryStrings, "Dashboard", "icon-dashboard", true));

                return tree;
            }

            if (id == "dashboard")
            {
                var tree = new TreeNodeCollection();

                tree.Add(CreateTreeNode("orders", id, queryStrings, "Orders", "icon-uwebshop-box", true, FormDataCollectionExtensions.GetValue<string>(queryStrings, "application") + StringExtensions.EnsureStartsWith(this.TreeAlias, '/') + "/orders/all"));
                tree.Add(CreateTreeNode("customers", id, queryStrings, "Customers", "icon-users", false, FormDataCollectionExtensions.GetValue<string>(queryStrings, "application") + StringExtensions.EnsureStartsWith(this.TreeAlias, '/') + "/customers/list"));
                tree.Add(CreateTreeNode("stock", id, queryStrings, "Stock", "icon-uwebshop-list", false, FormDataCollectionExtensions.GetValue<string>(queryStrings, "application") + StringExtensions.EnsureStartsWith(this.TreeAlias, '/') + "/stock/list"));

                return tree;
            }

            if (id == "orders")
            {
                var tree = new TreeNodeCollection();

                tree.Add(CreateTreeNode("AllOrders", id, queryStrings, "AllOrders", "icon-uwebshop-statistics", false, FormDataCollectionExtensions.GetValue<string>(queryStrings, "application") + StringExtensions.EnsureStartsWith(this.TreeAlias, '/') + "/orders/all"));
                tree.Add(CreateTreeNode("ReadyForDispatch", id, queryStrings, "ReadyForDispatch", "icon-uwebshop-statistics", false, FormDataCollectionExtensions.GetValue<string>(queryStrings, "application") + StringExtensions.EnsureStartsWith(this.TreeAlias, '/') + "/orders/readyForDispatch"));
                tree.Add(CreateTreeNode("OfflinePayment", id, queryStrings, "OfflinePayment", "icon-uwebshop-statistics", false, FormDataCollectionExtensions.GetValue<string>(queryStrings, "application") + StringExtensions.EnsureStartsWith(this.TreeAlias, '/') + "/orders/offlinePayment"));
                tree.Add(CreateTreeNode("Dispatched", id, queryStrings, "Dispatched", "icon-uwebshop-statistics", false, FormDataCollectionExtensions.GetValue<string>(queryStrings, "application") + StringExtensions.EnsureStartsWith(this.TreeAlias, '/') + "/orders/dispatched"));
                tree.Add(CreateTreeNode("CompletedOrders", id, queryStrings, "CompletedOrders", "icon-uwebshop-statistics", false, FormDataCollectionExtensions.GetValue<string>(queryStrings, "application") + StringExtensions.EnsureStartsWith(this.TreeAlias, '/') + "/orders/completedOrders"));
                tree.Add(CreateTreeNode("Closed", id, queryStrings, "Closed", "icon-uwebshop-statistics", false, FormDataCollectionExtensions.GetValue<string>(queryStrings, "application") + StringExtensions.EnsureStartsWith(this.TreeAlias, '/') + "/orders/closed"));
                tree.Add(CreateTreeNode("RefundedOrders", id, queryStrings, "RefundedOrders", "icon-uwebshop-statistics", false, FormDataCollectionExtensions.GetValue<string>(queryStrings, "application") + StringExtensions.EnsureStartsWith(this.TreeAlias, '/') + "/orders/refundedOrders"));
                tree.Add(CreateTreeNode("WaitingForPayment", id, queryStrings, "WaitingForPayment", "icon-uwebshop-statistics", false, FormDataCollectionExtensions.GetValue<string>(queryStrings, "application") + StringExtensions.EnsureStartsWith(this.TreeAlias, '/') + "/orders/waitingForPayment"));
                tree.Add(CreateTreeNode("Incomplete", id, queryStrings, "Incomplete", "icon-uwebshop-statistics", false, FormDataCollectionExtensions.GetValue<string>(queryStrings, "application") + StringExtensions.EnsureStartsWith(this.TreeAlias, '/') + "/orders/incomplete"));
                tree.Add(CreateTreeNode("AbandondBaskets", id, queryStrings, "AbandondBaskets", "icon-uwebshop-statistics", false, FormDataCollectionExtensions.GetValue<string>(queryStrings, "application") + StringExtensions.EnsureStartsWith(this.TreeAlias, '/') + "/orders/abandondBaskets"));

                return tree;
            }

            throw new NotSupportedException();
        }

        protected override MenuItemCollection GetMenuForNode(string id, FormDataCollection queryStrings)
        {
            var menu = new MenuItemCollection();

            menu.DefaultMenuAlias = ActionRefresh.Instance.Alias;
            menu.Items.Add<ActionRefresh>("Refresh");

            return menu;
        }
    }
}
