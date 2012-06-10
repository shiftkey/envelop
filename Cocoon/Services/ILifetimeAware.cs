using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cocoon.Services
{
    public interface ILifetimeAware
    {
        // *** Methods ***

        Task OnExiting();
        Task OnResuming();
        Task OnSuspending();
    }
}
