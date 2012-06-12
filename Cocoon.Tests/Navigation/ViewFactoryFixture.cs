using System;
using System.Collections.Generic;
using System.Composition;
using Cocoon.Navigation;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Cocoon.Tests.Navigation
{
    [TestClass]
    public class ViewFactoryFixture
    {
        // *** Method Tests ***

        [TestMethod]
        public void CreateView_CreatesNewPage_WithViewModel()
        {
            IViewFactory viewFactory = CreateViewFactory();

            IViewLifetimeContext lifetimeContext = viewFactory.CreateView("Page 2");
            object view = lifetimeContext.View;

            Assert.IsNotNull(view);
            Assert.IsInstanceOfType(view, typeof(MockPage));
            Assert.AreEqual("Page 2", ((MockPage)view).PageName);
        }

        [TestMethod]
        public void CreateView_CreatesNewPage_WithoutViewModel()
        {
            IViewFactory viewFactory = CreateViewFactory();

            IViewLifetimeContext lifetimeContext = viewFactory.CreateView("Page 3");
            object view = lifetimeContext.View;

            Assert.IsNotNull(view);
            Assert.IsInstanceOfType(view, typeof(MockPage));
            Assert.AreEqual("Page 3", ((MockPage)view).PageName);
        }

        [TestMethod]
        public void CreateView_CreatesNewViewModel_WithViewModel()
        {
            IViewFactory viewFactory = CreateViewFactory();

            IViewLifetimeContext lifetimeContext = viewFactory.CreateView("Page 2");
            object viewModel = lifetimeContext.ViewModel;

            Assert.IsNotNull(viewModel);
            Assert.IsInstanceOfType(viewModel, typeof(MockViewModel<string, string>));
            Assert.AreEqual("ViewModel 2", ((MockViewModel<string, string>)viewModel).Name);
        }

        [TestMethod]
        public void CreateView_ViewModelIsNull_WithoutViewModel()
        {
            IViewFactory viewFactory = CreateViewFactory();

            IViewLifetimeContext lifetimeContext = viewFactory.CreateView("Page 3");
            object viewModel = lifetimeContext.ViewModel;

            Assert.IsNull(viewModel);
        }

        [TestMethod]
        public void CreateView_SetsViewModel_WithViewModel()
        {
            IViewFactory viewFactory = CreateViewFactory();

            IViewLifetimeContext lifetimeContext = viewFactory.CreateView("Page 2");
            MockPage page = lifetimeContext.View as MockPage;
            object viewModel = lifetimeContext.ViewModel;

            Assert.AreEqual(viewModel, page.DataContext);
        }

        [TestMethod]
        public void CreateView_SetsViewModel_ToNullWithoutViewModel()
        {
            IViewFactory viewFactory = CreateViewFactory();

            IViewLifetimeContext lifetimeContext = viewFactory.CreateView("Page 3");
            MockPage page = lifetimeContext.View as MockPage;
            object viewModel = page.DataContext;

            Assert.IsNull(viewModel);
        }

        [TestMethod]
        public void CreateView_ThrowsException_NoPageWithSpecifiedName()
        {
            IViewFactory viewFactory = CreateViewFactory();

            Assert.ThrowsException<InvalidOperationException>(() => viewFactory.CreateView("Page X"));
        }

        // *** Behaviour Tests ***

        [TestMethod]
        public void DisposingViewLifetimeContext_DisposesCurrentPage_WithViewModel()
        {
            IViewFactory viewFactory = CreateViewFactory();

            IViewLifetimeContext lifetimeContext = viewFactory.CreateView("Page 2");
            MockPage page = lifetimeContext.View as MockPage;

            lifetimeContext.Dispose();

            Assert.AreEqual(true, page.IsDisposed);
        }

        [TestMethod]
        public void DisposingViewLifetimeContext_DisposesCurrentPage_WithoutViewModel()
        {
            IViewFactory viewFactory = CreateViewFactory();

            IViewLifetimeContext lifetimeContext = viewFactory.CreateView("Page 3");
            MockPage page = lifetimeContext.View as MockPage;

            lifetimeContext.Dispose();

            Assert.AreEqual(true, page.IsDisposed);
        }

        [TestMethod]
        public void DisposingViewLifetimeContext_DisposesCurrentViewModel()
        {
            IViewFactory viewFactory = CreateViewFactory();

            IViewLifetimeContext lifetimeContext = viewFactory.CreateView("Page 2");
            MockPage page = lifetimeContext.View as MockPage;
            MockViewModel<string, string> viewModel = page.DataContext as MockViewModel<string, string>;

            lifetimeContext.Dispose();

            Assert.AreEqual(true, viewModel.IsDisposed);
        }

        // *** Private Methods ***

        private IViewFactory CreateViewFactory()
        {
            IEnumerable<ExportFactory<object, PageMetadata>> pageFactories = new List<ExportFactory<object, PageMetadata>>
                {
                    new ExportFactory<object, PageMetadata>(delegate() {return CreatePage("Page 1");}, new PageMetadata { PageName = "Page 1"}),
                    new ExportFactory<object, PageMetadata>(delegate() {return CreatePage("Page 2");}, new PageMetadata { PageName = "Page 2"}),
                    new ExportFactory<object, PageMetadata>(delegate() {return CreatePage("Page 3");}, new PageMetadata { PageName = "Page 3"})
                };

            IEnumerable<ExportFactory<object, ViewModelMetadata>> viewModelFactories = new List<ExportFactory<object, ViewModelMetadata>>
                {
                    new ExportFactory<object, ViewModelMetadata>(delegate() {return CreateViewModel("ViewModel 1");}, new ViewModelMetadata  { PageName = "Page 1"}),
                    new ExportFactory<object, ViewModelMetadata>(delegate() {return CreateViewModel("ViewModel 2");}, new ViewModelMetadata  { PageName = "Page 2"})
                };

            return new TestableViewFactory(pageFactories, viewModelFactories);
        }

        private Tuple<object, Action> CreatePage(string pageName)
        {
            MockPage page = new MockPage() { PageName = pageName };

            return new Tuple<object, Action>(page, delegate() { page.IsDisposed = true; });
        }

        private Tuple<object, Action> CreateViewModel(string viewModelName)
        {
            MockViewModel<string, string> viewModel = new MockViewModel<string, string>() { Name = viewModelName };

            return new Tuple<object, Action>(viewModel, delegate() { viewModel.IsDisposed = true; });
        }

        // *** Private Sub-classes ***

        private class TestableViewFactory : ViewFactory
        {
            // *** Constructors ***

            public TestableViewFactory(IEnumerable<ExportFactory<object, PageMetadata>> pageFactories, IEnumerable<ExportFactory<object, ViewModelMetadata>> viewModelFactories)
                : base(pageFactories, viewModelFactories)
            {
            }

            // *** Overriden Base Methods ***

            protected override void AttachViewModel(object page, object viewModel)
            {
                if (page is MockPage)
                    ((MockPage)page).DataContext = viewModel;
            }
        }

        private class MockPage
        {
            // *** Properties ***

            public bool IsDisposed { get; set; }
            public string PageName { get; set; }
            public object DataContext { get; set; }
        }

        private class MockViewModel<TArguments, TState> : IActivatable<TArguments, TState>
        {
            // *** Properties ***

            public TArguments ActivationArguments { get; private set; }
            public TState ActivationState { get; private set; }
            public bool IsActivated { get; private set; }
            public bool IsDisposed { get; set; }
            public string Name { get; set; }
            public TState State { get; set; }

            // *** Methods ***

            public void Activate(TArguments arguments, TState state)
            {
                IsActivated = true;
                ActivationArguments = arguments;
                ActivationState = state;
            }

            public TState SaveState()
            {
                return State;
            }
        }
    }
}