using System;
using System.Threading.Tasks;
using Cocoon.Navigation;
using Cocoon.Services;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using NSubstitute;
using Windows.ApplicationModel.Activation;

namespace Cocoon.Tests.Services
{
    [TestClass]
    public class ActivationManagerFixture
    {
        INavigationManager navigationManager = Substitute.For<INavigationManager>();
        ActivationManager activationManager;

        public ActivationManagerFixture()
        {
            activationManager = new ActivationManager(navigationManager);
        }

        [TestMethod]
        public async Task Activate_ReturnsFalse_IfActivationTypeIsNotSupported()
        {
            var args = Substitute.For<IActivatedEventArgs>();
            args.Kind.Returns(ActivationKind.CameraSettings);

            var success = await activationManager.Activate(args);

            Assert.IsFalse(success);
        }

        [TestMethod]
        public async Task Activate_Launch_RestoresNavigationIfApplicable()
        {
            navigationManager.RestoreNavigationStack().Returns(Task.FromResult(true));
            
            var args = Substitute.For<ILaunchActivatedEventArgs>();
            await activationManager.Activate(args);

            navigationManager.ReceivedWithAnyArgs(1).NavigateTo(Arg.Any<string>());
        }

        [TestMethod]
        public async Task Activate_Launch_NavigatesToHomePageIfNoPreviousNavigationStack()
        {
            navigationManager.RestoreNavigationStack().Returns(Task.FromResult(false));

            var args = Substitute.For<ILaunchActivatedEventArgs>();
            await activationManager.Activate(args);

            navigationManager.Received().NavigateTo("MockHomePage");
        }

        [TestMethod]
        public async Task Activate_Launch_NavigatesToHomePageIfPreviousExecutionClosedByUser()
        {
            navigationManager.RestoreNavigationStack().Returns(Task.FromResult(true));

            var args = Substitute.For<ILaunchActivatedEventArgs>();
            args.PreviousExecutionState.Returns(ApplicationExecutionState.ClosedByUser);

            await activationManager.Activate(args);
            
            navigationManager.Received().NavigateTo("MockHomePage");
        }

        [TestMethod]
        public async Task Activate_Launch_NavigatesToHomePageIfPreviousExecutionNotRunning()
        {
            navigationManager.RestoreNavigationStack().Returns(Task.FromResult(true));

            var args = Substitute.For<ILaunchActivatedEventArgs>();
            args.PreviousExecutionState.Returns(ApplicationExecutionState.NotRunning);

            await activationManager.Activate(args);

            navigationManager.Received().NavigateTo("MockHomePage");
        }
    }
}
