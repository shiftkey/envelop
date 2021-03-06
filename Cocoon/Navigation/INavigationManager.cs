﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;

namespace Cocoon.Navigation
{
    public interface INavigationManager
    {
        bool CanGoBack { get; }
        string HomePageName { get; set; }
        NavigationStorageType NavigationStorageType { get; set; }

        void GoBack();
        void NavigateTo(string pageName);
        void NavigateTo(string pageName, object arguments);
        Task<bool> RestoreNavigationStack();
    }
}
