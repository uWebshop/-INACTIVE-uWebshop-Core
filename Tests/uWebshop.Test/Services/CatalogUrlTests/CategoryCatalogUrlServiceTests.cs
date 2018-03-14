using NUnit.Framework;
using System;
using System.Collections.Generic;
using uWebshop.Common.Interfaces;

namespace uWebshop.Test.Services.CatalogUrlTests
{
    [TestFixture]
    public class CategoryCatalogUrlServiceTests
    {
        private ICategoryCatalogUrlService _categoryCatalogUrlService;
        private MockCategory _cat1;
        private MockCategory _cat2;
        private MockCategory _cat3;

        [SetUp]
        public void Setup()
        {
            IOC.UnitTest();
            _categoryCatalogUrlService = IOC.CategoryCatalogUrlService.Actual().Resolve();
            _cat1 = new MockCategory { UrlName = "cat1" };
            _cat2 = new MockCategory { UrlName = "cat2" };
            _cat3 = new MockCategory { UrlName = "cat3" };
        }

        [Test]
        public void GetUrl_ForSingleRootCategory_ShouldBeUrlName()
        {
            var actual = _categoryCatalogUrlService.GetCanonicalUrl(_cat1);

            Assert.AreEqual("cat1", actual);
        }

        [Test]
        public void GetUrl_ForTwoCategories_ShouldBeUrlNamesAppendedBySlash()
        {
            _cat2.Parent = _cat1;

            var actual = _categoryCatalogUrlService.GetCanonicalUrl(_cat2);

            Assert.AreEqual("cat1/cat2", actual);
        }

        [Test]
        public void GetUrl_ForThreeCategories_ShouldBeUrlNamesAppendedBySlash()
        {
            _cat3.Parent = _cat2;
            _cat2.Parent = _cat1;

            var actual = _categoryCatalogUrlService.GetCanonicalUrl(_cat3);

            Assert.AreEqual("cat1/cat2/cat3", actual);
        }

        private class MockCategory : ICategory
        {
            public string UrlName { get; set; }

            #region not implemented

            public int Id
            {
                get { throw new NotImplementedException(); }
            }
            public Guid Key => throw new NotImplementedException();

            public string TypeAlias { get; private set; }

            public string ParentNodeTypeAlias
            {
                get { throw new NotImplementedException(); }
            }


            public IEnumerable<ICategory> ParentCategories { get; private set; }

            public IEnumerable<ICategory> SubCategories
            {
                get { throw new NotImplementedException(); }
            }

            public ICategory Parent { get; set; }

            public IEnumerable<IProduct> Products
            {
                get { throw new NotImplementedException(); }
            }

            #endregion
        }
    }
}