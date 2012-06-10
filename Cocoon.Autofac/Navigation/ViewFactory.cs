using System;
using Autofac;
using Cocoon.Helpers;
using Windows.UI.Xaml;

namespace Cocoon.Navigation
{
    public class ViewFactory : IViewFactory
    {
        private readonly IComponentContext context;

        public ViewFactory(IComponentContext context)
        {
            this.context = context;
        }

        public IViewLifetimeContext CreateView(string name)
        {
            var viewName = string.Concat(name, "View");
            var viewModelName = string.Concat(viewName, "Model");

            var page = context.ResolveNamed<object>(viewName);
            var viewModel = context.ResolveNamed<object>(viewModelName);

            if (page == null)
                throw new InvalidOperationException(string.Format(ResourceHelper.GetErrorResource("Exception_InvalidOperation_CannotNavigateAsPageIsNotFound"), name));

            if (viewModel != null)
            {
                AttachViewModel(page, viewModel);
            }

            return new ViewLifetimeContext(page, viewModel);
        }

        protected virtual void AttachViewModel(object page, object viewModel)
        {
            var frameworkElement = page as FrameworkElement;
            if (frameworkElement != null)
                frameworkElement.DataContext = viewModel;
        }
    }

    internal sealed class ViewLifetimeContext : IViewLifetimeContext
    {
        public ViewLifetimeContext(object page, object viewModel)
        {
            View = page;
            ViewModel = viewModel;
        }

        public object View { get; private set; }

        public object ViewModel { get; private set; }

        public void Dispose()
        {
            var disposableView = View as IDisposable;
            if (disposableView != null)
                disposableView.Dispose();

            var disposableViewModel = ViewModel as IDisposable;
            if (disposableViewModel != null)
                disposableViewModel.Dispose();
        }
    }

}
