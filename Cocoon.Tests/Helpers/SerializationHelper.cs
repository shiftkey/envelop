using System.IO;
using System.Runtime.Serialization;

namespace Cocoon.Tests.Helpers
{
    public static class SerializationHelper
    {
        public static T DeserializeFromArray<T>(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            {
                var serializer = new DataContractSerializer(typeof(T));
                return (T)serializer.ReadObject(stream);
            }
        }

        public static byte[] SerializeToArray<T>(T value)
        {
            using (var stream = new MemoryStream())
            {
                var serializer = new DataContractSerializer(typeof(T));
                serializer.WriteObject(stream, value);
                return stream.ToArray();
            }
        }
    }
}
