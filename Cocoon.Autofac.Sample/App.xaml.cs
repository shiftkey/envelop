namespace Cocoon.Autofac.Sample
{
    public sealed partial class App
    {
        public App()
        {
            InitializeComponent();

            var bootstrapper = new AppBootstrapper();
            bootstrapper.Initialize();
        }
    }
}
