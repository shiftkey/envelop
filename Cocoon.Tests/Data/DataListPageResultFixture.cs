using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cocoon.Data;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Concoon.Tests.Data
{
    [TestClass]
    public class DataListPageResultFixture
    {
        // *** Constructor Tests ***

        [TestMethod]
        public void Constructor_SetsProperties()
        {
            DataListPageResult<int> dataListPageResult = new DataListPageResult<int>(42, 5, 2, new int[] { 1, 2, 3, 4, 5 });

            Assert.AreEqual(42, dataListPageResult.TotalItemCount);
            Assert.AreEqual(5, dataListPageResult.ItemsPerPage);
            Assert.AreEqual(2, dataListPageResult.PageNumber);
            CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4, 5 }, dataListPageResult.Page.ToArray());
        }
    }
}
