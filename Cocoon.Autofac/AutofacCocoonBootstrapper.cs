using Autofac;
using Cocoon.Navigation;
using Cocoon.Services;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;

namespace Cocoon
{
    public abstract class AutofacCocoonBootstrapper
    {
        bool isActivated;

        public IActivationManager ActivationManager { get; set; }

        public INavigationManager NavigationManager { get; set; }

        public virtual void Initialize()
        {
            Initialize(true);
        }

        protected virtual IContainer Configure()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<NavigationManager>().AsImplementedInterfaces();
            builder.RegisterType<ActivationManager>().AsImplementedInterfaces();
            builder.RegisterType<WindowNavigationTarget>()
                   .AsImplementedInterfaces()
                   .SingleInstance();
            builder.RegisterType<ViewFactory>().AsImplementedInterfaces();
            builder.RegisterType<LifetimeManager>().AsImplementedInterfaces();
            builder.RegisterType<StorageManager>().AsImplementedInterfaces();

            RegisterDependencies(builder);

            return builder.Build();
        }

        protected abstract void RegisterDependencies(ContainerBuilder builder);

        protected void Initialize(bool registerForActivation)
        {
            var container = Configure();
            ActivationManager = container.Resolve<IActivationManager>();
            NavigationManager = container.Resolve<INavigationManager>();

            if (!registerForActivation) 
                return;

            var coreApplicationView = CoreApplication.GetCurrentView();
            coreApplicationView.Activated += OnActivated;
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
            if (!isActivated)
            {
                SetupServices();
                isActivated = true;
            }

            var page = SelectHomePage();

            ActivationManager.Activate(args, page);
        }

        public virtual string SelectHomePage()
        {
            return SpecialPageNames.HomePage;
        }
    }
}
