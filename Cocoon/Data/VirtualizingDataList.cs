using System;
using System.Threading.Tasks;

namespace Cocoon.Data
{
    public class VirtualizingDataList<T> : VirtualizingVector<T> where T : new()
    {
        // *** Fields ***

        private readonly IDataListSource<T> dataListSource;
        private bool isLoading;

        // *** Constructors ***

        public VirtualizingDataList(IDataListSource<T> dataListSource)
        {
            // Validate the parameters

            if (dataListSource == null)
                throw new ArgumentNullException("dataListSource");

            // Set the fields

            this.dataListSource = dataListSource;
        }

        // *** Properties ***

        public bool IsLoading
        {
            get
            {
                return isLoading;
            }
            set
            {
                if (isLoading != value)
                {
                    isLoading = value;
                    OnPropertyChanged("IsLoading");
                }
            }
        }

        // *** Overriden Base Methods ***

        protected override void FetchCount()
        {
            FetchCountAsync();
        }

        protected override void FetchItem(int index)
        {
            FetchItemAsync(index);
        }

        // *** Private Methods ***

        private async Task FetchCountAsync()
        {
            IsLoading = true;
            int count = await dataListSource.GetCountAsync();
            UpdateCount(count);
            IsLoading = false;
        }

        private async Task FetchItemAsync(int index)
        {
            T item = await dataListSource.GetItemAsync(index);
            base.UpdateItem(index, item);
        }
    }
}
