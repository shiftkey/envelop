using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Cocoon.Services;
using Cocoon.Tests.Helpers;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Windows.Storage;

namespace Cocoon.Tests.Services
{
    [TestClass]
    public class StorageManagerFixture
    {
        [TestMethod]
        public async Task StoreAsync_WithStorageFile_ThrowsException_IfFileIsNull()
        {
            StorageManager storageManager = new StorageManager();

            TestData data = new TestData() { Text = "Test Text", Number = 42 };
            await AssertEx.ThrowsExceptionAsync<ArgumentNullException>(() => storageManager.StoreAsync(null, data));
        }

        [TestMethod]
        public async Task StoreAsync_WithStorageFolder_ThrowsException_IfFolderIsNull()
        {
            StorageManager storageManager = new StorageManager();

            StorageFolder folder = null;
            string name = GetTestFilename();
            TestData data = new TestData() { Text = "Test Text", Number = 42 };
            await AssertEx.ThrowsExceptionAsync<ArgumentNullException>(() => storageManager.StoreAsync(folder, name, data));
        }

        [TestMethod]
        public async Task StoreAsync_WithStorageFolder_ThrowsException_IfNameIsNull()
        {
            StorageManager storageManager = new StorageManager();

            StorageFolder folder = ApplicationData.Current.TemporaryFolder;
            TestData data = new TestData() { Text = "Test Text", Number = 42 };
            await AssertEx.ThrowsExceptionAsync<ArgumentException>(() => storageManager.StoreAsync(folder, null, data));
        }

        [TestMethod]
        public async Task StoreAsync_WithStorageFolder_ThrowsException_IfNameIsEmpty()
        {
            StorageManager storageManager = new StorageManager();

            StorageFolder folder = ApplicationData.Current.TemporaryFolder;
            TestData data = new TestData() { Text = "Test Text", Number = 42 };
            await AssertEx.ThrowsExceptionAsync<ArgumentException>(() => storageManager.StoreAsync(folder, "", data));
        }

        [TestMethod]
        public async Task RetrieveAsync_WithStorageFolder_ReturnsNullIfFileDoesNotExist()
        {
            StorageManager storageManager = new StorageManager();

            StorageFolder folder = ApplicationData.Current.TemporaryFolder;
            string name = GetTestFilename();

            TestData retrievedData = await storageManager.RetrieveAsync<TestData>(folder, name);

            Assert.IsNull(retrievedData);
        }

        [TestMethod]
        public async Task RetrieveAsync_WithStorageFile_ThrowsException_IfFileIsNull()
        {
            StorageManager storageManager = new StorageManager();

            await AssertEx.ThrowsExceptionAsync<ArgumentNullException>(() => storageManager.RetrieveAsync<TestData>(null));
        }

        [TestMethod]
        public async Task RetrieveAsync_WithStorageFolder_ThrowsException_IfFolderIsNull()
        {
            StorageManager storageManager = new StorageManager();

            StorageFolder folder = null;
            string name = GetTestFilename();
            await AssertEx.ThrowsExceptionAsync<ArgumentNullException>(() => storageManager.RetrieveAsync<TestData>(folder, name));
        }

        [TestMethod]
        public async Task RetrieveAsync_WithStorageFolder_ThrowsException_IfNameIsNull()
        {
            StorageManager storageManager = new StorageManager();

            StorageFolder folder = ApplicationData.Current.TemporaryFolder;
            await AssertEx.ThrowsExceptionAsync<ArgumentException>(() => storageManager.RetrieveAsync<TestData>(folder, null));
        }

        [TestMethod]
        public async Task RetrieveAsync_WithStorageFolder_ThrowsException_IfNameIsEmpty()
        {
            StorageManager storageManager = new StorageManager();

            StorageFolder folder = ApplicationData.Current.TemporaryFolder;
            await AssertEx.ThrowsExceptionAsync<ArgumentException>(() => storageManager.RetrieveAsync<TestData>(folder, ""));
        }

        [TestMethod]
        public async Task StoreAsyncRetrieveAsync_WithStorageFile_PersistsFileViaStorage()
        {
            StorageManager storageManager = new StorageManager();
            StorageFolder folder = ApplicationData.Current.TemporaryFolder;
            StorageFile file = await folder.CreateFileAsync(GetTestFilename(), CreationCollisionOption.ReplaceExisting);

            TestData data = new TestData() { Text = "Test Text", Number = 42 };
            await storageManager.StoreAsync(file, data);

            TestData retrievedData = await storageManager.RetrieveAsync<TestData>(file);

            Assert.AreEqual("Test Text", retrievedData.Text);
            Assert.AreEqual(42, retrievedData.Number);
        }

        private string GetTestFilename([CallerMemberName]string callerName = null)
        {
            return callerName;
        }

        [DataContract]
        private class TestData
        {
            [DataMember]
            public string Text { get; set; }
            [DataMember]
            public int Number { get; set; }
        }
    }
}
