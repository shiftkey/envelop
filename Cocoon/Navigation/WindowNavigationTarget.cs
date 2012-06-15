using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Cocoon.Navigation
{
    public class WindowNavigationTarget : INavigationTarget
    {
        public void NavigateTo(object page)
        {
            Window.Current.Content = (UIElement)page;
        }
    }
}
