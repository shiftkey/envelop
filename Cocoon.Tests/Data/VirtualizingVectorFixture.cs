using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cocoon.Data;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Windows.Foundation.Collections;

namespace Concoon.Tests.Data
{
    [TestClass]
    public class VirtualizingVectorFixture
    {
        // *** Constructor Tests ***

        [TestMethod]
        public void Constructor_Void_CollectionIsEmpty()
        {
            MockEmptyVirtualizingVector vector = new MockEmptyVirtualizingVector();

            Assert.AreEqual(0, vector.Count);
        }

        // *** IList<T> Tests ***

        [TestMethod]
        public void IListT_IsReadOnly_Getter_ReturnsTrue()
        {
            MockVirtualizingVector vector = new MockVirtualizingVector();

            Assert.AreEqual(true, ((IList<int>)vector).IsReadOnly);
        }

        [TestMethod]
        public void IListT_Indexer_Getter_ReturnsFetchedElement()
        {
            MockVirtualizingVector vector = new MockVirtualizingVector();
            vector.CallUpdateCount();

            Assert.AreEqual(15, ((IList<int>)vector)[5]);
        }

        [TestMethod]
        public void IListT_Indexer_Getter_ReturnsFetchedElement_WithNullElements()
        {
            MockNullVirtualizingVector vector = new MockNullVirtualizingVector();
            vector.CallUpdateCount();

            Assert.AreEqual(null, ((IList<object>)vector)[5]);
        }

        [TestMethod]
        public void IListT_Indexer_Getter_ReturnsPlaceholderBeforeFetchedElement()
        {
            MockVirtualizingVector vector = new MockVirtualizingVector();
            vector.CallUpdateCount();

            Assert.AreEqual(0, ((IList<int>)vector)[4]);
        }

        [TestMethod]
        public void IListT_Indexer_Getter_ReturnsPlaceholderBeforeFetchedElement_WithNullElements()
        {
            MockNullVirtualizingVector vector = new MockNullVirtualizingVector();
            vector.CallUpdateCount();

            Assert.AreEqual(null, ((IList<object>)vector)[4]);
        }

        [TestMethod]
        public void IListT_Indexer_Getter_ReturnsPlaceholderAfterFetchedElement()
        {
            MockVirtualizingVector vector = new MockVirtualizingVector();
            vector.CallUpdateCount();

            Assert.AreEqual(0, ((IList<int>)vector)[6]);
        }

        [TestMethod]
        public void IListT_Indexer_Setter_ThrowsException_CollectionIsReadOnly()
        {
            MockVirtualizingVector vector = new MockVirtualizingVector();

            Assert.ThrowsException<InvalidOperationException>(() => ((IList<int>)vector)[6] = 10);
        }

        [TestMethod]
        public void IListT_Add_ThrowsException_CollectionIsReadOnly()
        {
            MockVirtualizingVector vector = new MockVirtualizingVector();

            Assert.ThrowsException<InvalidOperationException>(() => ((IList<int>)vector).Add(42));
        }

        [TestMethod]
        public void IListT_Clear_ThrowsException_CollectionIsReadOnly()
        {
            MockVirtualizingVector vector = new MockVirtualizingVector();

            Assert.ThrowsException<InvalidOperationException>(() => ((IList<int>)vector).Clear());
        }

        [TestMethod]
        public void IListT_Contains_ReturnsTrueIfInInnerCollection()
        {
            MockVirtualizingVector vector = new MockVirtualizingVector();
            vector.CallUpdateCount();
            vector.CallUpdateItem();

            Assert.AreEqual(true, ((IList<int>)vector).Contains(15));
        }

        [TestMethod]
        public void IListT_Contains_ReturnsFalseIfNotInInnerCollection()
        {
            MockVirtualizingVector vector = new MockVirtualizingVector();
            vector.CallUpdateCount();
            vector.CallUpdateItem();

            Assert.AreEqual(false, ((IList<int>)vector).Contains(16));
        }

        [TestMethod]
        public void IListT_CopyTo_CopiesToDestinationArray()
        {
            MockVirtualizingVector vector = new MockVirtualizingVector();
            vector.CallUpdateCount();

            int[] values = new int[42];
            ((IList<int>)vector).CopyTo(values, 0);

            CollectionAssert.AreEqual(new int[] { 0, 0, 0, 0, 0, 15, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, values);
        }

        [TestMethod]
        public void IListT_Count_InitiallyReturnsZero()
        {
            MockEmptyVirtualizingVector vector = new MockEmptyVirtualizingVector();

            Assert.AreEqual(0, ((IList<int>)vector).Count);
        }

        [TestMethod]
        public void IListT_Count_CallsFetchCount()
        {
            MockVirtualizingVector vector = new MockVirtualizingVector();

            Assert.AreEqual(42, ((IList<int>)vector).Count);
        }

        [TestMethod]
        public void IListT_Count_OnlyCallsFetchCountOnce()
        {
            MockVirtualizingVector vector = new MockVirtualizingVector();

            int dummy1 = ((IList<int>)vector).Count;
            int dummy2 = ((IList<int>)vector).Count;
            int dummy3 = ((IList<int>)vector).Count;
            int dummy4 = ((IList<int>)vector).Count;

            Assert.AreEqual(1, vector.FetchCountCallCount);
        }

        [TestMethod]
        public void IListT_GetEnumerator_EnumeratesCurrentStateOfCollection()
        {
            MockVirtualizingVector vector = new MockVirtualizingVector();
            vector.CallUpdateCount();

            IEnumerable<int> enumerable = (IEnumerable<int>)vector;
            List<int> values = new List<int>();

            foreach (int value in enumerable)
                values.Add(value);

            CollectionAssert.AreEqual(new int[] { 0, 0, 0, 0, 0, 15, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, values);
        }

        [TestMethod]
        public void IListT_IndexOf_ReturnsItemIndexIfInInnerCollection()
        {
            MockVirtualizingVector vector = new MockVirtualizingVector();
            vector.CallUpdateCount();
            vector.CallUpdateItem();

            Assert.AreEqual(5, ((IList<int>)vector).IndexOf(15));
        }

        [TestMethod]
        public void IListT_Insert_ThrowsException_CollectionIsReadOnly()
        {
            MockVirtualizingVector vector = new MockVirtualizingVector();

            Assert.ThrowsException<InvalidOperationException>(() => ((IList<int>)vector).Insert(42, 2));
        }

        [TestMethod]
        public void IListT_Remove_ThrowsException_CollectionIsReadOnly()
        {
            MockVirtualizingVector vector = new MockVirtualizingVector();

            Assert.ThrowsException<InvalidOperationException>(() => ((IList<int>)vector).Remove(42));
        }

        [TestMethod]
        public void IListT_RemoveAt_ThrowsException_CollectionIsReadOnly()
        {
            MockVirtualizingVector vector = new MockVirtualizingVector();

            Assert.ThrowsException<InvalidOperationException>(() => ((IList<int>)vector).RemoveAt(2));
        }

        // *** IList Tests ***

        [TestMethod]
        public void IList_IsFixedSize_Getter_ReturnsFalse()
        {
            MockVirtualizingVector vector = new MockVirtualizingVector();

            Assert.AreEqual(false, ((IList)vector).IsFixedSize);
        }

        [TestMethod]
        public void IList_IsSynchronized_Getter_ReturnsFalse()
        {
            MockVirtualizingVector vector = new MockVirtualizingVector();

            Assert.AreEqual(false, ((IList)vector).IsSynchronized);
        }

        [TestMethod]
        public void IList_SyncRoot_Getter_ReturnsItself()
        {
            MockVirtualizingVector vector = new MockVirtualizingVector();

            Assert.AreEqual(vector, ((IList)vector).SyncRoot);
        }

        [TestMethod]
        public void IList_IsReadOnly_Getter_ReturnsTrue()
        {
            MockVirtualizingVector vector = new MockVirtualizingVector();

            Assert.AreEqual(true, ((IList)vector).IsReadOnly);
        }

        [TestMethod]
        public void IList_Indexer_Getter_ReturnsFetchedElement()
        {
            MockVirtualizingVector vector = new MockVirtualizingVector();
            vector.CallUpdateCount();

            Assert.AreEqual(15, ((IList)vector)[5]);
        }

        [TestMethod]
        public void IList_Indexer_Getter_ReturnsPlaceholderBeforeFetchedElement()
        {
            MockVirtualizingVector vector = new MockVirtualizingVector();
            vector.CallUpdateCount();

            Assert.AreEqual(0, ((IList)vector)[4]);
        }

        [TestMethod]
        public void IList_Indexer_Getter_ReturnsPlaceholderAfterFetchedElement()
        {
            MockVirtualizingVector vector = new MockVirtualizingVector();
            vector.CallUpdateCount();

            Assert.AreEqual(0, ((IList)vector)[6]);
        }

        [TestMethod]
        public void IList_Indexer_Setter_ThrowsException_CollectionIsReadOnly()
        {
            MockVirtualizingVector vector = new MockVirtualizingVector();

            Assert.ThrowsException<InvalidOperationException>(() => ((IList)vector)[6] = 10);
        }

        [TestMethod]
        public void IList_Add_ThrowsException_CollectionIsReadOnly()
        {
            MockVirtualizingVector vector = new MockVirtualizingVector();

            Assert.ThrowsException<InvalidOperationException>(() => ((IList)vector).Add(42));
        }

        [TestMethod]
        public void IList_Clear_ThrowsException_CollectionIsReadOnly()
        {
            MockVirtualizingVector vector = new MockVirtualizingVector();

            Assert.ThrowsException<InvalidOperationException>(() => ((IList)vector).Clear());
        }

        [TestMethod]
        public void IList_Contains_ReturnsTrueIfInInnerCollection()
        {
            MockVirtualizingVector vector = new MockVirtualizingVector();
            vector.CallUpdateCount();
            vector.CallUpdateItem();

            Assert.AreEqual(true, ((IList)vector).Contains(15));
        }

        [TestMethod]
        public void IList_Contains_ReturnsFalseIfNotInInnerCollection()
        {
            MockVirtualizingVector vector = new MockVirtualizingVector();
            vector.CallUpdateCount();
            vector.CallUpdateItem();

            Assert.AreEqual(false, ((IList)vector).Contains(16));
        }

        [TestMethod]
        public void IList_CopyTo_CopiesToDestinationArray()
        {
            MockVirtualizingVector vector = new MockVirtualizingVector();
            vector.CallUpdateCount();

            int[] values = new int[42];
            ((IList)vector).CopyTo(values, 0);

            CollectionAssert.AreEqual(new int[] { 0, 0, 0, 0, 0, 15, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, values);
        }

        [TestMethod]
        public void IList_Count_CallsFetchCount()
        {
            MockVirtualizingVector vector = new MockVirtualizingVector();

            Assert.AreEqual(42, ((IList)vector).Count);
        }

        [TestMethod]
        public void IList_Count_OnlyCallsFetchCountOnce()
        {
            MockVirtualizingVector vector = new MockVirtualizingVector();

            int dummy1 = ((IList)vector).Count;
            int dummy2 = ((IList)vector).Count;
            int dummy3 = ((IList)vector).Count;
            int dummy4 = ((IList)vector).Count;

            Assert.AreEqual(1, vector.FetchCountCallCount);
        }

        [TestMethod]
        public void IList_GetEnumerator_EnumeratesCurrentStateOfCollection()
        {
            MockVirtualizingVector vector = new MockVirtualizingVector();
            vector.CallUpdateCount();

            IEnumerable enumerable = (IEnumerable)vector;
            List<int> values = new List<int>();

            foreach (int value in enumerable)
                values.Add(value);

            CollectionAssert.AreEqual(new int[] { 0, 0, 0, 0, 0, 15, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, values);
        }

        [TestMethod]
        public void IList_IndexOf_ReturnsItemIndexIfInInnerCollection()
        {
            MockVirtualizingVector vector = new MockVirtualizingVector();
            vector.CallUpdateCount();
            vector.CallUpdateItem();

            Assert.AreEqual(5, ((IList)vector).IndexOf(15));
        }

        [TestMethod]
        public void IList_Insert_ThrowsException_CollectionIsReadOnly()
        {
            MockVirtualizingVector vector = new MockVirtualizingVector();

            Assert.ThrowsException<InvalidOperationException>(() => ((IList)vector).Insert(42, 2));
        }

        [TestMethod]
        public void IList_Remove_ThrowsException_CollectionIsReadOnly()
        {
            MockVirtualizingVector vector = new MockVirtualizingVector();

            Assert.ThrowsException<InvalidOperationException>(() => ((IList)vector).Remove(42));
        }

        [TestMethod]
        public void IList_RemoveAt_ThrowsException_CollectionIsReadOnly()
        {
            MockVirtualizingVector vector = new MockVirtualizingVector();

            Assert.ThrowsException<InvalidOperationException>(() => ((IList)vector).RemoveAt(2));
        }

        // *** Behavior Tests ***

        [TestMethod]
        public void UpdateCountFiresPropertyChanged()
        {
            MockVirtualizingVector vector = new MockVirtualizingVector();

            bool propertyChanged = false;
            vector.PropertyChanged += (sender, e) => { if (e.PropertyName == "Count") { propertyChanged = true; } };

            vector.CallUpdateCount();

            Assert.AreEqual(true, propertyChanged);
        }

        [TestMethod]
        public void UpdateCountFiresCollectionChanged()
        {
            MockVirtualizingVector vector = new MockVirtualizingVector();

            bool collectionChanged = false;
            vector.CollectionChanged += (sender, e) => { if (e.Action == NotifyCollectionChangedAction.Reset) { collectionChanged = true; } };

            vector.CallUpdateCount();

            Assert.AreEqual(true, collectionChanged);
        }

        [TestMethod]
        public void UpdateCountDoesNotFireEventsIfCountHasNotChanged()
        {
            MockVirtualizingVector vector = new MockVirtualizingVector();

            vector.CallUpdateCount();

            bool propertyChanged = false;
            vector.PropertyChanged += (sender, e) => { if (e.PropertyName == "Count") { propertyChanged = true; } };

            bool collectionChanged = false;
            vector.CollectionChanged += (sender, e) => { if (e.Action == NotifyCollectionChangedAction.Reset) { collectionChanged = true; } };


            vector.CallUpdateCount();

            Assert.AreEqual(false, propertyChanged);
            Assert.AreEqual(false, collectionChanged);
        }

        // *** Private Sub-Classes ***

        private class MockNullVirtualizingVector : VirtualizingVector<object>
        {
            // *** Methods ***

            public void CallUpdateCount()
            {
                UpdateCount(42);
            }

            public void CallUpdateItem()
            {
                UpdateItem(5, null);
            }

            // *** Overriden Methods ***

            protected override void FetchCount()
            {
                UpdateCount(42);
            }

            protected override void FetchItem(int index)
            {
                if (index == 5)
                    UpdateItem(5, null);
            }
        }

        private class MockEmptyVirtualizingVector : VirtualizingVector<int>
        {
            // *** Overriden Methods ***

            protected override void FetchCount()
            {
            }

            protected override void FetchItem(int index)
            {
            }
        }

        private class MockVirtualizingVector : VirtualizingVector<int>
        {
            // *** Properties ***

            public int FetchCountCallCount { get; private set; }

            // *** Methods ***

            public void CallUpdateCount()
            {
                UpdateCount(42);
            }

            public void CallUpdateItem()
            {
                UpdateItem(5, 15);
            }

            // *** Overriden Methods ***

            protected override void FetchCount()
            {
                FetchCountCallCount++;

                UpdateCount(42);
            }

            protected override void FetchItem(int index)
            {
                if (index == 5)
                    UpdateItem(5, 15);
            }
        }
    }
}
