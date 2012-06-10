using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using Cocoon.Helpers;
using Windows.UI.Xaml;

namespace Cocoon.Navigation
{
    [Export(typeof(IViewFactory))]
    [Shared]
    public class ViewFactory : IViewFactory
    {
        // *** Fields ***

        private readonly IEnumerable<ExportFactory<object, PageMetadata>> pageFactories;
        private readonly IEnumerable<ExportFactory<object, ViewModelMetadata>> viewModelFactories;

        // *** Constructors ***

        [ImportingConstructor]
        public ViewFactory([ImportMany("CocoonPage")]IEnumerable<ExportFactory<object, PageMetadata>> pageFactories, [ImportMany("CocoonViewModel")]IEnumerable<ExportFactory<object, ViewModelMetadata>> viewModelFactories)
        {
            this.pageFactories = pageFactories;
            this.viewModelFactories = viewModelFactories;
        }

        // *** Methods ***

        public IViewLifetimeContext CreateView(string name)
        {
            // Get the page export

            ExportFactory<object, PageMetadata> pageFactory = pageFactories.Where(export => export.Metadata.PageName == name).FirstOrDefault();

            // If no suitable page is found then throw an exception

            if (pageFactory == null)
                throw new InvalidOperationException(string.Format(ResourceHelper.GetErrorResource("Exception_InvalidOperation_CannotNavigateAsPageIsNotFound"), name));

            // Create the page instance

            Export<object> pageLifetimeContext = pageFactory.CreateExport();
            object page = pageLifetimeContext.Value;

            // Attach the view model (if one exists)

            ExportFactory<object, ViewModelMetadata> viewModelFactory = viewModelFactories.Where(export => export.Metadata.PageName == name).FirstOrDefault();
            Export<object> viewModelLifetimeContext = null;

            if (viewModelFactory != null)
            {
                // Create the view model instance

                viewModelLifetimeContext = viewModelFactory.CreateExport();
                object viewModel = viewModelLifetimeContext.Value;

                // Attach the view-model to the page
                // NB: Do this via a virtual method call to help with unit testing

                AttachViewModel(page, viewModel);
            }

            // Return a new IViewLifetimeContext

            return new ViewLifetimeContext(pageLifetimeContext, viewModelLifetimeContext);
        }

        // *** Protected Methods ***

        protected virtual void AttachViewModel(object page, object viewModel)
        {
            if (page is FrameworkElement)
                ((FrameworkElement)page).DataContext = viewModel;
        }

        // *** Private Sub-Classes ***

        private sealed class ViewLifetimeContext : IViewLifetimeContext
        {
            // *** Fields ***

            private readonly Export<object> pageLifetimeContext;
            private readonly Export<object> viewModelLifetimeContext;

            // *** Constructors ***

            public ViewLifetimeContext(Export<object> pageLifetimeContext, Export<object> viewModelLifetimeContext)
            {
                this.pageLifetimeContext = pageLifetimeContext;
                this.viewModelLifetimeContext = viewModelLifetimeContext;
            }

            // *** Properties ***

            public object View
            {
                get
                {
                    return pageLifetimeContext.Value;
                }
            }

            public object ViewModel
            {
                get
                {
                    if (viewModelLifetimeContext == null)
                        return null;
                    else
                        return viewModelLifetimeContext.Value;
                }
            }

            // *** IDisposable Implementation ***

            public void Dispose()
            {
                // NB: No need for a more complex Dispose implementation as this class is sealed

                if (pageLifetimeContext != null)
                    pageLifetimeContext.Dispose();

                if (viewModelLifetimeContext != null)
                    viewModelLifetimeContext.Dispose();
            }
        }
    }
}
