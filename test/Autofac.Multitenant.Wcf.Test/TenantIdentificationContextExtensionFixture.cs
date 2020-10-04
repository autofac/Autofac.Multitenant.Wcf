// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Xunit;

namespace Autofac.Multitenant.Wcf.Test
{
    public class TenantIdentificationContextExtensionFixture
    {
        [Fact]
        public void Attach_NoOp()
        {
            // XUnit does not have "Assert.DoesNotThrow".
            new TenantIdentificationContextExtension().Attach(null);
        }

        [Fact]
        public void Detach_NoOp()
        {
            // XUnit does not have "Assert.DoesNotThrow".
            new TenantIdentificationContextExtension().Detach(null);
        }
    }
}
