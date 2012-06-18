using System.Threading.Tasks;
using Cocoon.Navigation;
using Cocoon.Services;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using NSubstitute;
using Windows.ApplicationModel.Activation;

namespace Cocoon.Tests
{
    [TestClass]
    public class SomeTest
    {
        public interface AnInterface
        {
            int GetAValue();
        }

        [TestMethod]
        public void TestThatAValueIsReturned()
        {
            var mock = Substitute.For<AnInterface>();
            mock.GetAValue().Returns(5);
            Assert.AreEqual(5, mock.GetAValue());
        }

        [TestMethod]
        public void TestThatAMethodWasCalled()
        {
            var mock = Substitute.For<AnInterface>();
            mock.GetAValue().Returns(5);
            var result = mock.GetAValue();
            mock.Received(1).GetAValue();
        }
    }
}
