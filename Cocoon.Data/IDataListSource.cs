using System.Threading.Tasks;

namespace Cocoon.Data
{
    public interface IDataListSource<T>
    {
        // *** Methods ***

        Task<int> GetCountAsync();
        Task<T> GetItemAsync(int index);
    }
}
