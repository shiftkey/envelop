using System;
using System.Threading.Tasks;
using Cocoon.Helpers;

namespace Cocoon.Data
{
    internal class DataListSource_Skip<T> : IDataListSource<T>
    {
        // *** Fields ***

        private readonly IDataListSource<T> source;
        private readonly int count;

        // *** Constructors ***

        public DataListSource_Skip(IDataListSource<T> source, int count)
        {
            this.source = source;
            this.count = count;
        }

        // *** Methods ***

        public async Task<int> GetCountAsync()
        {
            // Get the source count

            int sourceCount = await source.GetCountAsync();

            // Return the source count, minus 'count' (or zero if negative)

            int resultCount = sourceCount - count;
            return resultCount > 0 ? resultCount : 0;
        }

        public Task<T> GetItemAsync(int index)
        {
            // If the index is outside of the bounds of the Skip then throw an exception
            // NB: Don't need to validate the upper bounds as the source will do this for us

            if (index < 0)
                throw new ArgumentOutOfRangeException(ResourceHelper.GetErrorResource("Exception_ArgumentOutOfRange_ArrayIndexOutOfRange"));

            // Otherwise defer to the source with the relevant offset

            return source.GetItemAsync(index + count);
        }
    }
}
