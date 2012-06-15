using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Cocoon.Data;
using System.Threading.Tasks;

namespace Cocoon.Tests.Data
{
    [TestClass]
    public class DataListSourceFixture_Skip
    {
        // *** Constructor Method Tests ***

        [TestMethod]
        public void Skip_Exception_IfSourceIsNull()
        {
            MockDataListSource source = null;

            Assert.ThrowsException<ArgumentNullException>(() => source.Take(20));
        }

        [TestMethod]
        public void Skip_Exception_IfCountIsLessThanZero()
        {
            MockDataListSource source = new MockDataListSource(10);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => source.Take(-1));
        }

        // *** Method Tests ***

        [TestMethod]
        public void GetCount_ReturnsItemsFromListMinusCount()
        {
            MockDataListSource source = new MockDataListSource(10);
            IDataListSource<int> takeSource = source.Skip(4);

            int count = takeSource.GetCountAsync().Result;

            Assert.AreEqual(6, count);
        }

        [TestMethod]
        public void GetCount_ReturnsZeroIfSkipBeyondList()
        {
            MockDataListSource source = new MockDataListSource(10);
            IDataListSource<int> takeSource = source.Skip(15);

            int count = takeSource.GetCountAsync().Result;

            Assert.AreEqual(0, count);
        }

        [TestMethod]
        public void GetItem_ReturnsCorrectItem()
        {
            MockDataListSource source = new MockDataListSource(10);
            IDataListSource<int> takeSource = source.Skip(5);

            int item = takeSource.GetItemAsync(2).Result;

            Assert.AreEqual(8, item);
        }

        [TestMethod]
        public void GetItem_Exception_IfIndexIsLessThanZero()
        {
            MockDataListSource source = new MockDataListSource(10);
            IDataListSource<int> takeSource = source.Skip(4);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => takeSource.GetItemAsync(-1));
        }

        [TestMethod]
        public void GetItem_Exception_IfIndexIsGreaterThanLength()
        {
            MockDataListSource source = new MockDataListSource(10);
            IDataListSource<int> takeSource = source.Skip(4);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => takeSource.GetItemAsync(6));
        }

        // *** Private Sub-Classes ***

        private class MockDataListSource : IDataListSource<int>
        {
            // *** Fields ***

            private int count;

            // *** Constructors ***

            public MockDataListSource(int count)
            {
                this.count = count;
            }

            // *** Methods ***

            public async Task<int> GetCountAsync()
            {
                await Task.Yield();

                return count;
            }

            public Task<int> GetItemAsync(int index)
            {
                if (index < 0 || index >= count)
                    throw new ArgumentOutOfRangeException();

                return GetItemInternalAsync(index);
            }

            public async Task<int> GetItemInternalAsync(int index)
            {
                await Task.Yield();

                return index + 1;
            }
        }
    }
}