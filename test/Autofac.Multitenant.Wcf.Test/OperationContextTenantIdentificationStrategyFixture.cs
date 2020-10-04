// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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
