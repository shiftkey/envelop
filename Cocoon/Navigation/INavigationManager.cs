using System;
using System.Threading.Tasks;

namespace Cocoon.Navigation
{
    public interface INavigationManager
    {
        bool CanGoBack { get; }
        [Obsolete("Doesn't seem to be an easy way to programatically set this")]
        string HomePageName { get; set; }
        NavigationStorageType NavigationStorageType { get; set; }

        void GoBack();
        void NavigateTo(string pageName);
        void NavigateTo(string pageName, object arguments);
        Task<bool> RestoreNavigationStack();
    }
}
