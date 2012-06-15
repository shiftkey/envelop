using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Cocoon.Navigation;
using Windows.ApplicationModel.Activation;
using System.Threading.Tasks;
using System.Composition.Hosting;
using System.Composition;
using System.Collections;
using Windows.ApplicationModel.Core;
using Cocoon.Services;

namespace Cocoon.MEF.Tests
{
    [TestClass]
    public class CocoonBootstrapperFixture
    {
        // *** Method Tests ***

        [TestMethod]
        public void Initialize_ComposesProperties()
        {
            TestableBootstrapper bootstrapper = new TestableBootstrapper();

            bootstrapper.Initialize();

            Assert.IsInstanceOfType(bootstrapper.ActivationManager, typeof(MockActivationManager));
        }

        [TestMethod]
        public void OnActivated_CallsSetupServices()
        {
            TestableBootstrapper bootstrapper = new TestableBootstrapper();
            bootstrapper.Initialize();

            bootstrapper.Activate(new MockActivatedEventArgs(ActivationKind.Launch));

            CollectionAssert.Contains((ICollection)bootstrapper.SetupMethodCalls, "SetupServices");
        }

        [TestMethod]
        public void OnActivated_CallsSetupServices_OnlyOnce()
        {
            TestableBootstrapper bootstrapper = new TestableBootstrapper();
            bootstrapper.Initialize();

            bootstrapper.Activate(new MockActivatedEventArgs(ActivationKind.Launch));
            bootstrapper.Activate(new MockActivatedEventArgs(ActivationKind.Launch));
            bootstrapper.Activate(new MockActivatedEventArgs(ActivationKind.Launch));

            Assert.AreEqual(1, bootstrapper.SetupMethodCalls.Count(str => str == "SetupServices"));
        }

        [TestMethod]
        public void OnActivated_CallsSetupNavigationManager()
        {
            TestableBootstrapper bootstrapper = new TestableBootstrapper();
            bootstrapper.Initialize();

            bootstrapper.Activate(new MockActivatedEventArgs(ActivationKind.Launch));

            CollectionAssert.Contains((ICollection)bootstrapper.SetupMethodCalls, "SetupNavigationManager");
        }

        [TestMethod]
        public void OnActivated_CallsSetupNavigationManager_OnlyOnce()
        {
            TestableBootstrapper bootstrapper = new TestableBootstrapper();
            bootstrapper.Initialize();

            bootstrapper.Activate(new MockActivatedEventArgs(ActivationKind.Launch));
            bootstrapper.Activate(new MockActivatedEventArgs(ActivationKind.Launch));
            bootstrapper.Activate(new MockActivatedEventArgs(ActivationKind.Launch));

            Assert.AreEqual(1, bootstrapper.SetupMethodCalls.Count(str => str == "SetupNavigationManager"));
        }

        [TestMethod]
        public void OnActivated_Launch_PassesActivationEventToNavigationManager()
        {
            TestableBootstrapper bootstrapper = new TestableBootstrapper();
            bootstrapper.Initialize();

            bootstrapper.Activate(new MockActivatedEventArgs(ActivationKind.Launch));

            MockActivationManager activationManager = (MockActivationManager)bootstrapper.ActivationManager;
            Assert.AreEqual(1, activationManager.ActivationEventArgs.Count);
            Assert.IsInstanceOfType(activationManager.ActivationEventArgs[0], typeof(MockActivatedEventArgs));
            Assert.AreEqual(ActivationKind.Launch, activationManager.ActivationEventArgs[0].Kind);
        }

        // *** Private Sub-classes ***

        private class TestableBootstrapper : CocoonBootstrapper
        {
            // *** Fields ***

            public readonly IList<string> SetupMethodCalls = new List<string>();

            // *** Methods ***

            public void Activate(IActivatedEventArgs activatedEventArgs)
            {
                base.OnActivated(CoreApplication.MainView, activatedEventArgs);
            }

            // *** Overriden base methods ***

            public override void Initialize()
            {
                base.Initialize(false);
            }

            protected override ContainerConfiguration GetContainerConfiguration()
            {
                return new ContainerConfiguration()
                            .WithPart<MockActivationManager>()
                            .WithPart<MockNavigationManager>();
            }

            protected override void SetupNavigationManager()
            {
                SetupMethodCalls.Add("SetupNavigationManager");
                base.SetupNavigationManager();
            }

            protected override void SetupServices()
            {
                SetupMethodCalls.Add("SetupServices");
                base.SetupServices();
            }
        }

        [Export(typeof(IActivationManager))]
        [Shared]
        private class MockActivationManager : IActivationManager
        {
            // *** Fields ***

            public readonly IList<IActivatedEventArgs> ActivationEventArgs = new List<IActivatedEventArgs>();

            // *** Methods ***

            public Task<bool> Activate(IActivatedEventArgs activatedEventArgs)
            {
                ActivationEventArgs.Add(activatedEventArgs);
                return Task.FromResult<bool>(true);
            }
        }

        [Export(typeof(INavigationManager))]
        [Shared]
        private class MockNavigationManager : INavigationManager
        {
            public bool CanGoBack
            {
                get { throw new NotImplementedException(); }
            }

            public string HomePageName
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

            public void GoBack()
            {
                throw new NotImplementedException();
            }

            public void NavigateTo(string pageName)
            {
                throw new NotImplementedException();
            }

            public void NavigateTo(string pageName, object arguments)
            {
                throw new NotImplementedException();
            }

            public Task<bool> RestoreNavigationStack()
            {
                throw new NotImplementedException();
            }
        }

        private class MockActivatedEventArgs : IActivatedEventArgs
        {
            // *** Constructors ***

            public MockActivatedEventArgs(ActivationKind kind)
            {
                this.Kind = kind;
            }

            // *** Properties ***

            public ActivationKind Kind { get; set; }

            public ApplicationExecutionState PreviousExecutionState { get; set; }

            public SplashScreen SplashScreen { get; set; }
        }
    }
}