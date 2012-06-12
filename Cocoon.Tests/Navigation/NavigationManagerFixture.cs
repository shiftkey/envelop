using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Cocoon.Navigation;
using Cocoon.Services;
using Cocoon.Tests.Helpers;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Windows.Storage;
using Windows.UI.Xaml.Navigation;

namespace Cocoon.Tests.Navigation
{
    [TestClass]
    public class NavigationManagerFixture
    {
        // *** Property Tests ***

        [TestMethod]
        public void CanGoBack_IsFalseIfNoPagesNavigated()
        {
            MockNavigationTarget navigationTarget = new MockNavigationTarget();
            INavigationManager navigationManager = CreateNavigationManager(navigationTarget);

            Assert.AreEqual(false, navigationManager.CanGoBack);
        }

        [TestMethod]
        public void CanGoBack_IsFalseIfOnePageNavigated()
        {
            MockNavigationTarget navigationTarget = new MockNavigationTarget();
            INavigationManager navigationManager = CreateNavigationManager(navigationTarget);

            navigationManager.NavigateTo("Page 1");

            Assert.AreEqual(false, navigationManager.CanGoBack);
        }

        [TestMethod]
        public void CanGoBack_IsFalseIfTwoPagesNavigatedThenBack()
        {
            MockNavigationTarget navigationTarget = new MockNavigationTarget();
            INavigationManager navigationManager = CreateNavigationManager(navigationTarget);

            navigationManager.NavigateTo("Page 1");
            navigationManager.NavigateTo("Page 2");
            navigationManager.GoBack();

            Assert.AreEqual(false, navigationManager.CanGoBack);
        }

        [TestMethod]
        public void CanGoBack_IsTrueIfTwoPagesNavigated()
        {
            MockNavigationTarget navigationTarget = new MockNavigationTarget();
            INavigationManager navigationManager = CreateNavigationManager(navigationTarget);

            navigationManager.NavigateTo("Page 1");
            navigationManager.NavigateTo("Page 2");

            Assert.AreEqual(true, navigationManager.CanGoBack);
        }

        [TestMethod]
        public void HomePageName_DefaultsToSpecialPageName()
        {
            INavigationManager navigationManager = CreateNavigationManager(setHomePageName: false);

            Assert.AreEqual(SpecialPageNames.Home, navigationManager.HomePageName);
        }

        [TestMethod]
        public void HomePage_CanSetValue()
        {
            INavigationManager navigationManager = CreateNavigationManager();

            navigationManager.HomePageName = "Test Home Page";

            Assert.AreEqual("Test Home Page", navigationManager.HomePageName);
        }

        [TestMethod]
        public void HomePage_ThrowsException_IfHomePageNameIsNull()
        {
            INavigationManager navigationManager = CreateNavigationManager();

            Assert.ThrowsException<ArgumentException>(() => navigationManager.HomePageName = null);
        }

        [TestMethod]
        public void HomePage_ThrowsException_IfHomePageNameIsEmpty()
        {
            INavigationManager navigationManager = CreateNavigationManager();

            Assert.ThrowsException<ArgumentException>(() => navigationManager.HomePageName = "");
        }

        [TestMethod]
        public void NavigationStorageType_DefaultsToNone()
        {
            INavigationManager navigationManager = CreateNavigationManager();

            Assert.AreEqual(NavigationStorageType.None, navigationManager.NavigationStorageType);
        }

        [TestMethod]
        public void NavigationStorageType_CanSetValue()
        {
            INavigationManager navigationManager = CreateNavigationManager();

            navigationManager.NavigationStorageType = NavigationStorageType.Local;

            Assert.AreEqual(NavigationStorageType.Local, navigationManager.NavigationStorageType);
        }

        [TestMethod]
        public void NavigationTarget_SetViaConstructor()
        {
            MockNavigationTarget navigationTarget = new MockNavigationTarget();
            NavigationManager navigationManager = CreateNavigationManager(navigationTarget);

            Assert.AreEqual(navigationTarget, navigationManager.NavigationTarget);
        }

        [TestMethod]
        public void NavigationTarget_DefaultsToWindowNavigationTarget()
        {
            NavigationManager navigationManager = CreateNavigationManager(navigationTargetIsNull: true);

            Assert.IsInstanceOfType(navigationManager.NavigationTarget, typeof(WindowNavigationTarget));
        }

        // *** Method Tests ***

        [TestMethod]
        public void GoBack_NavigatesToNextPageInStack()
        {
            MockNavigationTarget navigationTarget = new MockNavigationTarget();
            INavigationManager navigationManager = CreateNavigationManager(navigationTarget);

            navigationManager.NavigateTo("Page 1");
            navigationManager.NavigateTo("Page 2");
            navigationManager.GoBack();

            string[] pageNames = navigationTarget.NavigatedPages.Cast<MockPage>().Select(page => page.PageName).ToArray();
            CollectionAssert.AreEqual(new string[] { "Page 1", "Page 2", "Page 1" }, pageNames);
        }

        [TestMethod]
        public void GoBack_DisposesCurrentPage_WithViewModel()
        {
            MockNavigationTarget navigationTarget = new MockNavigationTarget();
            INavigationManager navigationManager = CreateNavigationManager(navigationTarget);

            navigationManager.NavigateTo("Page 1");
            navigationManager.NavigateTo("Page 2");
            MockPage currentPage = (MockPage)navigationTarget.NavigatedPages.Last();
            navigationManager.GoBack();

            Assert.AreEqual(true, currentPage.IsDisposed);
        }

        [TestMethod]
        public void GoBack_DisposesCurrentViewModel()
        {
            MockNavigationTarget navigationTarget = new MockNavigationTarget();
            INavigationManager navigationManager = CreateNavigationManager(navigationTarget);

            navigationManager.NavigateTo("Page 1");
            navigationManager.NavigateTo("Page 2");
            MockPage currentPage = (MockPage)navigationTarget.NavigatedPages.Last();
            MockViewModel<string, string> currentViewModel = (MockViewModel<string, string>)currentPage.DataContext;
            navigationManager.GoBack();

            Assert.AreEqual(true, currentViewModel.IsDisposed);
        }

        [TestMethod]
        public void GoBack_CallsNavigatingFromOnPreviousViewModel()
        {
            MockNavigationTarget navigationTarget = new MockNavigationTarget();
            MockViewFactory_WithNavigationAware viewFactory = new MockViewFactory_WithNavigationAware();
            INavigationManager navigationManager = CreateNavigationManager(navigationTarget, viewFactory);

            navigationManager.NavigateTo("Page 1");

            navigationManager.NavigateTo("Page 2");
            MockPage currentPage = (MockPage)navigationTarget.NavigatedPages.Last();
            MockNavigationAwareViewModel<string, int> currentViewModel = (MockNavigationAwareViewModel<string, int>)currentPage.DataContext;
            currentViewModel.NavigationEvents.Clear();

            navigationManager.GoBack();

            CollectionAssert.AreEqual(new string[] { "NavigatingFrom(Back)" }, currentViewModel.NavigationEvents);
        }

        [TestMethod]
        public void GoBack_CallsNavigatingToOnNewViewModel()
        {
            MockNavigationTarget navigationTarget = new MockNavigationTarget();
            MockViewFactory_WithNavigationAware viewFactory = new MockViewFactory_WithNavigationAware();
            INavigationManager navigationManager = CreateNavigationManager(navigationTarget, viewFactory);

            navigationManager.NavigateTo("Page 1");
            MockPage currentPage = (MockPage)navigationTarget.NavigatedPages.Last();
            MockNavigationAwareViewModel<string, int> currentViewModel = (MockNavigationAwareViewModel<string, int>)currentPage.DataContext;
            
            navigationManager.NavigateTo("Page 2");
            currentViewModel.NavigationEvents.Clear();

            navigationManager.GoBack();

            CollectionAssert.AreEqual(new string[] { "NavigatedTo(Back)" }, currentViewModel.NavigationEvents);
        }

        [TestMethod]
        public void GoBack_ThrowsException_NoPageInBackStack()
        {
            MockNavigationTarget navigationTarget = new MockNavigationTarget();
            INavigationManager navigationManager = CreateNavigationManager(navigationTarget);

            navigationManager.NavigateTo("Page 1");

            Assert.ThrowsException<InvalidOperationException>(() => navigationManager.GoBack());
        }

        [TestMethod]
        public void NavigateTo_NavigatesToSpecifiedPage_WithViewModel()
        {
            MockNavigationTarget navigationTarget = new MockNavigationTarget();
            INavigationManager navigationManager = CreateNavigationManager(navigationTarget);

            navigationManager.NavigateTo("Page 2");

            string[] pageNames = navigationTarget.NavigatedPages.Cast<MockPage>().Select(page => page.PageName).ToArray();
            CollectionAssert.AreEqual(new string[] { "Page 2" }, pageNames);
        }

        [TestMethod]
        public void NavigateTo_NavigatesToSpecifiedPage_WithoutViewModel()
        {
            MockNavigationTarget navigationTarget = new MockNavigationTarget();
            INavigationManager navigationManager = CreateNavigationManager(navigationTarget);

            navigationManager.NavigateTo("Page 3");

            string[] pageNames = navigationTarget.NavigatedPages.Cast<MockPage>().Select(page => page.PageName).ToArray();
            CollectionAssert.AreEqual(new string[] { "Page 3" }, pageNames);
        }

        [TestMethod]
        public void NavigateTo_NavigatedTwice_NavigatesToSpecifiedPage()
        {
            MockNavigationTarget navigationTarget = new MockNavigationTarget();
            INavigationManager navigationManager = CreateNavigationManager(navigationTarget);

            navigationManager.NavigateTo("Page 1");
            navigationManager.NavigateTo("Page 2");

            string[] pageNames = navigationTarget.NavigatedPages.Cast<MockPage>().Select(page => page.PageName).ToArray();
            CollectionAssert.AreEqual(new string[] { "Page 1", "Page 2" }, pageNames);
        }

        [TestMethod]
        public void NavigateTo_SetsSpecifiedViewModel_WithViewModel()
        {
            MockNavigationTarget navigationTarget = new MockNavigationTarget();
            INavigationManager navigationManager = CreateNavigationManager(navigationTarget);

            navigationManager.NavigateTo("Page 2");

            MockPage navigatedPage = (MockPage)navigationTarget.NavigatedPages.Last();
            object dataContext = navigatedPage.DataContext;

            Assert.IsNotNull(dataContext);
            Assert.IsInstanceOfType(dataContext, typeof(MockViewModel<string, string>));
            Assert.AreEqual("ViewModel 2", ((MockViewModel<string, string>)dataContext).Name);
        }

        [TestMethod]
        public void NavigateTo_SetsNullViewModel_WithoutViewModel()
        {
            MockNavigationTarget navigationTarget = new MockNavigationTarget();
            INavigationManager navigationManager = CreateNavigationManager(navigationTarget);

            navigationManager.NavigateTo("Page 3");

            MockPage navigatedPage = (MockPage)navigationTarget.NavigatedPages.Last();
            object dataContext = navigatedPage.DataContext;

            Assert.IsNull(dataContext);
        }

        [TestMethod]
        public void NavigateTo_PassesNullArgumentToViewModel()
        {
            MockNavigationTarget navigationTarget = new MockNavigationTarget();
            INavigationManager navigationManager = CreateNavigationManager(navigationTarget);

            navigationManager.NavigateTo("Page 2");

            MockPage navigatedPage = (MockPage)navigationTarget.NavigatedPages.Last();
            object dataContext = navigatedPage.DataContext;

            Assert.IsNotNull(dataContext);
            Assert.IsInstanceOfType(dataContext, typeof(MockViewModel<string, string>));
            Assert.AreEqual(true, ((MockViewModel<string, string>)dataContext).IsActivated);
            Assert.AreEqual(null, ((MockViewModel<string, string>)dataContext).ActivationArguments);
        }

        [TestMethod]
        public void NavigateTo_WithArguments_PassesArgumentToViewModel()
        {
            MockNavigationTarget navigationTarget = new MockNavigationTarget();
            INavigationManager navigationManager = CreateNavigationManager(navigationTarget);

            navigationManager.NavigateTo("Page 2", "Test Argument");

            MockPage navigatedPage = (MockPage)navigationTarget.NavigatedPages.Last();
            object dataContext = navigatedPage.DataContext;

            Assert.IsNotNull(dataContext);
            Assert.IsInstanceOfType(dataContext, typeof(MockViewModel<string, string>));
            Assert.AreEqual(true, ((MockViewModel<string, string>)dataContext).IsActivated);
            Assert.AreEqual("Test Argument", ((MockViewModel<string, string>)dataContext).ActivationArguments);
        }

        [TestMethod]
        public void NavigateTo_PassesNullStateToViewModel_WithStringState()
        {
            MockNavigationTarget navigationTarget = new MockNavigationTarget();
            INavigationManager navigationManager = CreateNavigationManager(navigationTarget);

            navigationManager.NavigateTo("Page 2", "Test Argument");

            MockPage navigatedPage = (MockPage)navigationTarget.NavigatedPages.Last();
            object dataContext = navigatedPage.DataContext;

            Assert.IsNotNull(dataContext);
            Assert.IsInstanceOfType(dataContext, typeof(MockViewModel<string, string>));
            Assert.AreEqual(true, ((MockViewModel<string, string>)dataContext).IsActivated);
            Assert.AreEqual(null, ((MockViewModel<string, string>)dataContext).ActivationState);
        }

        [TestMethod]
        public void NavigateTo_PassesNullStateToViewModel_WithComplexState()
        {
            MockNavigationTarget navigationTarget = new MockNavigationTarget();
            IViewFactory viewFactory = new MockViewFactory_WithComplexState();
            INavigationManager navigationManager = CreateNavigationManager(navigationTarget: navigationTarget, viewFactory: viewFactory);

            navigationManager.NavigateTo("Page 2", "Test Argument");

            MockPage navigatedPage = (MockPage)navigationTarget.NavigatedPages.Last();
            object dataContext = navigatedPage.DataContext;

            Assert.IsNotNull(dataContext);
            Assert.IsInstanceOfType(dataContext, typeof(MockViewModel<string, TestData>));
            Assert.AreEqual(true, ((MockViewModel<string, TestData>)dataContext).IsActivated);
            Assert.AreEqual(null, ((MockViewModel<string, TestData>)dataContext).ActivationState);
        }

        [TestMethod]
        public void NavigateTo_PassesNullStateToViewModel_WithNonNullableState()
        {
            MockNavigationTarget navigationTarget = new MockNavigationTarget();
            IViewFactory viewFactory = new MockViewFactory_WithNonNullableState();
            INavigationManager navigationManager = CreateNavigationManager(navigationTarget: navigationTarget, viewFactory: viewFactory);

            navigationManager.NavigateTo("Page 2", "Test Argument");

            MockPage navigatedPage = (MockPage)navigationTarget.NavigatedPages.Last();
            object dataContext = navigatedPage.DataContext;

            Assert.IsNotNull(dataContext);
            Assert.IsInstanceOfType(dataContext, typeof(MockViewModel<string, int>));
            Assert.AreEqual(true, ((MockViewModel<string, int>)dataContext).IsActivated);
            Assert.AreEqual(0, ((MockViewModel<string, int>)dataContext).ActivationState);
        }

        [TestMethod]
        public void NavigateTo_CallsNavigatingFromOnPreviousViewModel()
        {
            MockNavigationTarget navigationTarget = new MockNavigationTarget();
            MockViewFactory_WithNavigationAware viewFactory = new MockViewFactory_WithNavigationAware();
            INavigationManager navigationManager = CreateNavigationManager(navigationTarget, viewFactory);

            navigationManager.NavigateTo("Page 1");
            MockPage currentPage = (MockPage)navigationTarget.NavigatedPages.Last();
            MockNavigationAwareViewModel<string, int> currentViewModel = (MockNavigationAwareViewModel<string, int>)currentPage.DataContext;
            currentViewModel.NavigationEvents.Clear();

            navigationManager.NavigateTo("Page 2");

            CollectionAssert.AreEqual(new string[] { "NavigatingFrom(New)" }, currentViewModel.NavigationEvents);
        }

        [TestMethod]
        public void NavigateTo_CallsNavigatingToOnNewViewModel()
        {
            MockNavigationTarget navigationTarget = new MockNavigationTarget();
            MockViewFactory_WithNavigationAware viewFactory = new MockViewFactory_WithNavigationAware();
            INavigationManager navigationManager = CreateNavigationManager(navigationTarget, viewFactory);

            navigationManager.NavigateTo("Page 1");

            navigationManager.NavigateTo("Page 2");
            MockPage currentPage = (MockPage)navigationTarget.NavigatedPages.Last();
            MockNavigationAwareViewModel<string, int> currentViewModel = (MockNavigationAwareViewModel<string, int>)currentPage.DataContext;

            CollectionAssert.AreEqual(new string[] { "NavigatedTo(New)" }, currentViewModel.NavigationEvents);
        }

        [TestMethod]
        public void NavigateTo_ThrowsException_NoPageWithSpecifiedName()
        {
            MockNavigationTarget navigationTarget = new MockNavigationTarget();
            INavigationManager navigationManager = CreateNavigationManager(navigationTarget);

            Assert.ThrowsException<InvalidOperationException>(() => navigationManager.NavigateTo("Page X"));
        }

        [TestMethod]
        public async Task RestoreNavigationStack_ReturnsFalseIfNoPreviousNavigationStack()
        {
            IStorageManager storageManager = new MockStorageManager();

            // --- First Instance ---

            {
                MockNavigationTarget navigationTarget = new MockNavigationTarget();
                MockLifetimeManager lifetimeManager = new MockLifetimeManager();
                INavigationManager navigationManager = CreateNavigationManager(navigationTarget: navigationTarget, lifetimeManager: lifetimeManager, storageManager: storageManager);
                navigationManager.NavigationStorageType = NavigationStorageType.Local;

                bool success = await navigationManager.RestoreNavigationStack();

                Assert.AreEqual(false, success);
            }
        }

        [TestMethod]
        public async Task RestoreNavigationStack_ReturnsFalseIfNavigationStorageTypeIsNone()
        {
            IStorageManager storageManager = new MockStorageManager();

            // --- First Instance ---

            {
                MockLifetimeManager lifetimeManager = new MockLifetimeManager();
                INavigationManager navigationManager = CreateNavigationManager(lifetimeManager: lifetimeManager, storageManager: storageManager);
                navigationManager.NavigationStorageType = NavigationStorageType.None;

                // Navigate to some pages

                navigationManager.NavigateTo("Page 1");
                navigationManager.NavigateTo("Page 2");

                // Suspend the application

                lifetimeManager.Suspend();
            }

            // --- Second Instance ---

            {
                MockNavigationTarget navigationTarget = new MockNavigationTarget();
                MockLifetimeManager lifetimeManager = new MockLifetimeManager();
                INavigationManager navigationManager = CreateNavigationManager(navigationTarget: navigationTarget, lifetimeManager: lifetimeManager, storageManager: storageManager);
                navigationManager.NavigationStorageType = NavigationStorageType.None;

                bool success = await navigationManager.RestoreNavigationStack();

                Assert.AreEqual(false, success);
            }
        }

        [TestMethod]
        public async Task RestoreNavigationStack_RestoresPreviousNavigationStackViaLocalStorage()
        {
            IStorageManager storageManager = new MockStorageManager();

            // --- First Instance ---

            {
                MockLifetimeManager lifetimeManager = new MockLifetimeManager();
                INavigationManager navigationManager = CreateNavigationManager(lifetimeManager: lifetimeManager, storageManager: storageManager);
                navigationManager.NavigationStorageType = NavigationStorageType.Local;

                // Navigate to some pages

                navigationManager.NavigateTo("Page 1");
                navigationManager.NavigateTo("Page 2");

                // Suspend the application

                lifetimeManager.Suspend();
            }

            // --- Second Instance ---

            {
                MockNavigationTarget navigationTarget = new MockNavigationTarget();
                MockLifetimeManager lifetimeManager = new MockLifetimeManager();
                INavigationManager navigationManager = CreateNavigationManager(navigationTarget: navigationTarget, lifetimeManager: lifetimeManager, storageManager: storageManager);
                navigationManager.NavigationStorageType = NavigationStorageType.Local;

                // Navigate to the application entry point and step back through all pages in the navigation stack

                bool success = await navigationManager.RestoreNavigationStack();

                while (navigationManager.CanGoBack)
                    navigationManager.GoBack();

                // Assert that the current page is restored from storage

                Assert.AreEqual(true, success);
                string[] pageNames = navigationTarget.NavigatedPages.Cast<MockPage>().Select(page => page.PageName).ToArray();
                CollectionAssert.AreEqual(new string[] { "Page 2", "Page 1" }, pageNames);
            }
        }

        [TestMethod]
        public async Task RestoreNavigationStack_RestoresPreviousNavigationStackViaRoamingStorage()
        {
            IStorageManager storageManager = new MockStorageManager();

            // --- First Instance ---

            {
                MockLifetimeManager lifetimeManager = new MockLifetimeManager();
                INavigationManager navigationManager = CreateNavigationManager(lifetimeManager: lifetimeManager, storageManager: storageManager);
                navigationManager.NavigationStorageType = NavigationStorageType.Roaming;

                // Navigate to some pages

                navigationManager.NavigateTo("Page 1");
                navigationManager.NavigateTo("Page 2");

                // Suspend the application

                lifetimeManager.Suspend();
            }

            // --- Second Instance ---

            {
                MockNavigationTarget navigationTarget = new MockNavigationTarget();
                MockLifetimeManager lifetimeManager = new MockLifetimeManager();
                INavigationManager navigationManager = CreateNavigationManager(navigationTarget: navigationTarget, lifetimeManager: lifetimeManager, storageManager: storageManager);
                navigationManager.NavigationStorageType = NavigationStorageType.Roaming;

                // Navigate to the application entry point and step back through all pages in the navigation stack

                bool success = await navigationManager.RestoreNavigationStack();

                while (navigationManager.CanGoBack)
                    navigationManager.GoBack();

                // Assert that the current page is restored from storage

                Assert.AreEqual(true, success);
                string[] pageNames = navigationTarget.NavigatedPages.Cast<MockPage>().Select(page => page.PageName).ToArray();
                CollectionAssert.AreEqual(new string[] { "Page 2", "Page 1" }, pageNames);
            }
        }

        [TestMethod]
        public async Task RestoreNavigationStack_RestoresPreviousViewModelViaLocalStorage()
        {
            IStorageManager storageManager = new MockStorageManager();

            // --- First Instance ---

            {
                MockLifetimeManager lifetimeManager = new MockLifetimeManager();
                INavigationManager navigationManager = CreateNavigationManager(lifetimeManager: lifetimeManager, storageManager: storageManager);
                navigationManager.NavigationStorageType = NavigationStorageType.Local;

                // Navigate to some pages

                navigationManager.NavigateTo("Page 2", "Test Argument");

                // Suspend the application

                lifetimeManager.Suspend();
            }

            // --- Second Instance ---

            {
                MockNavigationTarget navigationTarget = new MockNavigationTarget();
                MockLifetimeManager lifetimeManager = new MockLifetimeManager();
                INavigationManager navigationManager = CreateNavigationManager(navigationTarget: navigationTarget, lifetimeManager: lifetimeManager, storageManager: storageManager);
                navigationManager.NavigationStorageType = NavigationStorageType.Local;

                // Navigate to the application entry point

                await navigationManager.RestoreNavigationStack();

                // Check that the view model has been activated with the correct argument

                MockPage navigatedPage = (MockPage)navigationTarget.NavigatedPages.Last();
                object dataContext = navigatedPage.DataContext;

                Assert.IsNotNull(dataContext);
                Assert.IsInstanceOfType(dataContext, typeof(MockViewModel<string, string>));
                Assert.AreEqual("ViewModel 2", ((MockViewModel<string, string>)dataContext).Name);
            }
        }

        [TestMethod]
        public async Task RestoreNavigationStack_RestoresStringArgumentsViaLocalStorage()
        {
            IStorageManager storageManager = new MockStorageManager();

            // --- First Instance ---

            {
                MockLifetimeManager lifetimeManager = new MockLifetimeManager();
                INavigationManager navigationManager = CreateNavigationManager(lifetimeManager: lifetimeManager, storageManager: storageManager);
                navigationManager.NavigationStorageType = NavigationStorageType.Local;

                // Navigate to some pages

                navigationManager.NavigateTo("Page 2", "Test Argument");

                // Suspend the application

                lifetimeManager.Suspend();
            }

            // --- Second Instance ---

            {
                MockNavigationTarget navigationTarget = new MockNavigationTarget();
                MockLifetimeManager lifetimeManager = new MockLifetimeManager();
                INavigationManager navigationManager = CreateNavigationManager(navigationTarget: navigationTarget, lifetimeManager: lifetimeManager, storageManager: storageManager);
                navigationManager.NavigationStorageType = NavigationStorageType.Local;

                // Navigate to the application entry point

                await navigationManager.RestoreNavigationStack();

                // Check that the view model has been activated with the correct argument

                MockPage navigatedPage = (MockPage)navigationTarget.NavigatedPages.Last();
                object dataContext = navigatedPage.DataContext;

                Assert.IsNotNull(dataContext);
                Assert.IsInstanceOfType(dataContext, typeof(MockViewModel<string, string>));
                Assert.AreEqual(true, ((MockViewModel<string, string>)dataContext).IsActivated);
                Assert.AreEqual("Test Argument", ((MockViewModel<string, string>)dataContext).ActivationArguments);
            }
        }

        [TestMethod]
        public async Task RestoreNavigationStack_RestoresComplexArgumentsViaLocalStorage()
        {
            IStorageManager storageManager = new MockStorageManager();

            // --- First Instance ---

            {
                MockLifetimeManager lifetimeManager = new MockLifetimeManager();
                IViewFactory viewFactory = new MockViewFactory_WithComplexArguments();
                INavigationManager navigationManager = CreateNavigationManager(viewFactory: viewFactory, lifetimeManager: lifetimeManager, storageManager: storageManager);
                navigationManager.NavigationStorageType = NavigationStorageType.Local;

                // Navigate to some pages

                navigationManager.NavigateTo("Page 2", new TestData { Text = "Test Text", Number = 42 });

                // Suspend the application

                lifetimeManager.Suspend();
            }

            // --- Second Instance ---

            {
                MockNavigationTarget navigationTarget = new MockNavigationTarget();
                IViewFactory viewFactory = new MockViewFactory_WithComplexArguments();
                MockLifetimeManager lifetimeManager = new MockLifetimeManager();
                INavigationManager navigationManager = CreateNavigationManager(navigationTarget: navigationTarget, viewFactory: viewFactory, lifetimeManager: lifetimeManager, storageManager: storageManager);
                navigationManager.NavigationStorageType = NavigationStorageType.Local;

                // Navigate to the application entry point

                await navigationManager.RestoreNavigationStack();

                // Check that the view model has been activated with the correct argument

                MockPage navigatedPage = (MockPage)navigationTarget.NavigatedPages.Last();
                object dataContext = navigatedPage.DataContext;

                Assert.IsNotNull(dataContext);
                Assert.IsInstanceOfType(dataContext, typeof(MockViewModel<TestData, string>));
                Assert.AreEqual(true, ((MockViewModel<TestData, string>)dataContext).IsActivated);

                object arguments = ((MockViewModel<TestData, string>)dataContext).ActivationArguments;
                Assert.IsInstanceOfType(arguments, typeof(TestData));
                Assert.AreEqual("Test Text", ((TestData)arguments).Text);
                Assert.AreEqual(42, ((TestData)arguments).Number);
            }
        }

        [TestMethod]
        public async Task RestoreNavigationStack_RestoresNullArgumentsViaLocalStorage()
        {
            IStorageManager storageManager = new MockStorageManager();

            // --- First Instance ---

            {
                MockLifetimeManager lifetimeManager = new MockLifetimeManager();
                INavigationManager navigationManager = CreateNavigationManager(lifetimeManager: lifetimeManager, storageManager: storageManager);
                navigationManager.NavigationStorageType = NavigationStorageType.Local;

                // Navigate to some pages

                navigationManager.NavigateTo("Page 2");

                // Suspend the application

                lifetimeManager.Suspend();
            }

            // --- Second Instance ---

            {
                MockNavigationTarget navigationTarget = new MockNavigationTarget();
                MockLifetimeManager lifetimeManager = new MockLifetimeManager();
                INavigationManager navigationManager = CreateNavigationManager(navigationTarget: navigationTarget, lifetimeManager: lifetimeManager, storageManager: storageManager);
                navigationManager.NavigationStorageType = NavigationStorageType.Local;

                // Navigate to the application entry point

                await navigationManager.RestoreNavigationStack();

                // Check that the view model has been activated with the correct argument

                MockPage navigatedPage = (MockPage)navigationTarget.NavigatedPages.Last();
                object dataContext = navigatedPage.DataContext;

                Assert.IsNotNull(dataContext);
                Assert.IsInstanceOfType(dataContext, typeof(MockViewModel<string, string>));
                Assert.AreEqual(true, ((MockViewModel<string, string>)dataContext).IsActivated);
                Assert.AreEqual(null, ((MockViewModel<string, string>)dataContext).ActivationArguments);
            }
        }

        [TestMethod]
        public async Task RestoreNavigationStack_RestoresStringStateViaLocalStorage_ForLastPage()
        {
            IStorageManager storageManager = new MockStorageManager();

            // --- First Instance ---

            {
                MockNavigationTarget navigationTarget = new MockNavigationTarget();
                MockLifetimeManager lifetimeManager = new MockLifetimeManager();
                INavigationManager navigationManager = CreateNavigationManager(navigationTarget: navigationTarget, lifetimeManager: lifetimeManager, storageManager: storageManager);
                navigationManager.NavigationStorageType = NavigationStorageType.Local;

                // Navigate to some pages and set some view model state

                navigationManager.NavigateTo("Page 2");

                MockPage navigatedPage = (MockPage)navigationTarget.NavigatedPages.Last();
                MockViewModel<string, string> viewModel = (MockViewModel<string, string>)navigatedPage.DataContext;
                viewModel.State = "Test State";

                // Suspend the application

                lifetimeManager.Suspend();
            }

            // --- Second Instance ---

            {
                MockNavigationTarget navigationTarget = new MockNavigationTarget();
                MockLifetimeManager lifetimeManager = new MockLifetimeManager();
                INavigationManager navigationManager = CreateNavigationManager(navigationTarget: navigationTarget, lifetimeManager: lifetimeManager, storageManager: storageManager);
                navigationManager.NavigationStorageType = NavigationStorageType.Local;

                // Navigate to the application entry point

                await navigationManager.RestoreNavigationStack();

                // Check that the view model has been activated with the correct argument

                MockPage navigatedPage = (MockPage)navigationTarget.NavigatedPages.Last();
                object dataContext = navigatedPage.DataContext;

                Assert.IsNotNull(dataContext);
                Assert.IsInstanceOfType(dataContext, typeof(MockViewModel<string, string>));
                Assert.AreEqual(true, ((MockViewModel<string, string>)dataContext).IsActivated);
                Assert.AreEqual("Test State", ((MockViewModel<string, string>)dataContext).ActivationState);
            }
        }

        [TestMethod]
        public async Task RestoreNavigationStack_RestoresComplexStateViaLocalStorage_ForLastPage()
        {
            IStorageManager storageManager = new MockStorageManager();

            // --- First Instance ---

            {
                MockNavigationTarget navigationTarget = new MockNavigationTarget();
                IViewFactory viewFactory = new MockViewFactory_WithComplexState();
                MockLifetimeManager lifetimeManager = new MockLifetimeManager();
                INavigationManager navigationManager = CreateNavigationManager(navigationTarget: navigationTarget, viewFactory: viewFactory, lifetimeManager: lifetimeManager, storageManager: storageManager);
                navigationManager.NavigationStorageType = NavigationStorageType.Local;

                // Navigate to some pages

                navigationManager.NavigateTo("Page 2");

                MockPage navigatedPage = (MockPage)navigationTarget.NavigatedPages.Last();
                MockViewModel<string, TestData> viewModel = (MockViewModel<string, TestData>)navigatedPage.DataContext;
                viewModel.State = new TestData { Text = "Test Text", Number = 42 };

                // Suspend the application

                lifetimeManager.Suspend();
            }

            // --- Second Instance ---

            {
                MockNavigationTarget navigationTarget = new MockNavigationTarget();
                IViewFactory viewFactory = new MockViewFactory_WithComplexState();
                MockLifetimeManager lifetimeManager = new MockLifetimeManager();
                INavigationManager navigationManager = CreateNavigationManager(navigationTarget: navigationTarget, viewFactory: viewFactory, lifetimeManager: lifetimeManager, storageManager: storageManager);
                navigationManager.NavigationStorageType = NavigationStorageType.Local;

                // Navigate to the application entry point

                await navigationManager.RestoreNavigationStack();

                // Check that the view model has been activated with the correct argument

                MockPage navigatedPage = (MockPage)navigationTarget.NavigatedPages.Last();
                object dataContext = navigatedPage.DataContext;

                Assert.IsNotNull(dataContext);
                Assert.IsInstanceOfType(dataContext, typeof(MockViewModel<string, TestData>));
                Assert.AreEqual(true, ((MockViewModel<string, TestData>)dataContext).IsActivated);

                object state = ((MockViewModel<string, TestData>)dataContext).ActivationState;
                Assert.IsInstanceOfType(state, typeof(TestData));
                Assert.AreEqual("Test Text", ((TestData)state).Text);
                Assert.AreEqual(42, ((TestData)state).Number);
            }
        }

        [TestMethod]
        public async Task RestoreNavigationStack_RestoresNullStateViaLocalStorage_ForLastPage()
        {
            IStorageManager storageManager = new MockStorageManager();

            // --- First Instance ---

            {
                MockLifetimeManager lifetimeManager = new MockLifetimeManager();
                INavigationManager navigationManager = CreateNavigationManager(lifetimeManager: lifetimeManager, storageManager: storageManager);
                navigationManager.NavigationStorageType = NavigationStorageType.Local;

                // Navigate to some pages

                navigationManager.NavigateTo("Page 2");

                // Suspend the application

                lifetimeManager.Suspend();
            }

            // --- Second Instance ---

            {
                MockNavigationTarget navigationTarget = new MockNavigationTarget();
                MockLifetimeManager lifetimeManager = new MockLifetimeManager();
                INavigationManager navigationManager = CreateNavigationManager(navigationTarget: navigationTarget, lifetimeManager: lifetimeManager, storageManager: storageManager);
                navigationManager.NavigationStorageType = NavigationStorageType.Local;

                // Navigate to the application entry point

                await navigationManager.RestoreNavigationStack();

                // Check that the view model has been activated with the correct argument

                MockPage navigatedPage = (MockPage)navigationTarget.NavigatedPages.Last();
                object dataContext = navigatedPage.DataContext;

                Assert.IsNotNull(dataContext);
                Assert.IsInstanceOfType(dataContext, typeof(MockViewModel<string, string>));
                Assert.AreEqual(true, ((MockViewModel<string, string>)dataContext).IsActivated);
                Assert.AreEqual(null, ((MockViewModel<string, string>)dataContext).ActivationState);
            }
        }

        [TestMethod]
        public async Task RestoreNavigationStack_RestoresStringStateViaLocalStorage_ForPreviousPage()
        {
            IStorageManager storageManager = new MockStorageManager();

            // --- First Instance ---

            {
                MockNavigationTarget navigationTarget = new MockNavigationTarget();
                MockLifetimeManager lifetimeManager = new MockLifetimeManager();
                INavigationManager navigationManager = CreateNavigationManager(navigationTarget: navigationTarget, lifetimeManager: lifetimeManager, storageManager: storageManager);
                navigationManager.NavigationStorageType = NavigationStorageType.Local;

                // Navigate to some pages and set some view model state

                navigationManager.NavigateTo("Page 2");

                MockPage navigatedPage = (MockPage)navigationTarget.NavigatedPages.Last();
                MockViewModel<string, string> viewModel = (MockViewModel<string, string>)navigatedPage.DataContext;
                viewModel.State = "Test State";

                navigationManager.NavigateTo("Page 1");

                // Suspend the application

                lifetimeManager.Suspend();
            }

            // --- Second Instance ---

            {
                MockNavigationTarget navigationTarget = new MockNavigationTarget();
                MockLifetimeManager lifetimeManager = new MockLifetimeManager();
                INavigationManager navigationManager = CreateNavigationManager(navigationTarget: navigationTarget, lifetimeManager: lifetimeManager, storageManager: storageManager);
                navigationManager.NavigationStorageType = NavigationStorageType.Local;

                // Navigate to the application entry point and navigate back one page

                await navigationManager.RestoreNavigationStack();
                navigationManager.GoBack();

                // Check that the view model has been activated with the correct argument

                MockPage navigatedPage = (MockPage)navigationTarget.NavigatedPages.Last();
                object dataContext = navigatedPage.DataContext;

                Assert.IsNotNull(dataContext);
                Assert.IsInstanceOfType(dataContext, typeof(MockViewModel<string, string>));
                Assert.AreEqual(true, ((MockViewModel<string, string>)dataContext).IsActivated);
                Assert.AreEqual("Test State", ((MockViewModel<string, string>)dataContext).ActivationState);
            }
        }

        [TestMethod]
        public async Task RestoreNavigationStack_RestoresComplexStateViaLocalStorage_ForPreviousPage()
        {
            IStorageManager storageManager = new MockStorageManager();

            // --- First Instance ---

            {
                MockNavigationTarget navigationTarget = new MockNavigationTarget();
                IViewFactory viewFactory = new MockViewFactory_WithComplexState();
                MockLifetimeManager lifetimeManager = new MockLifetimeManager();
                INavigationManager navigationManager = CreateNavigationManager(navigationTarget: navigationTarget, viewFactory: viewFactory, lifetimeManager: lifetimeManager, storageManager: storageManager);
                navigationManager.NavigationStorageType = NavigationStorageType.Local;

                // Navigate to some pages

                navigationManager.NavigateTo("Page 2");

                MockPage navigatedPage = (MockPage)navigationTarget.NavigatedPages.Last();
                MockViewModel<string, TestData> viewModel = (MockViewModel<string, TestData>)navigatedPage.DataContext;
                viewModel.State = new TestData { Text = "Test Text", Number = 42 };

                navigationManager.NavigateTo("Page 1");

                // Suspend the application

                lifetimeManager.Suspend();
            }

            // --- Second Instance ---

            {
                MockNavigationTarget navigationTarget = new MockNavigationTarget();
                IViewFactory viewFactory = new MockViewFactory_WithComplexState();
                MockLifetimeManager lifetimeManager = new MockLifetimeManager();
                INavigationManager navigationManager = CreateNavigationManager(navigationTarget: navigationTarget, viewFactory: viewFactory, lifetimeManager: lifetimeManager, storageManager: storageManager);
                navigationManager.NavigationStorageType = NavigationStorageType.Local;

                // Navigate to the application entry point

                await navigationManager.RestoreNavigationStack();
                navigationManager.GoBack();

                // Check that the view model has been activated with the correct argument

                MockPage navigatedPage = (MockPage)navigationTarget.NavigatedPages.Last();
                object dataContext = navigatedPage.DataContext;

                Assert.IsNotNull(dataContext);
                Assert.IsInstanceOfType(dataContext, typeof(MockViewModel<string, TestData>));
                Assert.AreEqual(true, ((MockViewModel<string, TestData>)dataContext).IsActivated);

                object state = ((MockViewModel<string, TestData>)dataContext).ActivationState;
                Assert.IsInstanceOfType(state, typeof(TestData));
                Assert.AreEqual("Test Text", ((TestData)state).Text);
                Assert.AreEqual(42, ((TestData)state).Number);
            }
        }

        [TestMethod]
        public async Task RestoreNavigationStack_RestoresNullStateViaLocalStorage_ForPreviousPage()
        {
            IStorageManager storageManager = new MockStorageManager();

            // --- First Instance ---

            {
                MockLifetimeManager lifetimeManager = new MockLifetimeManager();
                INavigationManager navigationManager = CreateNavigationManager(lifetimeManager: lifetimeManager, storageManager: storageManager);
                navigationManager.NavigationStorageType = NavigationStorageType.Local;

                // Navigate to some pages

                navigationManager.NavigateTo("Page 2");

                navigationManager.NavigateTo("Page 1");

                // Suspend the application

                lifetimeManager.Suspend();
            }

            // --- Second Instance ---

            {
                MockNavigationTarget navigationTarget = new MockNavigationTarget();
                MockLifetimeManager lifetimeManager = new MockLifetimeManager();
                INavigationManager navigationManager = CreateNavigationManager(navigationTarget: navigationTarget, lifetimeManager: lifetimeManager, storageManager: storageManager);
                navigationManager.NavigationStorageType = NavigationStorageType.Local;

                // Navigate to the application entry point

                await navigationManager.RestoreNavigationStack();
                navigationManager.GoBack();

                // Check that the view model has been activated with the correct argument

                MockPage navigatedPage = (MockPage)navigationTarget.NavigatedPages.Last();
                object dataContext = navigatedPage.DataContext;

                Assert.IsNotNull(dataContext);
                Assert.IsInstanceOfType(dataContext, typeof(MockViewModel<string, string>));
                Assert.AreEqual(true, ((MockViewModel<string, string>)dataContext).IsActivated);
                Assert.AreEqual(null, ((MockViewModel<string, string>)dataContext).ActivationState);
            }
        }

        [TestMethod]
        public async Task RestoreNavigationStack_CallsNavigatingToOnRestoredViewModel()
        {
            IStorageManager storageManager = new MockStorageManager();

            // --- First Instance ---

            {
                MockLifetimeManager lifetimeManager = new MockLifetimeManager();
                INavigationManager navigationManager = CreateNavigationManager(lifetimeManager: lifetimeManager, storageManager: storageManager);
                navigationManager.NavigationStorageType = NavigationStorageType.Local;

                // Navigate to some pages

                navigationManager.NavigateTo("Page 2", "Test Argument");

                // Suspend the application

                lifetimeManager.Suspend();
            }

            // --- Second Instance ---

            {
                MockNavigationTarget navigationTarget = new MockNavigationTarget();
                MockViewFactory_WithNavigationAware viewFactory = new MockViewFactory_WithNavigationAware();
                MockLifetimeManager lifetimeManager = new MockLifetimeManager();
                INavigationManager navigationManager = CreateNavigationManager(navigationTarget: navigationTarget, viewFactory: viewFactory, lifetimeManager: lifetimeManager, storageManager: storageManager);
                navigationManager.NavigationStorageType = NavigationStorageType.Local;

                // Navigate to the application entry point

                await navigationManager.RestoreNavigationStack();

                // Check that the NavigatingTo() method has been called

                MockPage currentPage = (MockPage)navigationTarget.NavigatedPages.Last();
                MockNavigationAwareViewModel<string, int> currentViewModel = (MockNavigationAwareViewModel<string, int>)currentPage.DataContext;

                CollectionAssert.AreEqual(new string[] { "NavigatedTo(Refresh)" }, currentViewModel.NavigationEvents);
            }
        }

        // *** Behavior Tests ***

        [TestMethod]
        public void Constructor_RegistersWithLifetimeManager()
        {
            MockLifetimeManager lifetimeManager = new MockLifetimeManager();
            INavigationManager navigationManager = CreateNavigationManager(lifetimeManager: lifetimeManager);

            CollectionAssert.Contains(lifetimeManager.RegisteredServices, navigationManager);
        }

        // *** Private Methods ***

        private NavigationManager CreateNavigationManager(INavigationTarget navigationTarget = null, IViewFactory viewFactory = null, ILifetimeManager lifetimeManager = null, IStorageManager storageManager = null, bool navigationTargetIsNull = false, bool setHomePageName = true)
        {
            if (navigationTarget == null && !navigationTargetIsNull)
                navigationTarget = new MockNavigationTarget();

            if (viewFactory == null)
                viewFactory = new MockViewFactory();

            if (lifetimeManager == null)
                lifetimeManager = new MockLifetimeManager();

            if (storageManager == null)
                storageManager = new MockStorageManager();

            NavigationManager navigationManager = new NavigationManager(navigationTarget, viewFactory, lifetimeManager, storageManager);

            if (setHomePageName)
                navigationManager.HomePageName = "Page 1";

            return navigationManager;
        }

        // *** Private Sub-classes ***

        private class MockViewFactory : IViewFactory
        {
            // *** Methods ***

            public IViewLifetimeContext CreateView(string name)
            {
                switch (name)
                {
                    case "Page 1":
                        return new MockViewLifetimeContext<string, string>("Page 1", "ViewModel 1");
                    case "Page 2":
                        return new MockViewLifetimeContext<string, string>("Page 2", "ViewModel 2");
                    case "Page 3":
                        return new MockViewLifetimeContext<string, string>("Page 3", null);
                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        private class MockViewFactory_WithComplexArguments : IViewFactory
        {
            // *** Methods ***

            public IViewLifetimeContext CreateView(string name)
            {
                switch (name)
                {
                    case "Page 1":
                        return new MockViewLifetimeContext<TestData, string>("Page 1", "ViewModel 1");
                    case "Page 2":
                        return new MockViewLifetimeContext<TestData, string>("Page 2", "ViewModel 2");
                    case "Page 3":
                        return new MockViewLifetimeContext<TestData, string>("Page 3", null);
                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        private class MockViewFactory_WithComplexState : IViewFactory
        {
            // *** Methods ***

            public IViewLifetimeContext CreateView(string name)
            {
                switch (name)
                {
                    case "Page 1":
                        return new MockViewLifetimeContext<string, TestData>("Page 1", "ViewModel 1");
                    case "Page 2":
                        return new MockViewLifetimeContext<string, TestData>("Page 2", "ViewModel 2");
                    case "Page 3":
                        return new MockViewLifetimeContext<string, TestData>("Page 3", null);
                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        private class MockViewFactory_WithNonNullableState : IViewFactory
        {
            // *** Methods ***

            public IViewLifetimeContext CreateView(string name)
            {
                switch (name)
                {
                    case "Page 1":
                        return new MockViewLifetimeContext<string, int>("Page 1", "ViewModel 1");
                    case "Page 2":
                        return new MockViewLifetimeContext<string, int>("Page 2", "ViewModel 2");
                    case "Page 3":
                        return new MockViewLifetimeContext<string, int>("Page 3", null);
                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        private class MockViewFactory_WithNavigationAware : IViewFactory
        {
            // *** Methods ***

            public IViewLifetimeContext CreateView(string name)
            {
                switch (name)
                {
                    case "Page 1":
                        return new MockViewLifetimeContext<string, int>("Page 1", "ViewModel 1", viewModelType: typeof(MockNavigationAwareViewModel<string, int>));
                    case "Page 2":
                        return new MockViewLifetimeContext<string, int>("Page 2", "ViewModel 2", viewModelType: typeof(MockNavigationAwareViewModel<string, int>));
                    case "Page 3":
                        return new MockViewLifetimeContext<string, int>("Page 3", null, viewModelType: typeof(MockNavigationAwareViewModel<string, int>));
                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        private class MockViewLifetimeContext<TArguments, TState> : IViewLifetimeContext
        {
            // *** Constructors ***

            public MockViewLifetimeContext(string pageName, string viewModelName, Type pageType = null, Type viewModelType = null)
            {
                if (pageName != null)
                {
                    MockPage page = (MockPage)Activator.CreateInstance(pageType ?? typeof(MockPage));
                    page.PageName = pageName;

                    View = page;
                }

                if (viewModelName != null)
                {
                    MockViewModel<TArguments, TState> viewModel = (MockViewModel<TArguments, TState>)Activator.CreateInstance(viewModelType ?? typeof(MockViewModel<TArguments, TState>));
                    viewModel.Name = viewModelName;

                    ViewModel = viewModel;
                }

                if (pageName != null && viewModelName != null)
                    ((MockPage)View).DataContext = ViewModel;
            }

            // *** Properties ***

            public object View { get; set; }
            public object ViewModel { get; set; }

            // *** Methods ***

            public void Dispose()
            {
                ((MockPage)View).IsDisposed = true;
                ((MockViewModel<TArguments, TState>)ViewModel).IsDisposed = true;
            }
        }

        private class MockNavigationTarget : INavigationTarget
        {
            // *** Fields ***

            public List<object> NavigatedPages = new List<object>();

            // *** Methods ***

            public void NavigateTo(object page)
            {
                NavigatedPages.Add(page);
            }
        }

        private class MockLifetimeManager : ILifetimeManager
        {
            // *** Fields ***

            public List<ILifetimeAware> RegisteredServices = new List<ILifetimeAware>();

            // *** Methods ***

            public void Register(ILifetimeAware service)
            {
                RegisteredServices.Add(service);
            }

            public void Unregister(ILifetimeAware service)
            {
                RegisteredServices.Remove(service);
            }

            // *** Mock Methods ***

            public void Suspend()
            {
                foreach (ILifetimeAware service in RegisteredServices)
                    service.OnSuspending().Wait();
            }

            public void Resume()
            {
                foreach (ILifetimeAware service in RegisteredServices)
                    service.OnResuming().Wait();
            }

            public void Exit()
            {
                foreach (ILifetimeAware service in RegisteredServices)
                    service.OnExiting().Wait();
            }
        }

        private class MockStorageManager : IStorageManager
        {
            // *** Fields ***

            private Dictionary<string, byte[]> storageDictionary = new Dictionary<string, byte[]>();

            // *** IStorageManager Methods ***

            public async Task<T> RetrieveAsync<T>(StorageFile file)
            {
                await Task.Yield();

                if (storageDictionary.ContainsKey(file.Path))
                    return SerializationHelper.DeserializeFromArray<T>(storageDictionary[file.Path]);
                else
                    return default(T);
            }

            public async Task<T> RetrieveAsync<T>(StorageFolder folder, string name)
            {
                await Task.Yield();

                if (storageDictionary.ContainsKey(folder.Path + @"\" + name))
                    return SerializationHelper.DeserializeFromArray<T>(storageDictionary[folder.Path + @"\" + name]);
                else
                    return default(T);
            }

            public async Task StoreAsync<T>(StorageFile file, T value)
            {
                await Task.Yield();
                byte[] data = SerializationHelper.SerializeToArray(value);
                storageDictionary[file.Path] = data;
            }

            public async Task StoreAsync<T>(StorageFolder folder, string name, T value)
            {
                await Task.Yield();
                byte[] data = SerializationHelper.SerializeToArray(value);
                storageDictionary[folder.Path + @"\" + name] = data;
            }
        }

        private class MockPage
        {
            // *** Properties ***

            public bool IsDisposed { get; set; }
            public string PageName { get; set; }
            public object DataContext { get; set; }
        }

        private class MockViewModel<TArguments, TState> : IActivatable<TArguments, TState>
        {
            // *** Properties ***

            public TArguments ActivationArguments { get; private set; }
            public TState ActivationState { get; private set; }
            public bool IsActivated { get; private set; }
            public bool IsDisposed { get; set; }
            public string Name { get; set; }
            public TState State { get; set; }

            // *** Methods ***

            public void Activate(TArguments arguments, TState state)
            {
                IsActivated = true;
                ActivationArguments = arguments;
                ActivationState = state;
            }

            public TState SaveState()
            {
                return State;
            }
        }

        private class MockNavigationAwareViewModel<TArguments, TState> : MockViewModel<TArguments, TState>, INavigationAware
        {
            // *** Fields ***

            public List<string> NavigationEvents = new List<string>();

            // *** Methods ***

            public void NavigatedTo(NavigationMode navigationMode)
            {
                NavigationEvents.Add(string.Format("NavigatedTo({0})", navigationMode));
            }

            public void NavigatingFrom(NavigationMode navigationMode)
            {
                NavigationEvents.Add(string.Format("NavigatingFrom({0})", navigationMode));
            }
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