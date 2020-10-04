// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using Xunit;

namespace Autofac.Multitenant.Wcf.Test
{
    public class ServiceMetadataTypeAttributeFixture
    {
        [Fact]
        public void Ctor_NullType()
        {
            Assert.Throws<ArgumentNullException>(() => new ServiceMetadataTypeAttribute(null));
        }

        [Fact]
        public void Ctor_SetsProperties()
        {
            var attrib = new ServiceMetadataTypeAttribute(typeof(string));
            Assert.Equal(typeof(string), attrib.MetadataClassType);
        }
    }
}
