using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cocoon.Data;
using Cocoon.Tests.Helpers;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Concoon.Tests.Data
{
    [TestClass]
    public class VirtualizingDataListFixture
    {
        // *** Constructor Tests ***

        [TestMethod]
        public void Constructor_Exception_DataListSourceIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new VirtualizingDataList<int>(null));
        }

        // *** Property Tests ***

        [TestMethod]
        public void Indexer_IsPlaceholderWhilstAwaitingValue()
        {
            var dataListSource = new MockDataListSource();
            var dataList = new VirtualizingDataList<int>(dataListSource);

            SynchronizationHelper.Run(() =>
                {
                    int count = dataList.Count;
                    dataListSource.TriggerCount();
                });

            Assert.AreEqual(0, dataList[4]);
        }

        [TestMethod]
        public void Indexer_IsValueOnceProvided()
        {
            var dataListSource = new MockDataListSource();
            var dataList = new VirtualizingDataList<int>(dataListSource);

            SynchronizationHelper.Run(() =>
                {
                    int count = dataList.Count;
                    dataListSource.TriggerCount();

                    object item = dataList[4];
                    dataListSource.TriggerItems();
                });


            Assert.AreEqual(10, dataList[4]);
        }

        [TestMethod]
        public void Count_IsZeroWhilstLoading()
        {
            var dataListSource = new MockDataListSource();
            var dataList = new VirtualizingDataList<int>(dataListSource);

            Assert.AreEqual(0, dataList.Count);
        }

        [TestMethod]
        public void Count_IsListLengthAfterLoading()
        {
            var dataListSource = new MockDataListSource();
            var dataList = new VirtualizingDataList<int>(dataListSource);

            SynchronizationHelper.Run(() =>
                {
                    int count = dataList.Count;
                    dataListSource.TriggerCount();
                });

            Assert.AreEqual(10, dataList.Count);
        }

        [TestMethod]
        public void IsLoading_IsInitiallyFalse()
        {
            var dataListSource = new MockDataListSource();
            var dataList = new VirtualizingDataList<int>(dataListSource);

            Assert.AreEqual(false, dataList.IsLoading);
        }

        [TestMethod]
        public void IsLoading_TrueAfterFirstCallToCountGetter()
        {
            var dataListSource = new MockDataListSource();
            var dataList = new VirtualizingDataList<int>(dataListSource);

            SynchronizationHelper.Run(() =>
                {
                    int count = dataList.Count;
                });

            Assert.AreEqual(true, dataList.IsLoading);
        }

        [TestMethod]
        public void IsLoading_RaisesPropertyChangedToTrue()
        {
            var dataListSource = new MockDataListSource();
            var dataList = new VirtualizingDataList<int>(dataListSource);

            int isLoadingChangedCount = 0;

            dataList.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
                {
                    if (e.PropertyName == "IsLoading")
                        isLoadingChangedCount++;
                };

            SynchronizationHelper.Run(() =>
                {
                    int count = dataList.Count;
                });

            Assert.AreEqual(1, isLoadingChangedCount);
        }

        [TestMethod]
        public void IsLoading_FalseAfterCountIsReturned()
        {
            var dataListSource = new MockDataListSource();
            var dataList = new VirtualizingDataList<int>(dataListSource);

            SynchronizationHelper.Run(() =>
                {
                    int count = dataList.Count;
                    dataListSource.TriggerCount();
                });

            Assert.AreEqual(false, dataList.IsLoading);
        }

        [TestMethod]
        public void IsLoading_RaisesPropertyChangedToFalse()
        {
            var dataListSource = new MockDataListSource();
            var dataList = new VirtualizingDataList<int>(dataListSource);
            int isLoadingChangedCount = 0;

            SynchronizationHelper.Run(() =>
                {
                    int count = dataList.Count;
                    
                    dataList.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
                    {
                        if (e.PropertyName == "IsLoading")
                            isLoadingChangedCount++;
                    };

                    dataListSource.TriggerCount();
                });

            Assert.AreEqual(1, isLoadingChangedCount);
        }

        // *** Private Sub-Classes ***

        private class MockDataListSource : IDataListSource<int>
        {
            // *** Fields ***

            private TaskCompletionSource<bool> countCompletionSource = new TaskCompletionSource<bool>();
            private TaskCompletionSource<bool> itemsCompletionSource = new TaskCompletionSource<bool>();

            // *** Constructors ***

            public MockDataListSource()
            {
            }

            // *** Methods ***

            public void TriggerCount()
            {
                countCompletionSource.SetResult(true);
            }

            public void TriggerItems()
            {
                itemsCompletionSource.SetResult(true);
            }

            // *** IDataListSource<int> Methods ***

            public async Task<int> GetCountAsync()
            {
                await countCompletionSource.Task;
                return 10;
            }

            public async Task<int> GetItemAsync(int index)
            {
                await itemsCompletionSource.Task;
                return (index + 1) * 2;
            }
        }
    }
}
