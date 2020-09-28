// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.ServiceModel;
using Xunit;

namespace Autofac.Multitenant.Wcf.Test
{
    public class MultitenantServiceImplementationDataProviderFixture
    {
        [Fact]
        public void GetServiceImplementationData_ConstructorStringDoesNotResolveToType()
        {
            var provider = new MultitenantServiceImplementationDataProvider();
            Assert.Throws<InvalidOperationException>(() => provider.GetServiceImplementationData("This is not a type name."));
        }

        [Fact]
        public void GetServiceImplementationData_ConstructorStringDoesNotResolveToInterface()
        {
            var provider = new MultitenantServiceImplementationDataProvider();
            Assert.Throws<InvalidOperationException>(() => provider.GetServiceImplementationData(typeof(object).AssemblyQualifiedName));
        }

        [Fact]
        public void GetServiceImplementationData_ConstructorStringDoesNotResolveToServiceContract()
        {
            var provider = new MultitenantServiceImplementationDataProvider();
            Assert.Throws<InvalidOperationException>(() => provider.GetServiceImplementationData(typeof(INotAServiceContract).AssemblyQualifiedName));
        }

        [Fact]
        public void GetServiceImplementationData_DataReturnedBasedOnProxy()
        {
            var provider = new MultitenantServiceImplementationDataProvider();
            var data = provider.GetServiceImplementationData(typeof(IServiceContract).AssemblyQualifiedName);
            Assert.Equal(typeof(IServiceContract).AssemblyQualifiedName, data.ConstructorString);
            Assert.Single(data.ServiceTypeToHost.FindInterfaces((f, o) => f == (Type)o, typeof(IServiceContract)));
        }

        [Fact]
        public void GetServiceImplementationData_DataResolvesTypeThroughProxy()
        {
            var builder = new ContainerBuilder();
            var implementation = new ServiceImplementation();
            builder.RegisterInstance(implementation).As<IServiceContract>();
            var container = builder.Build();
            var provider = new MultitenantServiceImplementationDataProvider();
            var data = provider.GetServiceImplementationData(typeof(IServiceContract).AssemblyQualifiedName);
            var resolved = data.ImplementationResolver(container.BeginLifetimeScope());
            Assert.NotNull(resolved);
            ((IServiceContract)resolved).MethodToProxy();
            Assert.True(implementation.ProxyMethodCalled);
        }

        [Fact]
        public void GetServiceImplementationData_EmptyConstructorString()
        {
            var provider = new MultitenantServiceImplementationDataProvider();
            Assert.Throws<ArgumentException>(() => provider.GetServiceImplementationData(""));
        }

        [Fact]
        public void GetServiceImplementationData_NullConstructorString()
        {
            var provider = new MultitenantServiceImplementationDataProvider();
            Assert.Throws<ArgumentNullException>(() => provider.GetServiceImplementationData(null));
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

        private class ServiceImplementation : IServiceContract
        {
            public bool ProxyMethodCalled { get; set; }

            public void MethodToProxy()
            {
                this.ProxyMethodCalled = true;
            }
        }
    }
}
