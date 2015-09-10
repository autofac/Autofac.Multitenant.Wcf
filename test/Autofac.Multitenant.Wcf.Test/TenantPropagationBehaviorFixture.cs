using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Autofac.Multitenant.Wcf.Test.Stubs;
using Xunit;

namespace Autofac.Multitenant.Wcf.Test
{
    public class TenantPropagationBehaviorFixture
    {
        [Fact]
        public void Ctor_NullContainerProviderFunction()
        {
            Assert.Throws<ArgumentNullException>(() => new TenantPropagationBehavior<string>(null));
        }

        [Fact]
        public void AddBindingParameters2_NoOp()
        {
            var behavior = new TenantPropagationBehavior<string>(new StubTenantIdentificationStrategy());

            // XUnit does not have "Assert.DoesNotThrow".
            behavior.AddBindingParameters(null, null);
        }

        [Fact]
        public void AddBindingParameters4_NoOp()
        {
            var behavior = new TenantPropagationBehavior<string>(new StubTenantIdentificationStrategy());

            // XUnit does not have "Assert.DoesNotThrow".
            behavior.AddBindingParameters(null, null, null, null);
        }

        [Fact]
        public void ApplyClientBehavior_NullServiceHost()
        {
            var behavior = new TenantPropagationBehavior<string>(new StubTenantIdentificationStrategy());
            var ex = Assert.Throws<ArgumentNullException>(() => behavior.ApplyClientBehavior(null, null));
            Assert.Equal("clientRuntime", ex.ParamName);
        }

        [Fact]
        public void ApplyDispatchBehavior_Client_NoOp()
        {
            var behavior = new TenantPropagationBehavior<string>(new StubTenantIdentificationStrategy());

            // XUnit does not have "Assert.DoesNotThrow".
            behavior.ApplyDispatchBehavior((ServiceEndpoint)null, (EndpointDispatcher)null);
        }

        [Fact]
        public void ApplyDispatchBehavior_Service_NullServiceHost()
        {
            var behavior = new TenantPropagationBehavior<string>(new StubTenantIdentificationStrategy());
            var ex = Assert.Throws<ArgumentNullException>(() => behavior.ApplyDispatchBehavior((ServiceDescription)null, (ServiceHostBase)null));
            Assert.Equal("serviceHostBase", ex.ParamName);
        }

        [Fact]
        public void Validate1_NoOp()
        {
            var behavior = new TenantPropagationBehavior<string>(new StubTenantIdentificationStrategy());

            // XUnit does not have "Assert.DoesNotThrow".
            behavior.Validate(null);
        }

        [Fact]
        public void Validate2_NoOp()
        {
            var behavior = new TenantPropagationBehavior<string>(new StubTenantIdentificationStrategy());

            // XUnit does not have "Assert.DoesNotThrow".
            behavior.Validate(null, null);
        }
    }
}
