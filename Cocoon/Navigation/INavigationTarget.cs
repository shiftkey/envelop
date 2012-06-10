using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cocoon.Navigation
{
    public interface INavigationTarget
    {
        // *** Methods ***

        void NavigateTo(object page);
    }
}
