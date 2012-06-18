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

    
    public class SomeTougherTests
    {
        INavigationManager navigationManager = Substitute.For<INavigationManager>();
        ActivationManager activationManager;

        public SomeTougherTests()
        {
            activationManager = new ActivationManager(navigationManager);
        }

        // this same test fails in ActivationManagerFixture.cs - how curious

        [TestMethod]
        public async Task Activate_Launch_NavigatesToHomePageIfNoPreviousNavigationStack()
        {
            navigationManager.RestoreNavigationStack().Returns(Task.FromResult(false));

            var args = Substitute.For<ILaunchActivatedEventArgs>();
            await activationManager.Activate(args);

            navigationManager.Received().NavigateTo("MockHomePage");
        }


        [TestMethod]
        public async Task Activate_ReturnsFalse_IfActivationTypeIsNotSupported()
        {
            var args = Substitute.For<IActivatedEventArgs>();
            args.Kind.Returns(ActivationKind.CameraSettings);

            var success = await activationManager.Activate(args);

            Assert.IsFalse(success);
        }
    }
}
