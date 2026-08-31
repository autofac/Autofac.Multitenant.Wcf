// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.ServiceModel;
using Autofac.Multitenant.Wcf.Test.Stubs;
using Xunit;

namespace Autofac.Multitenant.Wcf.Test.Integration;

/// <summary>
/// Asserts the shape of the type a multitenant host is built on and the
/// description WCF derives from it.
/// </summary>
/// <remarks>
/// <para>
/// These only construct a host, never open one, because WCF reads the hosted type's
/// attributes in the <see cref="ServiceHost"/> constructor. That is also why nothing
/// downstream - <c>HostConfigurationAction</c> included - can correct them: by the
/// time a caller holds the host, configuration has already been matched.
/// </para>
/// <para>
/// Assertions against <c>Description.ConfigurationName</c> are Windows-only; Mono
/// leaves that property null.
/// </para>
/// </remarks>
[Collection("WcfHosting")]
public class ServiceDescriptionFixture
{
    [Fact]
    public void HostedType_ImplementsContractWithoutDeclaringIt()
    {
        // The entire reason this package generates a type rather than handing WCF
        // the interface: WCF rejects a class that both declares and inherits a
        // service contract.
        WithHost(typeof(IMultitenantEchoService), host =>
        {
            var hostedType = host.Description.ServiceType!;
            Assert.Contains(typeof(IMultitenantEchoService), hostedType.GetInterfaces());
            Assert.Empty(hostedType.GetCustomAttributes(typeof(ServiceContractAttribute), false));
        });
    }

    [Fact]
    public void MetadataBuddyClass_AttributesLandOnHostedType()
    {
        WithHost(typeof(INamedEchoService), host =>
        {
            var behavior = Assert.Single(host.Description.ServiceType!.GetCustomAttributes(typeof(ServiceBehaviorAttribute), false));
            Assert.Equal("NamedEchoService", ((ServiceBehaviorAttribute)behavior).Name);
            Assert.Equal(typeof(INamedEchoService).FullName, ((ServiceBehaviorAttribute)behavior).ConfigurationName);
        });
    }

    [Fact]
    public void MetadataBuddyClass_SetsServiceName()
    {
        WithHost(typeof(INamedEchoService), host => Assert.Equal("NamedEchoService", host.Description.Name));
    }

    [WindowsFact]
    public void MetadataBuddyClass_SetsConfigurationName()
    {
        WithHost(typeof(INamedEchoService), host => Assert.Equal(typeof(INamedEchoService).FullName, host.Description.ConfigurationName));
    }

    [WindowsFact]
    public void ContractWithoutMetadataBuddyClass_FallsBackToGeneratedTypeName()
    {
        // Documents the cost of not using a buddy class: configuration has to name
        // the generated type.
        WithHost(typeof(IMultitenantEchoService), host => Assert.Equal(host.Description.ServiceType!.FullName, host.Description.ConfigurationName));
    }

    private static void WithHost(Type contractType, Action<ServiceHost> assert)
    {
        var builder = new ContainerBuilder();
        using var container = new MultitenantContainer(new OperationContextTenantIdentificationStrategy(), builder.Build());
        var baseAddress = WcfTestHarness.CreateBaseAddress();
        WcfTestHarness.WithMultitenantContainer(container, () =>
        {
            using var host = WcfTestHarness.CreateHost(contractType, baseAddress, new OperationContextTenantIdentificationStrategy());
            assert(host);
        });
    }
}
