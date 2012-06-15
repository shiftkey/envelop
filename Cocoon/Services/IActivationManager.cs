using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;

namespace Cocoon.Services
{
    public interface IActivationManager
    {
        Task<bool> Activate(IActivatedEventArgs activatedEventArgs);
    }
}
