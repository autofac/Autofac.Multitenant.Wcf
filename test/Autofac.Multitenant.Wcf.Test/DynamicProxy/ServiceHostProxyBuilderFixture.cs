// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using Autofac.Multitenant.Wcf.DynamicProxy;
using Castle.DynamicProxy.Generators;
using Xunit;

namespace Autofac.Multitenant.Wcf.Test.DynamicProxy
{
    public class ServiceHostProxyBuilderFixture
    {
        [Fact]
        public void CreateWcfProxyType_BuildsProxyType()
        {
            var builder = new ServiceHostProxyBuilder();
            var type = builder.CreateWcfProxyType(typeof(ValidType));
            Assert.NotNull(type);
        }

        [Fact]
        public void CreateWcfProxyType_NullTypeToProxy()
        {
            var builder = new ServiceHostProxyBuilder();
            Assert.Throws<ArgumentNullException>(() => builder.CreateWcfProxyType(null));
        }

        [Fact]
        public void CreateWcfProxyType_TypeIsGeneric()
        {
            var builder = new ServiceHostProxyBuilder();
            Assert.Throws<GeneratorException>(() => builder.CreateWcfProxyType(typeof(GenericType<>)));
        }

        [Fact]
        public void CreateWcfProxyType_TypeNotAccessible()
        {
            var builder = new ServiceHostProxyBuilder();
            Assert.Throws<GeneratorException>(() => builder.CreateWcfProxyType(typeof(PrivateType)));
        }

        private class PrivateType
        {
        }

        public class GenericType<T>
        {
        }

        public class ValidType
        {
        }
    }
}
