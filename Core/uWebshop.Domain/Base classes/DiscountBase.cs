using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using uWebshop.Common;
using uWebshop.Common.Interfaces;
using uWebshop.Domain.ContentTypes;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain.BaseClasses
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Namespace = "", IsReference = true)]
    public abstract class DiscountBase : uWebshopEntity // MultiStoreUwebshopContent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiscountBase"/> class.
        /// </summary>
        /// <param name="nodeId">The node unique identifier.</param>
        protected DiscountBase(int nodeId) : base(nodeId)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscountBase"/> class.
        /// </summary>
        protected DiscountBase()
        {
        }

        /// <summary>
        /// Gets the title of the content
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        [DataMember]
        [ContentPropertyType(Alias = "title", DataType = DataType.String, Tab = ContentTypeTab.Global, Name = "#Title", Description = "#TitleDescription", Mandatory = true, SortOrder = 1)]
        public string Title { get; set; }

        /// <summary>
        /// Gets the long description of the content
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [DataMember]
        [ContentPropertyType(Alias = "description", DataType = DataType.RichText, Tab = ContentTypeTab.Global, Name = "#Description", Description = "#DescriptionDescription", Mandatory = false, SortOrder = 2)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [disable].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [disable]; otherwise, <c>false</c>.
        /// </value>
        [ContentPropertyType(Alias = "disable", DataType = DataType.TrueFalse, Tab = ContentTypeTab.Global, Name = "#Disable", Description = "#DisableDescription", SortOrder = 3)]
        public override bool Disabled { get; set; }

        /// <summary>
        /// Ranges from the range data type
        /// </summary>
        /// <value>
        /// The ranges.
        /// </value>
        [DataMember]
        public List<Range> Ranges
        {
            get { return Range.CreateFromString(RangesString); }
            set { }
        }

        #region global tab

        /// <summary>
        /// Type of discount (Percentage, Amount, Free shipping)
        /// </summary>
        /// <value>
        /// The type of the discount.
        /// </value>
        [ContentPropertyType(Alias = "discountType", DataType = DataType.DiscountType, Tab = ContentTypeTab.Details, Name = "#DiscountType", Description = "#DiscountTypeDescription", SortOrder = 4)]
        public DiscountType DiscountType { get; set; }

        /// <summary>
        /// The discount value in cents or percentage
        /// </summary>
        /// <value>
        /// The discount value.
        /// </value>
        [ContentPropertyType(Alias = "discount", DataType = DataType.Price, Tab = ContentTypeTab.Details, Name = "#Discount", Description = "#DiscountDescription", SortOrder = 5)]
        public int DiscountValue { get; set; }

        /// <summary>
        /// Gets the ranged discount value.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public int RangedDiscountValue(int count)
        {
            var range = Ranges.FindRangeForValue(count);
            return range != null ? range.PriceInCents : DiscountValue;
        }

        /// <summary>
        /// Gets or sets the ranges string.
        /// </summary>
        /// <value>
        /// The ranges string.
        /// </value>
        [ContentPropertyType(Alias = "ranges", DataType = DataType.Ranges, Tab = ContentTypeTab.Details, Name = "#Ranges", Description = "#RangesDescription", SortOrder = 5)]
        public string RangesString { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [counter enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [counter enabled]; otherwise, <c>false</c>.
        /// </value>
        [ContentPropertyType(Alias = "countdownEnabled", DataType = DataType.TrueFalse, Tab = ContentTypeTab.Details, Name = "#CountdownEnabled", Description = "#CountdownEnabledDescription", SortOrder = 6)]
        public bool CounterEnabled { get; set; }

        /// <summary>
        /// Gets or sets the counter.
        /// </summary>
        /// <value>
        /// The counter.
        /// </value>
        [ContentPropertyType(Alias = "countdown", DataType = DataType.Stock, Tab = ContentTypeTab.Details, Name = "#Countdown", Description = "#CountdownDescription", SortOrder = 7)]
        public int Counter
        {
            get { return StoreHelper.GetMultiStoreStock(Id); }
            set { }
        }

        #endregion

        #region condition tab

        /// <summary>
        /// Membergroups this discount is valid for
        /// </summary>
        /// <value>
        /// The member groups.
        /// </value>
        [ContentPropertyType(Alias = "memberGroups", DataType = DataType.MemberGroups, Tab = ContentTypeTab.Conditions, Name = "#MemberGroups", Description = "#MemberGroupsDescription", SortOrder = 99)]
        public List<string> MemberGroups { get; set; }

        #endregion
    }
}