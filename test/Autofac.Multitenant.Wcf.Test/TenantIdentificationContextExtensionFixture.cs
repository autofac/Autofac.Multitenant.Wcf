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
            var exception = Record.Exception(() => new TenantIdentificationContextExtension().Attach(null));
            Assert.Null(exception);
        }

        [Fact]
        public void Detach_NoOp()
        {
            var exception = Record.Exception(() => new TenantIdentificationContextExtension().Detach(null));
            Assert.Null(exception);
        }
    }
}
