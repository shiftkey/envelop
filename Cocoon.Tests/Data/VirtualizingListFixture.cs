using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cocoon.Data;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Concoon.Tests.Data
{
    [TestClass]
    public class VirtualizingListFixture
    {
        // *** Constructor Tests ***

        [TestMethod]
        public void Constructor_Void_CollectionIsEmpty()
        {
            VirtualizingList<int> list = new VirtualizingList<int>();

            Assert.AreEqual(0, list.Count);
        }

        // *** Property Tests ***

        [TestMethod]
        public void Indexer_SettingAndGettingReturnsSameElement()
        {
            VirtualizingList<int> list = new VirtualizingList<int>();
            list.UpdateCount(20);
            list[10] = 42;

            Assert.AreEqual(42, list[10]);
        }

        [TestMethod]
        public void Indexer_Setter_Exception_IndexIsLessThanZero()
        {
            VirtualizingList<int> list = new VirtualizingList<int>();
            list.UpdateCount(20);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => list[-1] = 100);
        }

        [TestMethod]
        public void Indexer_Setter_Exception_IndexIsEqualToCount()
        {
            VirtualizingList<int> list = new VirtualizingList<int>();
            list.UpdateCount(20);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => list[20] = 100);
        }

        [TestMethod]
        public void Indexer_Getter_ReturnsPlaceholderBeforeSetElement()
        {
            VirtualizingList<int> list = new VirtualizingList<int>();
            list.UpdateCount(20);
            list[10] = 42;

            Assert.AreEqual(0, list[9]);
        }

        [TestMethod]
        public void Indexer_Getter_ReturnsPlaceholderAfterSetElement()
        {
            VirtualizingList<int> list = new VirtualizingList<int>();
            list.UpdateCount(20);
            list[10] = 42;

            Assert.AreEqual(0, list[11]);
        }

        [TestMethod]
        public void Indexer_Getter_ReturnsPlaceholderFarAfterSetElement()
        {
            VirtualizingList<int> list = new VirtualizingList<int>();
            list.UpdateCount(2000);
            list[10] = 42;

            Assert.AreEqual(0, list[1990]);
        }

        [TestMethod]
        public void Indexer_Getter_Exception_IndexIsLessThanZero()
        {
            VirtualizingList<int> list = new VirtualizingList<int>();
            list.UpdateCount(20);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => list[-1]);
        }

        [TestMethod]
        public void Indexer_Getter_Exception_IndexIsEqualToCount()
        {
            VirtualizingList<int> list = new VirtualizingList<int>();
            list.UpdateCount(20);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => list[20]);
        }

        [TestMethod]
        public void IsReadOnly_IsFalse()
        {
            VirtualizingList<int> list = new VirtualizingList<int>();

            Assert.AreEqual(false, list.IsReadOnly);
        }

        // *** Method Tests ***

        [TestMethod]
        public void Add_AddsNewItemToList()
        {
            VirtualizingList<int> list = new VirtualizingList<int>();
            list.UpdateCount(20);

            list.Add(123);

            Assert.AreEqual(21, list.Count);
            Assert.AreEqual(123, list[20]);
        }

        [TestMethod]
        public void Clear_ClearsTheList()
        {
            VirtualizingList<int> list = new VirtualizingList<int>();
            list.UpdateCount(20);

            list.Clear();

            Assert.AreEqual(0, list.Count);
        }

        [TestMethod]
        public void Contains_ReturnsTrueIfItemInList()
        {
            VirtualizingList<int> list = new VirtualizingList<int>();
            list.UpdateCount(20);
            list[2] = 8;
            list[10] = 42;

            bool result = list.Contains(42);

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void Contains_ReturnsFalseIfNotInList()
        {
            VirtualizingList<int> list = new VirtualizingList<int>();
            list.UpdateCount(20);
            list[2] = 8;
            list[10] = 42;

            bool result = list.Contains(15);

            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void CopyTo_CopiesToDestinationArray()
        {
            VirtualizingList<int> list = new VirtualizingList<int>();
            list.UpdateCount(20);
            list[2] = 8;
            list[10] = 42;

            int[] destination = new int[25];
            destination[1] = 1;
            destination[5] = 2;
            destination[22] = 3;

            list.CopyTo(destination, 2);

            CollectionAssert.AreEqual(new int[] { 0, 1, 0, 0, 8, 0, 0, 0, 0, 0, 0, 0, 42, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0 }, destination);
        }

        [TestMethod]
        public void IndexOf_ReturnsIndexOfItemInList()
        {
            VirtualizingList<int> list = new VirtualizingList<int>();
            list.UpdateCount(20);
            list[2] = 8;
            list[10] = 42;

            int result = list.IndexOf(42);

            Assert.AreEqual(10, result);
        }

        [TestMethod]
        public void IndexOf_ReturnsMinusOneIfNotInList()
        {
            VirtualizingList<int> list = new VirtualizingList<int>();
            list.UpdateCount(20);
            list[2] = 8;
            list[10] = 42;

            int result = list.IndexOf(15);

            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void Insert_InsertsItemIntoList()
        {
            VirtualizingList<int> list = new VirtualizingList<int>();
            list.UpdateCount(20);
            list[2] = 8;
            list[10] = 42;

            list.Insert(5, 123);

            Assert.AreEqual(21, list.Count);
            Assert.AreEqual(8, list[2]);
            Assert.AreEqual(123, list[5]);
            Assert.AreEqual(42, list[11]);
        }

        [TestMethod]
        public void Insert_Exception_IndexIsLessThanZero()
        {
            VirtualizingList<int> list = new VirtualizingList<int>();
            list.UpdateCount(20);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => list.Insert(-1, 123));
        }

        [TestMethod]
        public void Insert_Exception_IndexIsGreaterThanCount()
        {
            VirtualizingList<int> list = new VirtualizingList<int>();
            list.UpdateCount(20);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => list.Insert(21, 123));
        }

        [TestMethod]
        public void Remove_RemovesItemFromList()
        {
            VirtualizingList<int> list = new VirtualizingList<int>();
            list.UpdateCount(20);
            list[2] = 8;
            list[5] = 10;
            list[10] = 42;

            list.Remove(10);

            Assert.AreEqual(19, list.Count);
            Assert.AreEqual(8, list[2]);
            Assert.AreEqual(42, list[9]);
        }

        [TestMethod]
        public void Remove_ReturnsTrueForItemInList()
        {
            VirtualizingList<int> list = new VirtualizingList<int>();
            list.UpdateCount(20);
            list[2] = 8;
            list[5] = 10;
            list[10] = 42;

            bool result = list.Remove(10);

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void Remove_ReturnsFalseForItemNotInList()
        {
            VirtualizingList<int> list = new VirtualizingList<int>();
            list.UpdateCount(20);
            list[2] = 8;
            list[5] = 10;
            list[10] = 42;

            bool result = list.Remove(12);

            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void RemoveAt_RemovesItemFromList()
        {
            VirtualizingList<int> list = new VirtualizingList<int>();
            list.UpdateCount(20);
            list[2] = 8;
            list[5] = 10;
            list[10] = 42;

            list.RemoveAt(5);

            Assert.AreEqual(19, list.Count);
            Assert.AreEqual(8, list[2]);
            Assert.AreEqual(42, list[9]);
        }

        [TestMethod]
        public void RemoveAt_Exception_IndexIsLessThanZero()
        {
            VirtualizingList<int> list = new VirtualizingList<int>();
            list.UpdateCount(20);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => list.RemoveAt(-1));
        }

        [TestMethod]
        public void RemoveAt_Exception_IndexIsEqualToCount()
        {
            VirtualizingList<int> list = new VirtualizingList<int>();
            list.UpdateCount(20);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => list.RemoveAt(20));
        }

        [TestMethod]
        public void UpdateCount_UpdatesCountProperty()
        {
            VirtualizingList<int> list = new VirtualizingList<int>();
            list.UpdateCount(20);

            Assert.AreEqual(20, list.Count);
        }

        [TestMethod]
        public void UpdateCount_Exception_CountIsLessThanZero()
        {
            VirtualizingList<int> list = new VirtualizingList<int>();
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => list.UpdateCount(-1));
        }

        [TestMethod]
        public void GetEnumerator_Generic_EnumeratesArray()
        {
            VirtualizingList<int> list = new VirtualizingList<int>();
            list.UpdateCount(20);
            list[2] = 8;
            list[10] = 42;

            List<int> result = new List<int>();

            IEnumerator<int> enumerator = ((IEnumerable<int>)list).GetEnumerator();

            while (enumerator.MoveNext())
                result.Add(enumerator.Current);

            CollectionAssert.AreEqual(new int[] { 0, 0, 8, 0, 0, 0, 0, 0, 0, 0, 42, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, result);
        }

        [TestMethod]
        public void GetEnumerator_NonGeneric_EnumeratesArray()
        {
            VirtualizingList<int> list = new VirtualizingList<int>();
            list.UpdateCount(20);
            list[2] = 8;
            list[10] = 42;

            List<int> result = new List<int>();

            IEnumerator enumerator = ((IEnumerable)list).GetEnumerator();

            while (enumerator.MoveNext())
                result.Add((int)enumerator.Current);

            CollectionAssert.AreEqual(new int[] { 0, 0, 8, 0, 0, 0, 0, 0, 0, 0, 42, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, result);
        }
    }
}
