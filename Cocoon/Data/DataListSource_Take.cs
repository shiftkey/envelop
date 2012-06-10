using System;
using System.Threading.Tasks;
using Cocoon.Helpers;

namespace Cocoon.Data
{
    internal class DataListSource_Take<T> : IDataListSource<T>
    {
        // *** Fields ***

        private readonly IDataListSource<T> source;
        private readonly int count;

        // *** Constructors ***

        public DataListSource_Take(IDataListSource<T> source, int count)
        {
            this.source = source;
            this.count = count;
        }

        // *** Methods ***

        public async Task<int> GetCountAsync()
        {
            // Get the source count

            int sourceCount = await source.GetCountAsync();

            // Return the minimum value of source and 'count'

            return Math.Min(sourceCount, count);
        }

        public Task<T> GetItemAsync(int index)
        {
            // If the index is outside of the bounds of the Take then throw an exception
            // NB: If the source is shorter than the count then it will handle throwing the exception

            if (index < 0 || index >= count)
                throw new ArgumentOutOfRangeException(ResourceHelper.GetErrorResource("Exception_ArgumentOutOfRange_ArrayIndexOutOfRange"));

            // Otherwise defer to the source

            return source.GetItemAsync(index);
        }
    }
}
