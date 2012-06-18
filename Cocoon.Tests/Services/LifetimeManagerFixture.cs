using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cocoon.Services;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using NSubstitute;
using Windows.ApplicationModel;

namespace Cocoon.Tests.Services
{
    [TestClass]
    public class LifetimeManagerFixture
    {
        [TestMethod]
        public void Register_ThrowsException_IfServiceIsNull()
        {
            LifetimeManager lifetimeManager = new LifetimeManager();

            Assert.ThrowsException<ArgumentNullException>(() => lifetimeManager.Register(null));
        }

        [TestMethod]
        public void Register_ThrowsException_WithMultipleRegistrationOfSameService()
        {
            var service = Substitute.For<ILifetimeAware>();

            LifetimeManager lifetimeManager = new LifetimeManager();

            lifetimeManager.Register(service);

            Assert.ThrowsException<InvalidOperationException>(() => lifetimeManager.Register(service));
        }

        [TestMethod]
        public void Unregister_ThrowsException_IfServiceIsNull()
        {
            LifetimeManager lifetimeManager = new LifetimeManager();

            Assert.ThrowsException<ArgumentNullException>(() => lifetimeManager.Unregister(null));
        }

        [TestMethod]
        public void Unregister_ThrowsException_IfServiceIsNotRegistered()
        {
            var service1 = Substitute.For<ILifetimeAware>();
            var service2 = Substitute.For<ILifetimeAware>();
            LifetimeManager lifetimeManager = new LifetimeManager();

            lifetimeManager.Register(service1);

            Assert.ThrowsException<InvalidOperationException>(() => lifetimeManager.Unregister(service2));
        }

        [TestMethod]
        public void RegistedServices_ReceiveSuspendingEvent()
        {
            var service1 = Substitute.For<ILifetimeAware>();
            service1.OnSuspending().Returns(Task.FromResult(true));
            var service2 = Substitute.For<ILifetimeAware>();
            service2.OnSuspending().Returns(Task.FromResult(true));

            TestableLifetimeManager lifetimeManager = CreateLifetimeManager(new[] { service1, service2 });

            lifetimeManager.Suspend(new MockSuspendingEventArgs());

            service1.Received().OnSuspending();
            service2.Received().OnSuspending();
        }

        [TestMethod]
        public void RegistedServices_ReceiveResumingEvent()
        {
            var service1 = Substitute.For<ILifetimeAware>();
            service1.OnResuming().Returns(Task.FromResult(true));
            var service2 = Substitute.For<ILifetimeAware>();
            service2.OnResuming().Returns(Task.FromResult(true));

            TestableLifetimeManager lifetimeManager = CreateLifetimeManager(new[] { service1, service2 });

            lifetimeManager.Resume();

            service1.Received().OnResuming();
            service2.Received().OnResuming();
        }

        [TestMethod]
        public void RegistedServices_ReceiveExitingEvent()
        {
            var service1 = Substitute.For<ILifetimeAware>();
            service1.OnExiting().Returns(Task.FromResult(true));
            var service2 = Substitute.For<ILifetimeAware>();
            service2.OnExiting().Returns(Task.FromResult(true));

            TestableLifetimeManager lifetimeManager = CreateLifetimeManager(new[] { service1, service2 });

            lifetimeManager.Exit();

            service1.Received().OnExiting();
            service2.Received().OnExiting();
        }

        [TestMethod]
        public void RegistedServices_ReceiveMultipleEvents()
        {
            var service1 = Substitute.For<ILifetimeAware>();
            service1.OnExiting().Returns(Task.FromResult(true));
            service1.OnResuming().Returns(Task.FromResult(true));
            service1.OnSuspending().Returns(Task.FromResult(true));

            TestableLifetimeManager lifetimeManager = CreateLifetimeManager(new[] { service1 });

            lifetimeManager.Suspend(new MockSuspendingEventArgs());
            lifetimeManager.Resume();
            lifetimeManager.Exit();

            service1.Received().OnSuspending();
            service1.Received().OnResuming();
            service1.Received().OnExiting();
        }

        [TestMethod]
        public async Task RegistedServices_CausesDeferralOfSuspension()
        {
            var tcs1 = new TaskCompletionSource<bool>();
            var service1 = Substitute.For<ILifetimeAware>();
            service1.OnSuspending().Returns(tcs1.Task);

            var tcs2 = new TaskCompletionSource<bool>();
            var service2 = Substitute.For<ILifetimeAware>();
            service2.OnSuspending().Returns(tcs2.Task);

            TestableLifetimeManager lifetimeManager = CreateLifetimeManager(new[] { service1, service2 });

            MockSuspendingEventArgs suspendingEventArgs = new MockSuspendingEventArgs();
            lifetimeManager.Suspend(suspendingEventArgs);

            Assert.IsTrue(suspendingEventArgs.IsDeferred);

            tcs1.SetResult(true);
            await Task.Yield();
            Assert.IsTrue(suspendingEventArgs.IsDeferred);

            tcs2.SetResult(true);
            await Task.Yield();
            Assert.IsFalse(suspendingEventArgs.IsDeferred);
        }

        private TestableLifetimeManager CreateLifetimeManager(ILifetimeAware[] services)
        {
            TestableLifetimeManager lifetimeManager = new TestableLifetimeManager();

            foreach (ILifetimeAware service in services)
                lifetimeManager.Register(service);

            return lifetimeManager;
        }

        private class TestableLifetimeManager : LifetimeManager
        {
            public void Suspend(ISuspendingEventArgs e)
            {
                base.OnSuspending(null, e);
            }

            public void Resume()
            {
                base.OnResuming(null, null);
            }

            public void Exit()
            {
                base.OnExiting(null, null);
            }

            protected override ISuspendingDeferral GetDeferral(ISuspendingEventArgs e)
            {
                return ((MockSuspendingEventArgs)e).GetDeferral();
            }
        }

        [Obsolete]
        public class MockSuspendingEventArgs : ISuspendingEventArgs
        {
            public IList<ISuspendingDeferral> Deferrals = new List<ISuspendingDeferral>();

            public bool IsDeferred
            {
                get { return Deferrals.Any(d => !d.ReceivedCalls().Any()); }
            }

            public SuspendingOperation SuspendingOperation
            {
                get { throw new NotImplementedException(); }
            }

            public ISuspendingDeferral GetDeferral()
            {
                var deferral = Substitute.For<ISuspendingDeferral>();
                Deferrals.Add(deferral);
                return deferral;
            }
        }
    }
}