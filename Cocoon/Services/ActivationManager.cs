﻿using System.Threading.Tasks;
using Cocoon.Navigation;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

namespace Cocoon.Services
{
    public class ActivationManager : IActivationManager
    {
        // *** Fields ***

        private INavigationManager navigationManager;

        // *** Constructors ***

        public ActivationManager(INavigationManager navigationManager)
        {
            this.navigationManager = navigationManager;
        }

        // *** Methods ***

        public Task<bool> Activate(IActivatedEventArgs activatedEventArgs)
        {
            switch (activatedEventArgs.Kind)
            {
                case ActivationKind.Launch:
                    return Activate_Launch((ILaunchActivatedEventArgs)activatedEventArgs);
                default:
                    return Task.FromResult(false);
            }
        }

        public Task<bool> Activate(IActivatedEventArgs activatedEventArgs, string page)
        {
            switch (activatedEventArgs.Kind)
            {
                case ActivationKind.Launch:
                    return Activate_Launch(activatedEventArgs);
                default:
                    return Task.FromResult(false);
            }   
        }

        // *** Private Methods ***

        private async Task<bool> Activate_Launch(IActivatedEventArgs e)
        {
            // Unless the application was closed (ClosedByUser) or previously crashed (NotRunning) then attempt to restore the navigation stack

            if (e.PreviousExecutionState != ApplicationExecutionState.ClosedByUser && e.PreviousExecutionState != ApplicationExecutionState.NotRunning)
            {
                bool success = await navigationManager.RestoreNavigationStack();

                if (success)
                {
                    if (Window.Current != null)
                        Window.Current.Activate();

                    return true;
                }
            }

            // If we cannot restore the navigation stack for any reason, navigate to the home page

            navigationManager.NavigateTo(navigationManager.HomePageName);

            if (Window.Current != null)
                Window.Current.Activate();

            return true;
        }
    }
}
