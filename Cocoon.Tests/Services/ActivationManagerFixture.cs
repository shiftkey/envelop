using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cocoon.Navigation;
using Cocoon.Services;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Windows.ApplicationModel.Activation;

namespace Cocoon.Tests.Services
{
    [TestClass]
    public class ActivationManagerFixture
    {
        // *** Method Tests ***

        [TestMethod]
        public async Task Activate_ReturnsFalse_IfActivationTypeIsNotSupported()
        {
            IActivationManager activationManager = CreateActivationManager();

            IActivatedEventArgs activatedEventArgs = new MockActivatedEventArgs() { Kind = ActivationKind.CameraSettings };
            bool success = await activationManager.Activate(activatedEventArgs, SpecialPageNames.HomePage);

            Assert.AreEqual(false, success);
        }

        [TestMethod]
        public async Task Activate_Launch_RestoresNavigationIfApplicable()
        {
            MockNavigationManager navigationManager = new MockNavigationManager() { CanRestoreNavigationStack = true };
            IActivationManager activationManager = CreateActivationManager(navigationManager);

            // Activate the application

            await activationManager.Activate(new MockLaunchActivatedEventArgs(), SpecialPageNames.HomePage);

            // Assert that the home page was navigated to

            Assert.AreEqual(1, navigationManager.RestoreNavigationStackCount);
            CollectionAssert.AreEqual(new string[] { }, navigationManager.NavigationList);
        }

        [TestMethod]
        public async Task Activate_Launch_NavigatesToHomePageIfNoPreviousNavigationStack()
        {
            MockNavigationManager navigationManager = new MockNavigationManager() { CanRestoreNavigationStack = false };
            IActivationManager activationManager = CreateActivationManager(navigationManager);

            // Activate the application

            await activationManager.Activate(new MockLaunchActivatedEventArgs(), "MockHomePage");

            // Assert that the home page was navigated to

            Assert.AreEqual(1, navigationManager.RestoreNavigationStackCount);
            CollectionAssert.AreEqual(new string[] { "MockHomePage" }, navigationManager.NavigationList);
        }

        [TestMethod]
        public async Task Activate_Launch_NavigatesToHomePageIfPreviousExecutionClosedByUser()
        {
            MockNavigationManager navigationManager = new MockNavigationManager() { CanRestoreNavigationStack = true };
            IActivationManager activationManager = CreateActivationManager(navigationManager);

            // Activate the application

            await activationManager.Activate(new MockLaunchActivatedEventArgs() { PreviousExecutionState = ApplicationExecutionState.ClosedByUser }, SpecialPageNames.HomePage);

            // Assert that the home page was navigated to

            CollectionAssert.AreEqual(new string[] { "MockHomePage" }, navigationManager.NavigationList);
        }

        [TestMethod]
        public async Task Activate_Launch_NavigatesToHomePageIfPreviousExecutionNotRunning()
        {
            MockNavigationManager navigationManager = new MockNavigationManager() { CanRestoreNavigationStack = true };
            IActivationManager activationManager = CreateActivationManager(navigationManager);

            // Activate the application

            await activationManager.Activate(new MockLaunchActivatedEventArgs() { PreviousExecutionState = ApplicationExecutionState.NotRunning }, SpecialPageNames.HomePage);

            // Assert that the home page was navigated to

            CollectionAssert.AreEqual(new string[] { "MockHomePage" }, navigationManager.NavigationList);
        }

        // *** Private Methods ***

        private ActivationManager CreateActivationManager(INavigationManager navigationManager = null)
        {
            if (navigationManager == null)
                navigationManager = new MockNavigationManager();

            ActivationManager activationManager = new ActivationManager(navigationManager);

            return activationManager;
        }

        // *** Private sub-classes ***

        private class MockNavigationManager : INavigationManager
        {
            // *** Fields ***

            public bool CanRestoreNavigationStack = false;
            public List<string> NavigationList = new List<string>();
            public int RestoreNavigationStackCount;

            // *** Properties ***

            public bool CanGoBack
            {
                get { throw new NotImplementedException(); }
            }

            public string HomePageName
            {
                get
                {
                    return "MockHomePage";
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public NavigationStorageType NavigationStorageType
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            // *** Methods ***

            public void GoBack()
            {
                throw new NotImplementedException();
            }

            public void NavigateTo(string pageName)
            {
                NavigationList.Add(pageName);
            }

            public void NavigateTo(string pageName, object arguments)
            {
                throw new NotImplementedException();
            }

            public Task<bool> RestoreNavigationStack()
            {
                RestoreNavigationStackCount++;
                return Task.FromResult(CanRestoreNavigationStack);
            }

            public Task ActivateNavigation(IActivatedEventArgs activatedEventArgs)
            {
                throw new NotImplementedException();
            }
        }

        private class MockActivatedEventArgs : IActivatedEventArgs
        {
            // *** Properties ***

            public ActivationKind Kind
            {
                get;
                set;
            }

            public ApplicationExecutionState PreviousExecutionState
            {
                get;
                set;
            }

            public SplashScreen SplashScreen
            {
                get { throw new NotImplementedException(); }
            }
        }

        private class MockLaunchActivatedEventArgs : MockActivatedEventArgs, ILaunchActivatedEventArgs
        {
            // *** Constructors ***

            public MockLaunchActivatedEventArgs()
            {
                base.Kind = ActivationKind.Launch;
                base.PreviousExecutionState = ApplicationExecutionState.Terminated;
            }

            // *** Propertes ***

            public string Arguments
            {
                get { throw new NotImplementedException(); }
            }

            public string TileId
            {
                get { throw new NotImplementedException(); }
            }
        }
    }
}
