using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Cocoon.Helpers;
using Cocoon.Services;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI.Xaml;
using System.Reflection;
using Windows.UI.Xaml.Navigation;

namespace Cocoon.Navigation
{
    public class NavigationManager : INavigationManager, ILifetimeAware
    {
        // *** Constants ***

        private const string STORAGE_FILENAME = "Cocoon_Navigation_NavigationManager.xml";

        // *** Fields ***

        private readonly INavigationTarget navigationTarget;
        private readonly IViewFactory viewFactory;
        private readonly IStorageManager storageManager;

        private readonly Stack<NavigationEntry> navigationStack = new Stack<NavigationEntry>();

        private string homePageName = SpecialPageNames.HomePage;
        private NavigationStorageType navigationStorageType = NavigationStorageType.None;

        // *** Constructors ***

        public NavigationManager(INavigationTarget navigationTarget, IViewFactory viewFactory, ILifetimeManager lifetimeManager, IStorageManager storageManager)
        {
            this.viewFactory = viewFactory;
            this.storageManager = storageManager;

            // Use a default INavigationTarget if not specified

            if (navigationTarget != null)
                this.navigationTarget = navigationTarget;
            else
                this.navigationTarget = new WindowNavigationTarget();

            // Register with the LifetimeManager

            lifetimeManager.Register(this);
        }

        // *** Properties ***

        public bool CanGoBack
        {
            get
            {
                return navigationStack.Count > 1;
            }
        }

        public string HomePageName
        {
            get
            {
                return homePageName;
            }
            set
            {
                // Validate parameters

                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException(ResourceHelper.GetErrorResource("Exception_ArgumentException_StringIsNullOrEmpty"), "HomePageName");

                // Set the property

                homePageName = value;
            }
        }

        public NavigationStorageType NavigationStorageType
        {
            get
            {
                return navigationStorageType;
            }
            set
            {
                navigationStorageType = value;
            }
        }

        public INavigationTarget NavigationTarget
        {
            get
            {
                return navigationTarget;
            }
        }

        // *** Private Properties ***

        private NavigationEntry CurrentNavigationEntry
        {
            get
            {
                if (navigationStack.Count > 0)
                    return navigationStack.Peek();
                else
                    return null;
            }
        }

        // *** Methods ***

        public void GoBack()
        {
            // Check that we can go back

            if (!CanGoBack)
                throw new InvalidOperationException(ResourceHelper.GetErrorResource("Exception_InvalidOperation_CannotGoBackWithEmptyBackStack"));

            // Pop the last page from the stack, call NavigationFrom() and dispose any cached items

            NavigationEntry oldNavigationEntry = navigationStack.Pop();
            CallNavigatingFrom(oldNavigationEntry, NavigationMode.Back);
            oldNavigationEntry.DisposeCachedItems();

            // Display the new current page from the navigation stack

            DisplayPage(CurrentNavigationEntry);

            // Call NavigatingTo()

            CallNavigatedTo(CurrentNavigationEntry, NavigationMode.Back);
        }

        public void NavigateTo(string pageName)
        {
            NavigateTo(pageName, null);
        }

        public void NavigateTo(string pageName, object arguments)
        {
            // Call NavigatingFrom on the existing navigation entry (if one exists)

            CallNavigatingFrom(CurrentNavigationEntry, NavigationMode.New);

            // Create the new navigation entry and push it onto the navigation stack

            NavigationEntry navigationEntry = new NavigationEntry(pageName, arguments);
            navigationStack.Push(navigationEntry);

            // Navigate to the page

            DisplayPage(navigationEntry);

            // Call NavigatedTo on the new navigation entry

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
                case Navigation.NavigationStorageType.Local:
                    restoredState = await storageManager.RetrieveAsync<NavigationManagerState>(ApplicationData.Current.LocalFolder, STORAGE_FILENAME);
                    break;
                case Navigation.NavigationStorageType.Roaming:
                    restoredState = await storageManager.RetrieveAsync<NavigationManagerState>(ApplicationData.Current.RoamingFolder, STORAGE_FILENAME);
                    break;
            }

            // If a navigation stack is available, then restore this

            if (restoredState != null)
            {
                foreach (NavigationEntryState entryState in restoredState.NavigationStack.Reverse())
                {
                    // Push the restored navigation entry onto the stack

                    navigationStack.Push(new NavigationEntry(entryState.PageName, entryState.ArgumentsData, entryState.StateData));
                }

                // Display the last page in the stack

                DisplayPage(CurrentNavigationEntry);

                // Call NavigatedTo() on the restored page

                CallNavigatedTo(CurrentNavigationEntry, NavigationMode.Refresh);

                // Return true to signal success

                return true;
            }

            // Otherwise return false

            return false;
        }

        // *** ILifetimeAware Methods ***

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
            // Store the current navigation stack to the relevant place

            switch (NavigationStorageType)
            {
                case Navigation.NavigationStorageType.Local:
                    await StoreNavigationStack(ApplicationData.Current.LocalFolder);
                    break;
                case Navigation.NavigationStorageType.Roaming:
                    await StoreNavigationStack(ApplicationData.Current.RoamingFolder);
                    break;
            }
        }

        // *** Private Methods ***

        private void CallNavigatedTo(NavigationEntry entry, NavigationMode navigationMode)
        {
            if (entry == null)
                return;

            object viewModel = entry.ViewLifetimeContext.ViewModel;

            if (viewModel is INavigationAware)
                ((INavigationAware)viewModel).NavigatedTo(navigationMode);
        }

        private void CallNavigatingFrom(NavigationEntry entry, NavigationMode navigationMode)
        {
            if (entry == null)
                return;

            object viewModel = entry.ViewLifetimeContext.ViewModel;

            if (viewModel is INavigationAware)
                ((INavigationAware)viewModel).NavigatingFrom(navigationMode);
        }

        private void CreatePage(NavigationEntry entry)
        {
            // Create the View

            IViewLifetimeContext viewLifetimeContext = viewFactory.CreateView(entry.PageName);
            entry.ViewLifetimeContext = viewLifetimeContext;

            // Activate the view model if it implements IActivatable<,>
            // NB: Use reflection as we do not know the generic parameter types

            object viewModel = entry.ViewLifetimeContext.ViewModel;
            Type activatableInterface = ReflectionHelper.GetClosedGenericType(viewModel, typeof(IActivatable<,>));

            if (activatableInterface != null)
            {
                // If required deserialize the arguments and state

                entry.DeserializeData(activatableInterface.GenericTypeArguments[0], activatableInterface.GenericTypeArguments[1]);

                // Activate the view model

                MethodInfo activateMethod = activatableInterface.GetTypeInfo().GetDeclaredMethod("Activate");
                activateMethod.Invoke(viewModel, new object[] { entry.Arguments, entry.State });
            }
        }

        private void DisplayPage(NavigationEntry entry)
        {
            // If the page and VM have not been created then do so

            if (entry.ViewLifetimeContext == null)
                CreatePage(entry);

            // Navigate to the relevant page

            navigationTarget.NavigateTo(entry.ViewLifetimeContext.View);
        }

        private void SavePageState(NavigationEntry entry)
        {
            // If the view model is IActivatable<,> then use this to save the page state
            // NB: First check that the view has been created - this may still have state from a previous instance
            // NB: Use reflection as we do not know the generic parameter types


            if (entry.ViewLifetimeContext != null)
            {
                // Get the generic IActivatable<,> interface

                object viewModel = entry.ViewLifetimeContext.ViewModel;
                Type activatableInterface = ReflectionHelper.GetClosedGenericType(viewModel, typeof(IActivatable<,>));

                if (activatableInterface != null)
                {
                    // Save the state

                    MethodInfo saveStateMethod = activatableInterface.GetTypeInfo().GetDeclaredMethod("SaveState");
                    entry.State = saveStateMethod.Invoke(viewModel, null);

                    // Serialize the arguments and state

                    entry.SerializeData(activatableInterface.GenericTypeArguments[0], activatableInterface.GenericTypeArguments[1]);
                }
            }
        }

        private Task StoreNavigationStack(StorageFolder folder)
        {
            // Create an object for storage of the navigation state

            NavigationManagerState state = new NavigationManagerState();

            // Enumerate all NavigationEntries in the navigation stack

            foreach (NavigationEntry entry in navigationStack)
            {
                // Save the page state
                // TODO : Do this when navigating away from each page to save time when suspending

                SavePageState(entry);

                // Create an object for storage of this entry

                NavigationEntryState entryState = new NavigationEntryState()
                        {
                            PageName = entry.PageName,
                            ArgumentsData = entry.ArgumentsData,
                            StateData = entry.StateData
                        };

                state.NavigationStack.Add(entryState);
            }

            // Store the state using the IStorageManager

            return storageManager.StoreAsync(folder, STORAGE_FILENAME, state);
        }

        // *** Private Sub-Classes ***

        [DataContract]
        private class NavigationManagerState
        {
            // *** Constructors ***

            public NavigationManagerState()
            {
                NavigationStack = new List<NavigationEntryState>();
            }

            // *** Properties ***

            [DataMember]
            public IList<NavigationEntryState> NavigationStack
            {
                get;
                private set;
            }
        }

        [DataContract]
        private class NavigationEntryState
        {
            // *** Properties ***

            [DataMember]
            public string PageName { get; set; }

            [DataMember]
            public byte[] ArgumentsData { get; set; }

            [DataMember]
            public byte[] StateData { get; set; }
        }
    }
}
