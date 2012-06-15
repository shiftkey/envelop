using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cocoon.Navigation
{
    public interface IViewFactory
    {
        // *** Methods ***

        IViewLifetimeContext CreateView(string name);
    }
}
