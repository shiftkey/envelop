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

        private IActivationManager ActivationManager { get; set; }

        protected IContainer Container { get; private set; }

        public virtual void Initialize()
        {
            Initialize(true);
        }

        protected virtual IContainer Configure()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<NavigationManager>()
                   .AsImplementedInterfaces()
                   .SingleInstance();
            builder.RegisterType<ActivationManager>()
                   .AsImplementedInterfaces()
                   .SingleInstance();
            builder.RegisterType<WindowNavigationTarget>()
                   .AsImplementedInterfaces()
                   .SingleInstance();
            builder.RegisterType<ViewFactory>()
                   .AsImplementedInterfaces()
                   .SingleInstance();
            builder.RegisterType<LifetimeManager>()
                   .AsImplementedInterfaces()
                   .SingleInstance();
            builder.RegisterType<StorageManager>()
                   .AsImplementedInterfaces()
                   .SingleInstance();

            RegisterDependencies(builder);

            return builder.Build();
        }

        protected abstract void RegisterDependencies(ContainerBuilder builder);

        protected void Initialize(bool registerForActivation)
        {
            Container = Configure();
            ActivationManager = Container.Resolve<IActivationManager>();

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
            return SpecialPageNames.Home;
        }
    }
}
