using System;
using System.ServiceModel.Channels;
using Autofac.Multitenant.Wcf.Test.Stubs;
using Xunit;

namespace Autofac.Multitenant.Wcf.Test
{
    public class TenantPropagationMessageInspectorFixture
    {
        [Fact]
        public void Ctor_NullContainerProviderFunction()
        {
            Assert.Throws<ArgumentNullException>(() => new TenantPropagationMessageInspector<string>(null));
        }

        [Fact]
        public void AfterReceiveReply_NoOp()
        {
            var inspector = new TenantPropagationMessageInspector<string>(new StubTenantIdentificationStrategy());
            Message msg = null;

            // XUnit does not have "Assert.DoesNotThrow".
            inspector.AfterReceiveReply(ref msg, null);
        }

        [Fact]
        public void BeforeSendReply_NoOp()
        {
            var inspector = new TenantPropagationMessageInspector<string>(new StubTenantIdentificationStrategy());
            Message msg = null;

            // XUnit does not have "Assert.DoesNotThrow".
            inspector.BeforeSendReply(ref msg, null);
        }
    }
}
