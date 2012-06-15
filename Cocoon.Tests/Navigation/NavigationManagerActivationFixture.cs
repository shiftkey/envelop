using Cocoon.Navigation;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Cocoon.Tests.Navigation
{
    [TestClass]
    public class NavigationManagerActivationFixture
    {
        private INavigationTarget navigationTarget;
        private IViewFactory viewFactory;
        private ILifetimeManager lifetimeManager;
        public IStorageManager storageManager;
        [TestMethod]
        public void Monkey()
        {
            var navigationManager = new NavigationManager(navigationTarget, viewFactory, lifetimeManager, storageManager);

            navigationManager.NavigateTo(
        }

        public class MockNavigationManager : NavigationManager
        {
            
        }

        

        private class MockViewModelWithoutState<TArguments> : IActivatable<TArguments>
        {
            public TArguments ActivationArguments { get; private set; }
            public bool IsActivated { get; private set; }
            public bool IsDisposed { get; set; }
            public string Name { get; set; }

            public void Activate(TArguments arguments)
            {
                IsActivated = true;
                ActivationArguments = arguments;
            }
        }

        private class MockViewModel<TArguments, TState> : IActivatable<TArguments, TState>
        {
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

    }
}
