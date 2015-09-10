using Xunit;

namespace Autofac.Multitenant.Wcf.Test
{
    public class OperationContextTenantIdentificationStrategyFixture
    {
        [Fact]
        public void TryIdentifyTenant_NoOperationContext()
        {
            var strategy = new OperationContextTenantIdentificationStrategy();
            object tenantId;
            bool success = strategy.TryIdentifyTenant(out tenantId);
            Assert.False(success);
        }
    }
}
