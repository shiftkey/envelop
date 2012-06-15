using Autofac;
using Cocoon.Autofac.Sample.Data;
using Cocoon.Autofac.Sample.Modules.Home;

namespace Cocoon.Autofac.Sample
{
    public class AppBootstrapper : AutofacCocoonBootstrapper
    {
        protected override void RegisterDependencies(ContainerBuilder builder)
        {
            builder.RegisterType<UserContext>()
                   .AsImplementedInterfaces();
            builder.RegisterType<InterestingPhotosPage>()
                   .Named<object>("InterestingPhotosView");
            builder.RegisterType<InterestingPhotosViewModel>()
                   .Named<object>("InterestingPhotosViewModel");
            builder.RegisterType<FlickrDataSource>()
                   .AsSelf();
        }

        //public override string SelectHomePage()
        //{
        //    var context = Container.Resolve<IUserContext>();
        //    return context.GetHomePage();
        //}
    }
}
