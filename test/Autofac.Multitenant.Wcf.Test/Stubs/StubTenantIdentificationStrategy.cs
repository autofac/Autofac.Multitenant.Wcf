// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Autofac.Multitenant.Wcf.Test.Stubs
{
    public class StubTenantIdentificationStrategy : ITenantIdentificationStrategy
    {
        public bool IdentificationSuccess { get; }

        public object TenantId { get; set; }

        public StubTenantIdentificationStrategy()
        {
            this.IdentificationSuccess = true;
        }

        public bool TryIdentifyTenant(out object tenantId)
        {
            tenantId = this.TenantId;
            return this.IdentificationSuccess;
        }
    }
}
