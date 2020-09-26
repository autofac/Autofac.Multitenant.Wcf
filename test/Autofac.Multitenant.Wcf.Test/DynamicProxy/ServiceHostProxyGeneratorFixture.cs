// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.ServiceModel;
using Autofac.Multitenant.Wcf.DynamicProxy;
using Xunit;

namespace Autofac.Multitenant.Wcf.Test.DynamicProxy
{
    public class ServiceHostProxyGeneratorFixture
    {
        [Fact]
        public void Ctor_ProxyBuilderIsServiceHostProxyBuilder()
        {
            var generator = new ServiceHostProxyGenerator();
            Assert.IsType<ServiceHostProxyBuilder>(generator.ProxyBuilder);
        }

        [Fact]
        public void CreateWcfProxy_CustomProxyTypeCanBeHosted()
        {
            var generator = new ServiceHostProxyGenerator();
            object target = new ServiceImplementation();
            Type interfaceToProxy = typeof(IServiceContract);
            var proxy = generator.CreateWcfProxy(interfaceToProxy, target);

            // XUnit does not have "Assert.DoesNotThrow".
            new ServiceHost(proxy.GetType(), new Uri("http://localhost:22111/Foo.svc"));
        }

        [Fact]
        public void CreateWcfProxy_InterfaceToProxyNotInterface()
        {
            var generator = new ServiceHostProxyGenerator();
            object target = new ServiceImplementation();
            Type interfaceToProxy = typeof(ServiceImplementation);
            Assert.Throws<ArgumentException>(() => generator.CreateWcfProxy(interfaceToProxy, target));
        }

        [Fact]
        public void CreateWcfProxy_InterfaceToProxyNotServiceContract()
        {
            var generator = new ServiceHostProxyGenerator();
            object target = new NotAServiceImplementation();
            Type interfaceToProxy = typeof(INotAServiceContract);
            Assert.Throws<ArgumentException>(() => generator.CreateWcfProxy(interfaceToProxy, target));
        }

        [Fact]
        public void CreateWcfProxy_InterfaceToProxyIsGeneric()
        {
            var generator = new ServiceHostProxyGenerator();
            object target = new ServiceImplementation();
            Type interfaceToProxy = typeof(IServiceContractGeneric<>);
            Assert.Throws<ArgumentException>(() => generator.CreateWcfProxy(interfaceToProxy, target));
        }

        [Fact]
        public void CreateWcfProxy_NullInterface()
        {
            var generator = new ServiceHostProxyGenerator();
            object target = new ServiceImplementation();
            Type interfaceToProxy = null;
            Assert.Throws<ArgumentNullException>(() => generator.CreateWcfProxy(interfaceToProxy, target));
        }

        [Fact]
        public void CreateWcfProxy_NullTarget()
        {
            var generator = new ServiceHostProxyGenerator();
            object target = null;
            Type interfaceToProxy = typeof(IServiceContract);
            Assert.Throws<ArgumentNullException>(() => generator.CreateWcfProxy(interfaceToProxy, target));
        }

        [Fact]
        public void CreateWcfProxy_TargetDoesNotImplementInterface()
        {
            var generator = new ServiceHostProxyGenerator();
            object target = new NotAServiceImplementation();
            Type interfaceToProxy = typeof(IServiceContract);
            Assert.Throws<ArgumentException>(() => generator.CreateWcfProxy(interfaceToProxy, target));
        }

        public interface INotAServiceContract
        {
            // Has to be public or Castle.DynamicProxy can't make a proxy.
        }

        [ServiceContract]
        public interface IServiceContract
        {
            // Has to be public or Castle.DynamicProxy can't make a proxy.
            void MethodToProxy();
        }

        [ServiceContract]
        public interface IServiceContractGeneric<T>
        {
            // Has to be public or Castle.DynamicProxy can't make a proxy.
            void MethodToProxy();
        }

        private class ServiceImplementation : IServiceContract
        {
            public bool ProxyMethodCalled { get; set; }

            public void MethodToProxy()
            {
                this.ProxyMethodCalled = true;
            }
        }

        private class NotAServiceImplementation : INotAServiceContract
        {
        }
    }
}
