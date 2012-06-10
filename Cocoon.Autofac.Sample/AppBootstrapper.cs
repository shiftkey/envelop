using Autofac;
using Cocoon.Autofac.Sample.Data;
using Cocoon.Autofac.Sample.Pages;
using Cocoon.Autofac.Sample.ViewModels;

namespace Cocoon.Autofac.Sample
{
    public class AppBootstrapper : AutofacCocoonBootstrapper
    {
        protected override void RegisterDependencies(ContainerBuilder builder)
        {
            builder.RegisterType<InterestingPhotosPage>().Named<object>("InterestingPhotosView");
            builder.RegisterType<InterestingPhotosViewModel>().Named<object>("InterestingPhotosViewModel");
            builder.RegisterType<FlickrDataSource>().AsSelf();
        }
    }
}
