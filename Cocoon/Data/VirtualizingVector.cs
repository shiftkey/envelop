using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Cocoon.Helpers;

namespace Cocoon.Data
{
    public abstract class VirtualizingVector<T> : IList, IList<T>, INotifyPropertyChanged, INotifyCollectionChanged
    {
        // *** Constants ***

        private const string PropertyNameCount = "Count";
        private const string PropertyNameIndexer = "Item[]";

        // *** Fields ***

        private readonly VirtualizingList<ItemContainer> internalList = new VirtualizingList<ItemContainer>();

        private bool fetchCountCalled = false;

        // *** Events ***

        public event PropertyChangedEventHandler PropertyChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        // *** IList<T> Properties ***

        public T this[int index]
        {
            get
            {
                return GetItem(index);
            }
            set
            {
                throw new InvalidOperationException(ResourceHelper.GetErrorResource("Exception_InvalidOperation_CannotModifyReadOnlyCollection"));
            }
        }

        object IList.this[int index]
        {
            get
            {
                return GetItem(index);
            }
            set
            {
                throw new InvalidOperationException(ResourceHelper.GetErrorResource("Exception_InvalidOperation_CannotModifyReadOnlyCollection"));
            }
        }

        public int Count
        {
            get
            {
                return GetCount();
            }
        }

        int ICollection.Count
        {
            get
            {
                return GetCount();
            }
        }

        public bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        bool IList.IsReadOnly
        {
            get
            {
                return true;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        public object SyncRoot
        {
            get
            {
                return this;
            }
        }

        // *** IList<T> Methods ***

        public void Add(T item)
        {
            throw new InvalidOperationException(ResourceHelper.GetErrorResource("Exception_InvalidOperation_CannotModifyReadOnlyCollection"));
        }

        int IList.Add(object value)
        {
            throw new InvalidOperationException(ResourceHelper.GetErrorResource("Exception_InvalidOperation_CannotModifyReadOnlyCollection"));
        }

        public void Clear()
        {
            throw new InvalidOperationException(ResourceHelper.GetErrorResource("Exception_InvalidOperation_CannotModifyReadOnlyCollection"));
        }

        void IList.Clear()
        {
            throw new InvalidOperationException(ResourceHelper.GetErrorResource("Exception_InvalidOperation_CannotModifyReadOnlyCollection"));
        }

        public bool Contains(T item)
        {
            return internalList.Contains(new ItemContainer(item));
        }

        bool IList.Contains(object value)
        {
            return Contains((T)value);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            for (int i = 0; i < internalList.Count; i++)
                array[i + arrayIndex] = GetItem(i);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            for (int i = 0; i < internalList.Count; i++)
                array.SetValue(GetItem(i), new int[] { i + index });
        }

        public int IndexOf(T item)
        {
            return internalList.IndexOf(new ItemContainer(item));
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((T)value);
        }

        public void Insert(int index, T item)
        {
            throw new InvalidOperationException(ResourceHelper.GetErrorResource("Exception_InvalidOperation_CannotModifyReadOnlyCollection"));
        }

        void IList.Insert(int index, object value)
        {
            throw new InvalidOperationException(ResourceHelper.GetErrorResource("Exception_InvalidOperation_CannotModifyReadOnlyCollection"));
        }

        public bool Remove(T item)
        {
            throw new InvalidOperationException(ResourceHelper.GetErrorResource("Exception_InvalidOperation_CannotModifyReadOnlyCollection"));
        }

        void IList.Remove(object value)
        {
            throw new InvalidOperationException(ResourceHelper.GetErrorResource("Exception_InvalidOperation_CannotModifyReadOnlyCollection"));
        }

        public void RemoveAt(int index)
        {
            throw new InvalidOperationException(ResourceHelper.GetErrorResource("Exception_InvalidOperation_CannotModifyReadOnlyCollection"));
        }

        void IList.RemoveAt(int index)
        {
            throw new InvalidOperationException(ResourceHelper.GetErrorResource("Exception_InvalidOperation_CannotModifyReadOnlyCollection"));
        }

        // *** IEnumerable<T> Methods ***

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
                yield return GetItem(i);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)this).GetEnumerator();
        }

        // *** Protected Methods ***

        protected void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected void UpdateCount(int count)
        {
            if (internalList.Count != count)
            {
                // Update the VirtualizingList's Count property

                internalList.UpdateCount(count);

                // Raise property and collection changed events

                OnPropertyChanged(PropertyNameCount);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        protected void UpdateItem(int index, T item)
        {
            // Get the existing item

            T oldItem = internalList[index].Value;

            // Set the VirtualizingList's item

            internalList[index] = new ItemContainer(item);

            // Raise property and collection changed events

            OnPropertyChanged(PropertyNameIndexer);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, oldItem, index));
        }

        // *** Protected Abstract Methods ***

        abstract protected void FetchCount();

        abstract protected void FetchItem(int index);

        // *** Event Handlers ***

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
                CollectionChanged(this, e);
        }

        // *** Private Methods ***

        private int GetCount()
        {
            // Only call FetchCount() for the first call to the Count getter

            if (!fetchCountCalled)
            {
                fetchCountCalled = true;
                FetchCount();
            }

            return internalList.Count;
        }

        private T GetItem(int index)
        {
            // Fetch the item if it is currently not available

            if (!internalList[index].FetchPerformed)
                FetchItem(index);

            // Return the item, or placeholder if not currently available
            // NB: Check for base[index] even if we just ran FetchItem() in case it ran synchronously

            return internalList[index].Value;
        }

        // *** Private Sub-classes ***

        private struct ItemContainer
        {
            // *** Fields ***

            private readonly bool fetchPerformed;
            private readonly T value;

            // *** Constructors ***

            public ItemContainer(T value)
            {
                this.fetchPerformed = true;
                this.value = value;
            }

            // *** Properties ***

            public bool FetchPerformed
            {
                get
                {
                    return fetchPerformed;
                }
            }

            public T Value
            {
                get
                {
                    return value;
                }
            }
        }
    }
}
