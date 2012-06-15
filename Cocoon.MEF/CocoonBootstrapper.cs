using System.Composition;
using System.Composition.Convention;
using System.Composition.Hosting;
using System.Reflection;
using System.Linq;
using Cocoon.Navigation;
using Cocoon.Services;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;

namespace Cocoon
{
    public abstract class CocoonBootstrapper
    {
        // *** Fields ***

        private bool isActivated;

        // *** Imported Properties ***

        [Import]
        public IActivationManager ActivationManager { get; set; }

        [Import]
        public INavigationManager NavigationManager { get; set; }

        // *** Public Methods ***

        public virtual void Initialize()
        {
            Initialize(true);
        }

        // *** Protected Methods ***

        protected virtual ContainerConfiguration GetContainerConfiguration()
        {
            // Create a basic container configuration with,
            //    - The application's main assembly (i.e. that defines the current Application subclass)
            //    - The Cocoon assembly (via the INavigationManager interface)
            //    - The Cocoon.MEF assembly (via the CocoonBootstrapper class)

            ConventionBuilder cocoonConventionBuilder = new ConventionBuilder();

            cocoonConventionBuilder.ForType<NavigationManager>().Export<INavigationManager>()
                                                                .Shared()
                                                                .SelectConstructor(ctors => ctors.First(), (info, builder) =>
                                                                    {
                                                                        if (info.ParameterType == typeof(INavigationTarget))
                                                                            builder.AllowDefault();
                                                                    });

            cocoonConventionBuilder.ForType<ActivationManager>().Export<IActivationManager>().Shared();
            cocoonConventionBuilder.ForType<LifetimeManager>().Export<ILifetimeManager>().Shared();
            cocoonConventionBuilder.ForType<StorageManager>().Export<IStorageManager>().Shared();

            return new ContainerConfiguration()
                        .WithAssembly(Application.Current.GetType().GetTypeInfo().Assembly)
                        .WithAssembly(typeof(INavigationManager).GetTypeInfo().Assembly, cocoonConventionBuilder)
                        .WithAssembly(typeof(CocoonBootstrapper).GetTypeInfo().Assembly);
        }

        protected void Initialize(bool registerForActivation)
        {
            // Initialize MEF and compose bootstrapper

            ContainerConfiguration containerConfiguration = GetContainerConfiguration();
            CompositionHost compositionHost = containerConfiguration.CreateContainer();
            compositionHost.SatisfyImports(this);

            // Attach to the CoreApplicationView for activation events

            if (registerForActivation)
            {
                CoreApplicationView coreApplicationView = CoreApplication.GetCurrentView();
                coreApplicationView.Activated += OnActivated;
            }
        }

        protected virtual void SetupNavigationManager()
        {
        }

        protected virtual void SetupServices()
        {
            SetupNavigationManager();
        }

        protected virtual void OnActivated(CoreApplicationView sender, IActivatedEventArgs args)
        {
            // Setup services if this is the first activation

            if (!isActivated)
            {
                SetupServices();
                isActivated = true;
            }

            // Call the activation manager

            ActivationManager.Activate(args);
        }
    }
}
