using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cocoon.Data
{
    public abstract class DataListSourceBase<T> : IDataListSource<T>
    {
        // *** Protected Properties ***

        protected IList<T> InternalList
        {
            get;
            set;
        }

        // *** IDataListSource<T> Methods ***

        public abstract Task<int> GetCountAsync();
        public abstract Task<T> GetItemAsync(int index);
    }
}
