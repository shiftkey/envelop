using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Cocoon.Helpers;
using Windows.Storage;

namespace Cocoon.Services
{
    public class StorageManager : IStorageManager
    {
        public Task<T> RetrieveAsync<T>(StorageFile file)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            return RetrieveAsyncInternal<T>(file);
        }

        private static async Task<T> RetrieveAsyncInternal<T>(StorageFile file)
        {
            using (var fileStream = await file.OpenStreamForReadAsync())
            {
                using (var memoryStream = new MemoryStream())
                {
                    await fileStream.CopyToAsync(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    var serializer = new DataContractSerializer(typeof(T));
                    return (T)serializer.ReadObject(memoryStream);
                }
            }
        }

        public Task<T> RetrieveAsync<T>(StorageFolder folder, string name)
        {
            if (folder == null)
                throw new ArgumentNullException("folder");

            if (string.IsNullOrEmpty(name))
                throw new ArgumentException(ResourceHelper.GetErrorResource("Exception_ArgumentException_StringIsNullOrEmpty"), "name");

            return RetrieveAsyncInternal<T>(folder, name);
        }

        private async Task<T> RetrieveAsyncInternal<T>(StorageFolder folder, string name)
        {
            // Open the file, if it doesn't exist then return default, otherwise pass on
            // NB : Currently the way to check if a file exists in WinRT is by catching the exception
            // TODO : Check with Windows 8 RTM whether GetFileAsync(...) method returns null if a file doesn't exist or another method allows checking of this

            try
            {
                var file = await folder.GetFileAsync(name);
                return await RetrieveAsync<T>(file);
            }
            catch (FileNotFoundException)
            {
                return default(T);
            }
        }

        public Task StoreAsync<T>(StorageFile file, T value)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            return StoreAsyncInternal(file, value);
        }

        private static async Task StoreAsyncInternal<T>(IStorageFile file, T value)
        {
            // Write the object to a MemoryStream using the DataContractSerializer
            // NB: Do this so that,
            //        (i)  We store the state of the object at this point in case it changes before we open the file
            //        (ii) DataContractSerializer doesn't provide async methods for writing to storage
            // TODO : Alternatively we could perform this directly on the file stream and call 'await fileStream.FlushAsync()' (will this ever block?)

            using (var dataStream = new MemoryStream())
            {
                var serializer = new DataContractSerializer(typeof(T));
                serializer.WriteObject(dataStream, value);

                // Save the data to the file stream

                using (var fileStream = await file.OpenStreamForWriteAsync())
                {
                    dataStream.Seek(0, SeekOrigin.Begin);
                    await dataStream.CopyToAsync(fileStream);
                    await fileStream.FlushAsync();
                }
            }
        }

        public Task StoreAsync<T>(StorageFolder folder, string name, T value)
        {
            if (folder == null)
                throw new ArgumentNullException("folder");

            if (string.IsNullOrEmpty(name))
                throw new ArgumentException(ResourceHelper.GetErrorResource("Exception_ArgumentException_StringIsNullOrEmpty"), "name");

            return StoreAsyncInternal(folder, name, value);
        }

        public async Task StoreAsyncInternal<T>(StorageFolder folder, string name, T value)
        {
            // Create the new file, overwriting the existing data, then pass on

            var file = await folder.CreateFileAsync(name, CreationCollisionOption.ReplaceExisting);
            await StoreAsync(file, value);
        }
    }
}
