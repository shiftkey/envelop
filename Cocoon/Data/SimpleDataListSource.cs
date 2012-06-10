using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cocoon.Helpers;

namespace Cocoon.Data
{
    public abstract class SimpleDataListSource<T> : DataListSourceBase<T>
    {
        // *** Fields ***

        private Task fetchingTask;

        // *** IDataListSource<T> Methods ***

        public async override Task<int> GetCountAsync()
        {
            // If we are not initialized then await fetching the list

            if (InternalList == null)
                await GetFetchingTask();

            // Return the result from the cached list

            return InternalList.Count;
        }

        public async override Task<T> GetItemAsync(int index)
        {
            // Validate arguments

            if (index < 0)
                throw new ArgumentOutOfRangeException("index", ResourceHelper.GetErrorResource("Exception_ArgumentOutOfRange_ArrayIndexOutOfRange"));

            // If we are not initialized then await fetching the list

            if (InternalList == null)
                await GetFetchingTask();

            // Return the result from the cached list (or throw an exception if after the last item)

            if (index >= InternalList.Count)
                throw new ArgumentOutOfRangeException("index", ResourceHelper.GetErrorResource("Exception_ArgumentOutOfRange_ArrayIndexOutOfRange"));
            else
                return InternalList[index];
        }

        // *** Protected Methods ***

        protected abstract Task<IList<T>> FetchItemsAsync();

        // *** Private Methods ***

        private Task GetFetchingTask()
        {
            if (fetchingTask == null)
                fetchingTask = FetchingTask();

            return fetchingTask;
        }

        private async Task FetchingTask()
        {
            // Call the deriving class to get the items

            InternalList = await FetchItemsAsync();
        }
    }
}
