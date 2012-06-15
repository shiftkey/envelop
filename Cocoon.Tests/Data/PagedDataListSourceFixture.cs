using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cocoon.Data;
using Cocoon.Tests.Helpers;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Concoon.Tests.Data
{
    [TestClass]
    public class PagedDataListSourceFixture
    {
        // *** Method Tests ***

        [TestMethod]
        public void GetCount_ReturnsNumberOfItems()
        {
            MockDataListSource dataListSource = new MockDataListSource();

            int count = dataListSource.GetCountAsync().Result;

            Assert.AreEqual(14, count);
        }

        [TestMethod]
        public void GetCount_FullPage_ReturnsNumberOfItems()
        {
            MockFullPageDataListSource dataListSource = new MockFullPageDataListSource();

            int count = dataListSource.GetCountAsync().Result;

            Assert.AreEqual(14, count);
        }

        [TestMethod]
        public void GetCount_ReturnsSameValueWithParallelRequests()
        {
            MockDataListSource dataListSource = new MockDataListSource();

            Task<int> countTask1 = dataListSource.GetCountAsync();
            Task<int> countTask2 = dataListSource.GetCountAsync();
            Task<int> countTask3 = dataListSource.GetCountAsync();

            Task.WaitAll(countTask1, countTask2, countTask3);

            Assert.AreEqual(14, countTask1.Result);
            Assert.AreEqual(14, countTask2.Result);
            Assert.AreEqual(14, countTask3.Result);
        }

        [TestMethod]
        public void GetCount_ReturnsSameValueWithSubsequentRequests()
        {
            SynchronizationHelper.Run(async () =>
                {
                    MockDataListSource dataListSource = new MockDataListSource();

                    Task<int> countTask1 = dataListSource.GetCountAsync();
                    await countTask1;
                    Task<int> countTask2 = dataListSource.GetCountAsync();
                    await countTask2;
                    Task<int> countTask3 = dataListSource.GetCountAsync();
                    await countTask3;

                    Assert.AreEqual(14, countTask1.Result);
                    Assert.AreEqual(14, countTask2.Result);
                    Assert.AreEqual(14, countTask3.Result);
                });
        }

        [TestMethod]
        public void GetCount_OnlyFetchesItemsOnceWithParallelRequests()
        {
            MockDataListSource dataListSource = new MockDataListSource();

            Task<int> countTask1 = dataListSource.GetCountAsync();
            Task<int> countTask2 = dataListSource.GetCountAsync();
            Task<int> countTask3 = dataListSource.GetCountAsync();

            Task.WaitAll(countTask1, countTask2, countTask3);

            Assert.AreEqual(1, dataListSource.FetchCountCallCount);
            Assert.AreEqual(0, dataListSource.FetchPageSizeCallCount);
            Assert.AreEqual(0, dataListSource.FetchPageCallCount);
        }

        [TestMethod]
        public void GetCount_FullPage_OnlyFetchesItemsOnceWithParallelRequests()
        {
            MockFullPageDataListSource dataListSource = new MockFullPageDataListSource();

            Task<int> countTask1 = dataListSource.GetCountAsync();
            Task<int> countTask2 = dataListSource.GetCountAsync();
            Task<int> countTask3 = dataListSource.GetCountAsync();

            Task.WaitAll(countTask1, countTask2, countTask3);

            Assert.AreEqual(1, dataListSource.FetchCountCallCount);
            Assert.AreEqual(0, dataListSource.FetchPageSizeCallCount);
            Assert.AreEqual(0, dataListSource.FetchPageCallCount);
        }

        [TestMethod]
        public void GetCount_OnlyFetchesItemsOnceWithSubsequentRequests()
        {
            SynchronizationHelper.Run(async () =>
                {
                    MockDataListSource dataListSource = new MockDataListSource();

                    Task<int> countTask1 = dataListSource.GetCountAsync();
                    await countTask1;
                    Task<int> countTask2 = dataListSource.GetCountAsync();
                    await countTask2;
                    Task<int> countTask3 = dataListSource.GetCountAsync();
                    await countTask3;

                    Task.WaitAll(countTask1, countTask2, countTask3);

                    Assert.AreEqual(1, dataListSource.FetchCountCallCount);
                    Assert.AreEqual(0, dataListSource.FetchPageSizeCallCount);
                    Assert.AreEqual(0, dataListSource.FetchPageCallCount);
                });
        }

        [TestMethod]
        public void GetCount_FullPage_OnlyFetchesItemsOnceWithSubsequentRequests()
        {
            SynchronizationHelper.Run(async () =>
                {
                    MockFullPageDataListSource dataListSource = new MockFullPageDataListSource();

                    Task<int> countTask1 = dataListSource.GetCountAsync();
                    await countTask1;
                    Task<int> countTask2 = dataListSource.GetCountAsync();
                    await countTask2;
                    Task<int> countTask3 = dataListSource.GetCountAsync();
                    await countTask3;

                    Task.WaitAll(countTask1, countTask2, countTask3);

                    Assert.AreEqual(1, dataListSource.FetchCountCallCount);
                    Assert.AreEqual(0, dataListSource.FetchPageSizeCallCount);
                    Assert.AreEqual(0, dataListSource.FetchPageCallCount);
                });
        }

        [TestMethod]
        public void GetItem_ReturnsCorrectItem_FirstPage()
        {
            MockDataListSource dataListSource = new MockDataListSource();

            int item = dataListSource.GetItemAsync(3).Result;

            Assert.AreEqual(8, item);
        }

        [TestMethod]
        public void GetItem_ReturnsCorrectItem_MiddlePage()
        {
            MockDataListSource dataListSource = new MockDataListSource();

            int item = dataListSource.GetItemAsync(8).Result;

            Assert.AreEqual(18, item);
        }

        [TestMethod]
        public void GetItem_ReturnsCorrectItem_LastPage()
        {
            MockDataListSource dataListSource = new MockDataListSource();

            int item = dataListSource.GetItemAsync(12).Result;

            Assert.AreEqual(26, item);
        }

        [TestMethod]
        public void GetItem_FullPage_ReturnsCorrectItem_FirstPage()
        {
            MockFullPageDataListSource dataListSource = new MockFullPageDataListSource();

            int item = dataListSource.GetItemAsync(3).Result;

            Assert.AreEqual(8, item);
        }

        [TestMethod]
        public void GetItem_FullPage_ReturnsCorrectItem_MiddlePage()
        {
            MockFullPageDataListSource dataListSource = new MockFullPageDataListSource();

            int item = dataListSource.GetItemAsync(8).Result;

            Assert.AreEqual(18, item);
        }

        [TestMethod]
        public void GetItem_FullPage_ReturnsCorrectItem_LastPage()
        {
            MockFullPageDataListSource dataListSource = new MockFullPageDataListSource();

            int item = dataListSource.GetItemAsync(12).Result;

            Assert.AreEqual(26, item);
        }

        [TestMethod]
        public void GetItem_ReturnsSameValueWithParallelRequests()
        {
            MockDataListSource dataListSource = new MockDataListSource();

            Task<int> itemTask1 = dataListSource.GetItemAsync(8);
            Task<int> itemTask2 = dataListSource.GetItemAsync(8);
            Task<int> itemTask3 = dataListSource.GetItemAsync(8);

            Task.WaitAll(itemTask1, itemTask2, itemTask3);

            Assert.AreEqual(18, itemTask1.Result);
            Assert.AreEqual(18, itemTask2.Result);
            Assert.AreEqual(18, itemTask3.Result);
        }

        [TestMethod]
        public void GetItem_ReturnsSameValueWithSubsequentRequests()
        {
            SynchronizationHelper.Run(async () =>
                {
                    MockDataListSource dataListSource = new MockDataListSource();

                    Task<int> itemTask1 = dataListSource.GetItemAsync(8);
                    await itemTask1;
                    Task<int> itemTask2 = dataListSource.GetItemAsync(8);
                    await itemTask2;
                    Task<int> itemTask3 = dataListSource.GetItemAsync(8);
                    await itemTask3;

                    Assert.AreEqual(18, itemTask1.Result);
                    Assert.AreEqual(18, itemTask2.Result);
                    Assert.AreEqual(18, itemTask3.Result);
                });
        }

        [TestMethod]
        public void GetItem_OnlyFetchesItemsOnceWithParallelRequests()
        {
            SynchronizationHelper.Run(async () =>
                {
                    MockDataListSource dataListSource = new MockDataListSource();

                    Task<int> itemTask1 = dataListSource.GetItemAsync(8);
                    Task<int> itemTask2 = dataListSource.GetItemAsync(8);
                    Task<int> itemTask3 = dataListSource.GetItemAsync(8);

                    await itemTask1;
                    await itemTask2;
                    await itemTask3;

                    Assert.AreEqual(1, dataListSource.FetchCountCallCount);
                    Assert.AreEqual(1, dataListSource.FetchPageSizeCallCount);
                    Assert.AreEqual(1, dataListSource.FetchPageCallCount);
                });
        }

        [TestMethod]
        public void GetItem_OnlyFetchesItemsOnceWithSubsequentRequests()
        {
            SynchronizationHelper.Run(async () =>
                {
                    MockDataListSource dataListSource = new MockDataListSource();

                    Task<int> itemTask1 = dataListSource.GetItemAsync(8);
                    await itemTask1;
                    Task<int> itemTask2 = dataListSource.GetItemAsync(8);
                    await itemTask2;
                    Task<int> itemTask3 = dataListSource.GetItemAsync(8);
                    await itemTask3;

                    Task.WaitAll(itemTask1, itemTask2, itemTask3);

                    Assert.AreEqual(1, dataListSource.FetchCountCallCount);
                    Assert.AreEqual(1, dataListSource.FetchPageSizeCallCount);
                    Assert.AreEqual(1, dataListSource.FetchPageCallCount);
                });
        }

        [TestMethod]
        public void GetItem_FullPage_OnlyFetchesItemsOnceWithParallelRequests_FirstPage()
        {
            SynchronizationHelper.Run(async () =>
                {
                    MockDataListSource dataListSource = new MockDataListSource();

                    Task<int> itemTask1 = dataListSource.GetItemAsync(3);
                    Task<int> itemTask2 = dataListSource.GetItemAsync(3);
                    Task<int> itemTask3 = dataListSource.GetItemAsync(3);

                    await itemTask1;
                    await itemTask2;
                    await itemTask3;

                    Assert.AreEqual(1, dataListSource.FetchCountCallCount);
                    Assert.AreEqual(1, dataListSource.FetchPageSizeCallCount);
                    Assert.AreEqual(1, dataListSource.FetchPageCallCount);
                });
        }

        [TestMethod]
        public void GetItem_FullPage_OnlyFetchesItemsOnceWithSubsequentRequests_FirstPage()
        {
            SynchronizationHelper.Run(async () =>
                {
                    MockDataListSource dataListSource = new MockDataListSource();

                    Task<int> itemTask1 = dataListSource.GetItemAsync(3);
                    await itemTask1;
                    Task<int> itemTask2 = dataListSource.GetItemAsync(3);
                    await itemTask2;
                    Task<int> itemTask3 = dataListSource.GetItemAsync(3);
                    await itemTask3;

                    Task.WaitAll(itemTask1, itemTask2, itemTask3);

                    Assert.AreEqual(1, dataListSource.FetchCountCallCount);
                    Assert.AreEqual(1, dataListSource.FetchPageSizeCallCount);
                    Assert.AreEqual(1, dataListSource.FetchPageCallCount);
                });
        }

        [TestMethod]
        public void GetItem_FullPage_OnlyFetchesItemsOnceWithParallelRequests_MiddlePage()
        {
            SynchronizationHelper.Run(async () =>
                {
                    MockDataListSource dataListSource = new MockDataListSource();

                    Task<int> itemTask1 = dataListSource.GetItemAsync(8);
                    Task<int> itemTask2 = dataListSource.GetItemAsync(8);
                    Task<int> itemTask3 = dataListSource.GetItemAsync(8);

                    await itemTask1;
                    await itemTask2;
                    await itemTask3;

                    Assert.AreEqual(1, dataListSource.FetchCountCallCount);
                    Assert.AreEqual(1, dataListSource.FetchPageSizeCallCount);
                    Assert.AreEqual(1, dataListSource.FetchPageCallCount);
                });
        }

        [TestMethod]
        public void GetItem_FullPage_OnlyFetchesItemsOnceWithSubsequentRequests_MiddlePage()
        {
            SynchronizationHelper.Run(async () =>
                {
                    MockDataListSource dataListSource = new MockDataListSource();

                    Task<int> itemTask1 = dataListSource.GetItemAsync(8);
                    await itemTask1;
                    Task<int> itemTask2 = dataListSource.GetItemAsync(8);
                    await itemTask2;
                    Task<int> itemTask3 = dataListSource.GetItemAsync(8);
                    await itemTask3;

                    Task.WaitAll(itemTask1, itemTask2, itemTask3);

                    Assert.AreEqual(1, dataListSource.FetchCountCallCount);
                    Assert.AreEqual(1, dataListSource.FetchPageSizeCallCount);
                    Assert.AreEqual(1, dataListSource.FetchPageCallCount);
                }
            );
        }

        [TestMethod]
        public async Task GetItem_Exception_IndexIsGreaterThanListLength()
        {
            MockDataListSource dataListSource = new MockDataListSource();

            await AssertEx.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => dataListSource.GetItemAsync(14));
        }

        [TestMethod]
        public async Task GetItem_Exception_IndexIsNegative()
        {
            MockDataListSource dataListSource = new MockDataListSource();

            await AssertEx.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => dataListSource.GetItemAsync(-1));
        }

        // *** Private Sub-Classes ***

        private class MockDataListSource : PagedDataListSource<int>
        {
            // *** Fields ***

            public int FetchCountCallCount = 0;
            public int FetchPageSizeCallCount = 0;
            public int FetchPageCallCount = 0;

            // *** Overriden Base Methods ***

            protected async override Task<DataListPageResult<int>> FetchCountAsync()
            {
                FetchCountCallCount++;
                await Task.Yield();

                return new DataListPageResult<int>(14, null, null, null);
            }

            protected async override Task<DataListPageResult<int>> FetchPageSizeAsync()
            {
                FetchPageSizeCallCount++;
                await Task.Yield();

                return new DataListPageResult<int>(null, 5, null, null);
            }

            protected async override Task<DataListPageResult<int>> FetchPageAsync(int pageNumber)
            {
                FetchPageCallCount++;
                await Task.Yield();

                switch (pageNumber)
                {
                    case 1:
                        return new DataListPageResult<int>(null, null, 1, new int[] { 2, 4, 6, 8, 10 });
                    case 2:
                        return new DataListPageResult<int>(null, null, 2, new int[] { 12, 14, 16, 18, 20 });
                    case 3:
                        return new DataListPageResult<int>(null, null, 3, new int[] { 22, 24, 26, 28 });
                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        private class MockFullPageDataListSource : PagedDataListSource<int>
        {
            // *** Fields ***

            public int FetchCountCallCount = 0;
            public int FetchPageSizeCallCount = 0;
            public int FetchPageCallCount = 0;

            // *** Overriden Base Methods ***

            protected async override Task<DataListPageResult<int>> FetchCountAsync()
            {
                FetchCountCallCount++;
                await Task.Yield();

                // Return page one results rather than count
                return new DataListPageResult<int>(14, 5, 1, new int[] { 2, 4, 6, 8, 10 });
            }

            protected async override Task<DataListPageResult<int>> FetchPageSizeAsync()
            {
                FetchPageSizeCallCount++;
                await Task.Yield();

                // Return page one results rather than page size
                return new DataListPageResult<int>(14, 5, 1, new int[] { 2, 4, 6, 8, 10 });
            }

            protected async override Task<DataListPageResult<int>> FetchPageAsync(int pageNumber)
            {
                FetchPageCallCount++;
                await Task.Yield();

                switch (pageNumber)
                {
                    case 1:
                        return new DataListPageResult<int>(14, 5, 1, new int[] { 2, 4, 6, 8, 10 });
                    case 2:
                        return new DataListPageResult<int>(14, 5, 2, new int[] { 12, 14, 16, 18, 20 });
                    case 3:
                        return new DataListPageResult<int>(14, 5, 3, new int[] { 22, 24, 26, 28 });
                    default:
                        throw new InvalidOperationException();
                }
            }
        }
    }
}
