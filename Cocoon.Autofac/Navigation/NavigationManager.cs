using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Cocoon.Helpers;
using Cocoon.Services;
using Windows.Storage;
using Windows.UI.Xaml.Navigation;

namespace Cocoon.Navigation
{
    public class NavigationManager : INavigationManager, ILifetimeAware
    {
        private const string StorageFilename = "Cocoon_Navigation_NavigationManager.xml";

        private readonly IViewFactory viewFactory;
        private readonly IStorageManager storageManager;

        private readonly Stack<NavigationEntry> navigationStack = new Stack<NavigationEntry>();

        private string homePageName = SpecialPageNames.Home;

        public NavigationManager(INavigationTarget navigationTarget, IViewFactory viewFactory, ILifetimeManager lifetimeManager, IStorageManager storageManager)
        {
            NavigationStorageType = NavigationStorageType.None;
            this.viewFactory = viewFactory;
            this.storageManager = storageManager;

            // NOTE: ensure MEF passes in a default INavigationTarget

            NavigationTarget = navigationTarget;
            
            lifetimeManager.Register(this);
        }


        public bool CanGoBack
        {
            get { return navigationStack.Count > 1; }
        }

        public string HomePageName
        {
            get { return homePageName; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException(ResourceHelper.GetErrorResource("Exception_ArgumentException_StringIsNullOrEmpty"), "HomePageName");

                homePageName = value;
            }
        }

        public NavigationStorageType NavigationStorageType { get; set; }

        public INavigationTarget NavigationTarget { get; private set; }

        private NavigationEntry CurrentNavigationEntry
        {
            get
            {
                return navigationStack.Count > 0 ? navigationStack.Peek() : null;
            }
        }

        public void GoBack()
        {
            if (!CanGoBack)
                throw new InvalidOperationException(ResourceHelper.GetErrorResource("Exception_InvalidOperation_CannotGoBackWithEmptyBackStack"));

            var oldNavigationEntry = navigationStack.Pop();
            CallNavigatingFrom(oldNavigationEntry, NavigationMode.Back);
            oldNavigationEntry.DisposeCachedItems();

            DisplayPage(CurrentNavigationEntry);

            CallNavigatedTo(CurrentNavigationEntry, NavigationMode.Back);
        }

        public void NavigateTo(string pageName)
        {
            NavigateTo(pageName, null);
        }

        public void NavigateTo(string pageName, object arguments)
        {
            CallNavigatingFrom(CurrentNavigationEntry, NavigationMode.New);

            var navigationEntry = new NavigationEntry(pageName, arguments);
            navigationStack.Push(navigationEntry);

            DisplayPage(navigationEntry);

            CallNavigatedTo(navigationEntry, NavigationMode.New);
        }

        public async Task<bool> RestoreNavigationStack()
        {
            // Retrieve a navigation stack from storage unless,
            //    (1) The NavigationStorageType is 'None'
            //    (2) Cannot find the navigation stack in storage

            NavigationManagerState restoredState = null;

            switch (NavigationStorageType)
            {
                case NavigationStorageType.Local:
                    restoredState = await storageManager.RetrieveAsync<NavigationManagerState>(ApplicationData.Current.LocalFolder, StorageFilename);
                    break;
                case NavigationStorageType.Roaming:
                    restoredState = await storageManager.RetrieveAsync<NavigationManagerState>(ApplicationData.Current.RoamingFolder, StorageFilename);
                    break;
            }

            if (restoredState != null)
            {
                foreach (var entryState in restoredState.NavigationStack.Reverse())
                {
        
                    navigationStack.Push(new NavigationEntry(entryState.PageName, entryState.ArgumentsData, entryState.StateData));
                }

                DisplayPage(CurrentNavigationEntry);

                CallNavigatedTo(CurrentNavigationEntry, NavigationMode.Refresh);

                return true;
            }

            return false;
        }

        public Task OnExiting()
        {
            return Task.FromResult(true);
        }

        public Task OnResuming()
        {
            return Task.FromResult(true);
        }

        public async Task OnSuspending()
        {
            switch (NavigationStorageType)
            {
                case NavigationStorageType.Local:
                    await StoreNavigationStack(ApplicationData.Current.LocalFolder);
                    break;
                case NavigationStorageType.Roaming:
                    await StoreNavigationStack(ApplicationData.Current.RoamingFolder);
                    break;
            }
        }

        private static void CallNavigatedTo(NavigationEntry entry, NavigationMode navigationMode)
        {
            if (entry == null)
                return;

            var viewModel = entry.ViewLifetimeContext.ViewModel;

            var awareViewModel = viewModel as INavigationAware;
            if (awareViewModel != null)
                awareViewModel.NavigatedTo(navigationMode);
        }

        private static void CallNavigatingFrom(NavigationEntry entry, NavigationMode navigationMode)
        {
            if (entry == null)
                return;

            var viewModel = entry.ViewLifetimeContext.ViewModel;

            var awareViewModel = viewModel as INavigationAware;
            if (awareViewModel != null)
                awareViewModel.NavigatingFrom(navigationMode);
        }

        private void CreatePage(NavigationEntry entry)
        {
            var viewLifetimeContext = viewFactory.CreateView(entry.PageName);
            entry.ViewLifetimeContext = viewLifetimeContext;

            // Activate the view model if it implements IActivatable<,>

            var viewModel = entry.ViewLifetimeContext.ViewModel;
            var activatableInterface = ReflectionHelper.GetClosedGenericType(viewModel, typeof(IActivatable<,>));

            if (activatableInterface != null)
            {
                entry.DeserializeData(activatableInterface.GenericTypeArguments[0], activatableInterface.GenericTypeArguments[1]);

                var activateMethod = activatableInterface.GetTypeInfo().GetDeclaredMethod("Activate");
                activateMethod.Invoke(viewModel, new[] { entry.Arguments, entry.State });
            }
        }

        private void DisplayPage(NavigationEntry entry)
        {
            if (entry.ViewLifetimeContext == null)
                CreatePage(entry);

            NavigationTarget.NavigateTo(entry.ViewLifetimeContext.View);
        }

        private static void SavePageState(NavigationEntry entry)
        {
            // If the view model is IActivatable<,> then use this to save the page state
            // NB: First check that the view has been created - this may still have state from a previous instance
            // NB: Use reflection as we do not know the generic parameter types

            if (entry.ViewLifetimeContext != null)
            {
                // Get the generic IActivatable<,> interface

                var viewModel = entry.ViewLifetimeContext.ViewModel;
                var activatableInterface = ReflectionHelper.GetClosedGenericType(viewModel, typeof(IActivatable<,>));

                if (activatableInterface != null)
                {
                    var saveStateMethod = activatableInterface.GetTypeInfo().GetDeclaredMethod("SaveState");
                    entry.State = saveStateMethod.Invoke(viewModel, null);

                    entry.SerializeData(activatableInterface.GenericTypeArguments[0], activatableInterface.GenericTypeArguments[1]);
                }
            }
        }

        private Task StoreNavigationStack(StorageFolder folder)
        {
            var state = new NavigationManagerState();

            foreach (var entry in navigationStack)
            {
                // TODO : Do this when navigating away from each page to save time when suspending

                SavePageState(entry);

                var entryState = new NavigationEntryState
                {
                    PageName = entry.PageName,
                    ArgumentsData = entry.ArgumentsData,
                    StateData = entry.StateData
                };

                state.NavigationStack.Add(entryState);
            }

            return storageManager.StoreAsync(folder, StorageFilename, state);
        }

        [DataContract]
        private class NavigationManagerState
        {
            public NavigationManagerState()
            {
                NavigationStack = new List<NavigationEntryState>();
            }

            [DataMember]
            public IList<NavigationEntryState> NavigationStack { get; private set; }
        }

        [DataContract]
        private class NavigationEntryState
        {
            [DataMember]
            public string PageName { get; set; }

            [DataMember]
            public byte[] ArgumentsData { get; set; }

            [DataMember]
            public byte[] StateData { get; set; }
        }
    }
}
