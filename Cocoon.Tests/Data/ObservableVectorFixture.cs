using System;
using System.Collections.Generic;
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
    public class ObservableVectorFixture
    {
        // *** Constructor Tests ***

        [TestMethod]
        public void Constructor_Void_CollectionIsEmpty()
        {
            ObservableVector<int> vector = new ObservableVector<int>();

            Assert.AreEqual(0, vector.Count);
        }

        [TestMethod]
        public void Constructor_WithList_CollectionIsSameAsThatSupplied()
        {
            List<int> collection = new List<int>(new int[] { 1, 3, 7, 10, 42 });
            ObservableVector<int> vector = new ObservableVector<int>(collection);

            Assert.AreEqual(5, vector.Count);
            CollectionAssert.AreEqual(collection, vector.ToArray());
        }

        // *** Method Tests ***

        [TestMethod]
        public void Clear_RemovesAllItems()
        {
            List<int> collection = new List<int>(new int[] { 1, 3, 7, 10, 42 });
            ObservableVector<int> vector = new ObservableVector<int>(collection);

            vector.Clear();

            Assert.AreEqual(0, vector.Count);
        }

        [TestMethod]
        public void Clear_PropertyChanged_Count()
        {
            List<int> collection = new List<int>(new int[] { 1, 3, 7, 10, 42 });
            ObservableVector<int> vector = new ObservableVector<int>(collection);

            bool propertyChanged = false;
            vector.PropertyChanged += (sender, e) => { if (e.PropertyName == "Count") { propertyChanged = true; } };

            vector.Clear();

            Assert.AreEqual(true, propertyChanged);
        }

        [TestMethod]
        public void Clear_PropertyChanged_Indexer()
        {
            List<int> collection = new List<int>(new int[] { 1, 3, 7, 10, 42 });
            ObservableVector<int> vector = new ObservableVector<int>(collection);

            bool propertyChanged = false;
            vector.PropertyChanged += (sender, e) => { if (e.PropertyName == "Item[]") { propertyChanged = true; } };

            vector.Clear();

            Assert.AreEqual(true, propertyChanged);
        }

        //[TestMethod]
        //public void Clear_VectorChanged()
        //{
        //    List<int> collection = new List<int>(new int[] { 1, 3, 7, 10, 42 });
        //    ObservableVector<int> vector = new ObservableVector<int>(collection);

        //    bool vectorChanged = false;
        //    vector.VectorChanged += (sender, e) => { if (e.CollectionChange == CollectionChange.Reset) { vectorChanged = true; } };

        //    vector.Clear();

        //    Assert.AreEqual(true, vectorChanged);
        //}

        [TestMethod]
        public void Clear_CollectionChanged()
        {
            List<int> collection = new List<int>(new int[] { 1, 3, 7, 10, 42 });
            ObservableVector<int> vector = new ObservableVector<int>(collection);

            bool vectorChanged = false;
            vector.CollectionChanged += (sender, e) => { if (e.Action == NotifyCollectionChangedAction.Reset) { vectorChanged = true; } };

            vector.Clear();

            Assert.AreEqual(true, vectorChanged);
        }

        [TestMethod]
        public void Add_AddsItem()
        {
            List<int> collection = new List<int>(new int[] { 1, 3, 7, 10, 42 });
            ObservableVector<int> vector = new ObservableVector<int>(collection);

            vector.Add(100);

            CollectionAssert.AreEqual(new int[] { 1, 3, 7, 10, 42, 100 }, vector.ToArray());
        }

        [TestMethod]
        public void Add_PropertyChanged_Count()
        {
            List<int> collection = new List<int>(new int[] { 1, 3, 7, 10, 42 });
            ObservableVector<int> vector = new ObservableVector<int>(collection);

            bool propertyChanged = false;
            vector.PropertyChanged += (sender, e) => { if (e.PropertyName == "Count") { propertyChanged = true; } };

            vector.Add(100);

            Assert.AreEqual(true, propertyChanged);
        }

        [TestMethod]
        public void Add_PropertyChanged_Indexer()
        {
            List<int> collection = new List<int>(new int[] { 1, 3, 7, 10, 42 });
            ObservableVector<int> vector = new ObservableVector<int>(collection);

            bool propertyChanged = false;
            vector.PropertyChanged += (sender, e) => { if (e.PropertyName == "Item[]") { propertyChanged = true; } };

            vector.Add(100);

            Assert.AreEqual(true, propertyChanged);
        }

        //[TestMethod]
        //public void Add_VectorChanged()
        //{
        //    List<int> collection = new List<int>(new int[] { 1, 3, 7, 10, 42 });
        //    ObservableVector<int> vector = new ObservableVector<int>(collection);

        //    bool vectorChanged = false;
        //    vector.VectorChanged += (sender, e) => { if (e.CollectionChange == CollectionChange.ItemInserted && e.Index == 5) { vectorChanged = true; } };

        //    vector.Add(100);

        //    Assert.AreEqual(true, vectorChanged);
        //}

        [TestMethod]
        public void Add_CollectionChanged()
        {
            List<int> collection = new List<int>(new int[] { 1, 3, 7, 10, 42 });
            ObservableVector<int> vector = new ObservableVector<int>(collection);

            bool vectorChanged = false;
            vector.CollectionChanged += (sender, e) => { if (e.Action == NotifyCollectionChangedAction.Add && e.NewStartingIndex == 5 && (int)e.NewItems[0] == 100) { vectorChanged = true; } };

            vector.Add(100);

            Assert.AreEqual(true, vectorChanged);
        }

        [TestMethod]
        public void Insert_AddsItem()
        {
            List<int> collection = new List<int>(new int[] { 1, 3, 7, 10, 42 });
            ObservableVector<int> vector = new ObservableVector<int>(collection);

            vector.Insert(2, 100);

            CollectionAssert.AreEqual(new int[] { 1, 3, 100, 7, 10, 42 }, vector.ToArray());
        }

        [TestMethod]
        public void Insert_PropertyChanged_Count()
        {
            List<int> collection = new List<int>(new int[] { 1, 3, 7, 10, 42 });
            ObservableVector<int> vector = new ObservableVector<int>(collection);

            bool propertyChanged = false;
            vector.PropertyChanged += (sender, e) => { if (e.PropertyName == "Count") { propertyChanged = true; } };

            vector.Insert(2, 100);

            Assert.AreEqual(true, propertyChanged);
        }

        [TestMethod]
        public void Insert_PropertyChanged_Indexer()
        {
            List<int> collection = new List<int>(new int[] { 1, 3, 7, 10, 42 });
            ObservableVector<int> vector = new ObservableVector<int>(collection);

            bool propertyChanged = false;
            vector.PropertyChanged += (sender, e) => { if (e.PropertyName == "Item[]") { propertyChanged = true; } };

            vector.Insert(2, 100);

            Assert.AreEqual(true, propertyChanged);
        }

        //[TestMethod]
        //public void Insert_VectorChanged()
        //{
        //    List<int> collection = new List<int>(new int[] { 1, 3, 7, 10, 42 });
        //    ObservableVector<int> vector = new ObservableVector<int>(collection);

        //    bool vectorChanged = false;
        //    vector.VectorChanged += (sender, e) => { if (e.CollectionChange == CollectionChange.ItemInserted && e.Index == 2) { vectorChanged = true; } };

        //    vector.Insert(2, 100);

        //    Assert.AreEqual(true, vectorChanged);
        //}

        [TestMethod]
        public void Insert_CollectionChanged()
        {
            List<int> collection = new List<int>(new int[] { 1, 3, 7, 10, 42 });
            ObservableVector<int> vector = new ObservableVector<int>(collection);

            bool vectorChanged = false;
            vector.CollectionChanged += (sender, e) => { if (e.Action == NotifyCollectionChangedAction.Add && e.NewStartingIndex == 2 && (int)e.NewItems[0] == 100) { vectorChanged = true; } };

            vector.Insert(2, 100);

            Assert.AreEqual(true, vectorChanged);
        }

        [TestMethod]
        public void Remove_RemoveItem()
        {
            List<int> collection = new List<int>(new int[] { 1, 3, 7, 10, 42 });
            ObservableVector<int> vector = new ObservableVector<int>(collection);

            vector.Remove(7);

            CollectionAssert.AreEqual(new int[] { 1, 3, 10, 42 }, vector.ToArray());
        }

        [TestMethod]
        public void Remove_PropertyChanged_Count()
        {
            List<int> collection = new List<int>(new int[] { 1, 3, 7, 10, 42 });
            ObservableVector<int> vector = new ObservableVector<int>(collection);

            bool propertyChanged = false;
            vector.PropertyChanged += (sender, e) => { if (e.PropertyName == "Count") { propertyChanged = true; } };

            vector.Remove(7);

            Assert.AreEqual(true, propertyChanged);
        }

        [TestMethod]
        public void Remove_PropertyChanged_Indexer()
        {
            List<int> collection = new List<int>(new int[] { 1, 3, 7, 10, 42 });
            ObservableVector<int> vector = new ObservableVector<int>(collection);

            bool propertyChanged = false;
            vector.PropertyChanged += (sender, e) => { if (e.PropertyName == "Item[]") { propertyChanged = true; } };

            vector.Remove(7);

            Assert.AreEqual(true, propertyChanged);
        }

        //[TestMethod]
        //public void Remove_VectorChanged()
        //{
        //    List<int> collection = new List<int>(new int[] { 1, 3, 7, 10, 42 });
        //    ObservableVector<int> vector = new ObservableVector<int>(collection);

        //    bool vectorChanged = false;
        //    vector.VectorChanged += (sender, e) => { if (e.CollectionChange == CollectionChange.ItemRemoved && e.Index == 2) { vectorChanged = true; } };

        //    vector.Remove(7);

        //    Assert.AreEqual(true, vectorChanged);
        //}

        [TestMethod]
        public void Remove_CollectionChanged()
        {
            List<int> collection = new List<int>(new int[] { 1, 3, 7, 10, 42 });
            ObservableVector<int> vector = new ObservableVector<int>(collection);

            bool vectorChanged = false;
            vector.CollectionChanged += (sender, e) => { if (e.Action == NotifyCollectionChangedAction.Remove && e.OldStartingIndex == 2 && (int)e.OldItems[0] == 7) { vectorChanged = true; } };

            vector.Remove(7);

            Assert.AreEqual(true, vectorChanged);
        }

        [TestMethod]
        public void ItemSetter_SetsItem()
        {
            List<int> collection = new List<int>(new int[] { 1, 3, 7, 10, 42 });
            ObservableVector<int> vector = new ObservableVector<int>(collection);

            vector[2] = 20;

            CollectionAssert.AreEqual(new int[] { 1, 3, 20, 10, 42 }, vector.ToArray());
        }

        [TestMethod]
        public void ItemSetter_PropertyChanged_DoesNotChangeCount()
        {
            List<int> collection = new List<int>(new int[] { 1, 3, 7, 10, 42 });
            ObservableVector<int> vector = new ObservableVector<int>(collection);

            bool propertyChanged = false;
            vector.PropertyChanged += (sender, e) => { if (e.PropertyName == "Count") { propertyChanged = true; } };

            vector[2] = 20;

            Assert.AreEqual(false, propertyChanged);
        }

        [TestMethod]
        public void ItemSetter_PropertyChanged_Indexer()
        {
            List<int> collection = new List<int>(new int[] { 1, 3, 7, 10, 42 });
            ObservableVector<int> vector = new ObservableVector<int>(collection);

            bool propertyChanged = false;
            vector.PropertyChanged += (sender, e) => { if (e.PropertyName == "Item[]") { propertyChanged = true; } };

            vector[2] = 20;

            Assert.AreEqual(true, propertyChanged);
        }

        //[TestMethod]
        //public void ItemSetter_VectorChanged()
        //{
        //    List<int> collection = new List<int>(new int[] { 1, 3, 7, 10, 42 });
        //    ObservableVector<int> vector = new ObservableVector<int>(collection);

        //    bool vectorChanged = false;
        //    vector.VectorChanged += (sender, e) => { if (e.CollectionChange == CollectionChange.ItemChanged && e.Index == 2) { vectorChanged = true; } };

        //    vector[2] = 20;

        //    Assert.AreEqual(true, vectorChanged);
        //}

        [TestMethod]
        public void ItemSetter_CollectionChanged()
        {
            List<int> collection = new List<int>(new int[] { 1, 3, 7, 10, 42 });
            ObservableVector<int> vector = new ObservableVector<int>(collection);

            bool vectorChanged = false;
            vector.CollectionChanged += (sender, e) => { if (e.Action == NotifyCollectionChangedAction.Replace && e.OldStartingIndex == 2 && (int)e.OldItems[0] == 7 && (int)e.NewItems[0] == 20) { vectorChanged = true; } };

            vector[2] = 20;

            Assert.AreEqual(true, vectorChanged);
        }

        // *** Property Tests ***

        [TestMethod]
        public void IsReadOnly_IsFalse()
        {
            ObservableVector<int> vector = new ObservableVector<int>();

            Assert.AreEqual(false, ((IList<int>)vector).IsReadOnly);
        }
    }
}
