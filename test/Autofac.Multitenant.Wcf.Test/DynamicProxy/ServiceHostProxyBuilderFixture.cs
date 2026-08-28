// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.ServiceModel;
using Autofac.Multitenant.Wcf.DynamicProxy;
using Xunit;

namespace Autofac.Multitenant.Wcf.Test.DynamicProxy;

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
    public void CreateWcfProxyType_CopiesMetadataBuddyClassAttributes()
    {
        var builder = new ServiceHostProxyBuilder();
        var type = builder.CreateWcfProxyType(typeof(IServiceContractWithMetadata));
        var behavior = Assert.Single(type.GetCustomAttributes(typeof(ServiceBehaviorAttribute), false));
        Assert.Equal("CustomServiceName", ((ServiceBehaviorAttribute)behavior).Name);
    }

    [Fact]
    public void CreateWcfProxyType_DoesNotCopyServiceContractAttribute()
    {
        // WCF refuses to host a class marked with ServiceContractAttribute
        // that also implements an interface marked with it.
        var builder = new ServiceHostProxyBuilder();
        var type = builder.CreateWcfProxyType(typeof(IServiceContract));
        Assert.Empty(type.GetCustomAttributes(typeof(ServiceContractAttribute), false));
    }

    [Fact]
    public void CreateWcfProxyType_ReplicatesOtherNonInheritableInterfaceAttributes()
    {
        // Releases before 7.0 suppressed every non-inheritable interface attribute.
        // Castle 5 removed that extension point, and its replacement opts out by
        // attribute type against a process-global list, so the opt-out is deliberately
        // limited to ServiceContractAttribute rather than growing with consumer types.
        var builder = new ServiceHostProxyBuilder();
        var type = builder.CreateWcfProxyType(typeof(IServiceContractWithMarker));
        Assert.Single(type.GetCustomAttributes(typeof(NonInheritableMarkerAttribute), false));
    }

    [Fact]
    public void CreateWcfProxyType_NullTypeToProxy()
    {
        var builder = new ServiceHostProxyBuilder();
        Assert.Throws<ArgumentNullException>(() => builder.CreateWcfProxyType(null!));
    }

    [Fact]
    public void CreateWcfProxyType_TypeIsGeneric()
    {
        var builder = new ServiceHostProxyBuilder();
        Assert.Throws<ArgumentException>(() => builder.CreateWcfProxyType(typeof(GenericType<>)));
    }

    [Fact]
    public void CreateWcfProxyType_TypeNotAccessible()
    {
        var builder = new ServiceHostProxyBuilder();
        Assert.Throws<ArgumentException>(() => builder.CreateWcfProxyType(typeof(PrivateType)));
    }

    [ServiceContract]
    public interface IServiceContract
    {
        // Has to be public or Castle.DynamicProxy can't make a proxy.
        [OperationContract]
        void MethodToProxy();
    }

    [ServiceContract]
    [ServiceMetadataType(typeof(ServiceMetadata))]
    public interface IServiceContractWithMetadata
    {
        // Has to be public or Castle.DynamicProxy can't make a proxy.
        [OperationContract]
        void MethodToProxy();
    }

    [ServiceContract]
    [NonInheritableMarker]
    public interface IServiceContractWithMarker
    {
        // Has to be public or Castle.DynamicProxy can't make a proxy.
        [OperationContract]
        void MethodToProxy();
    }

    private class PrivateType
    {
    }

    [AttributeUsage(AttributeTargets.Interface, Inherited = false)]
    public sealed class NonInheritableMarkerAttribute : Attribute
    {
    }

    public class GenericType<T>
    {
    }

    public class ValidType
    {
    }

    [ServiceBehavior(Name = "CustomServiceName")]
    public class ServiceMetadata
    {
    }
}
