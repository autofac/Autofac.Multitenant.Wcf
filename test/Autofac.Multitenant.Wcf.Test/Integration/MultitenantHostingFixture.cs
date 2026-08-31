// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Autofac.Multitenant.Wcf.Test.Stubs;
using Xunit;

namespace Autofac.Multitenant.Wcf.Test.Integration;

/// <summary>
/// Opens a real multitenant WCF host and pushes messages through the full
/// pipeline, verifying that the tenant ID travels from client to service and
/// selects the tenant's implementation.
/// </summary>
[Collection("WcfHosting")]
public class MultitenantHostingFixture
{
    [WindowsFact]
    public void NoTenantHeader_ServesApplicationImplementation()
    {
        var activity = new DependencyActivity();
        using var container = CreateContainer(activity);
        var baseAddress = WcfTestHarness.CreateBaseAddress();

        WcfTestHarness.WithMultitenantContainer(container, () =>
        {
            var host = WcfTestHarness.CreateHost(typeof(IMultitenantEchoService), baseAddress, new OperationContextTenantIdentificationStrategy());
            WcfTestHarness.WithOpenHost<IMultitenantEchoService>(host, baseAddress, address =>
            {
                // No propagation behavior on the client, so no tenant header at all.
                WcfTestHarness.Invoke<IMultitenantEchoService>(address, null, channel =>
                {
                    Assert.Equal("base", channel.GetTenantName());
                    Assert.Equal("hi", channel.Echo("hi"));
                });
            });
        });
    }

    [WindowsFact]
    public void TenantHeader_RoutesEachTenantToItsOwnImplementation()
    {
        var activity = new DependencyActivity();
        using var container = CreateContainer(activity);
        var baseAddress = WcfTestHarness.CreateBaseAddress();

        WcfTestHarness.WithMultitenantContainer(container, () =>
        {
            var host = WcfTestHarness.CreateHost(typeof(IMultitenantEchoService), baseAddress, new OperationContextTenantIdentificationStrategy());
            WcfTestHarness.WithOpenHost<IMultitenantEchoService>(host, baseAddress, address =>
            {
                Assert.Equal("tenant1", GetTenantName(address, "1"));
                Assert.Equal("tenant2", GetTenantName(address, "2"));

                // A null tenant ID is the documented way to mean "default tenant".
                Assert.Equal("base", GetTenantName(address, null));
            });
        });
    }

    [WindowsFact]
    public void UnconfiguredTenant_FallsBackToApplicationImplementation()
    {
        var activity = new DependencyActivity();
        using var container = CreateContainer(activity);
        var baseAddress = WcfTestHarness.CreateBaseAddress();

        WcfTestHarness.WithMultitenantContainer(container, () =>
        {
            var host = WcfTestHarness.CreateHost(typeof(IMultitenantEchoService), baseAddress, new OperationContextTenantIdentificationStrategy());
            WcfTestHarness.WithOpenHost<IMultitenantEchoService>(host, baseAddress, address =>
            {
                Assert.Equal("base", GetTenantName(address, "no-such-tenant"));
            });
        });
    }

    [WindowsFact]
    public void TenantScopedDependency_IsInjectedAndReleased()
    {
        var activity = new DependencyActivity();
        using var container = CreateContainer(activity);
        var baseAddress = WcfTestHarness.CreateBaseAddress();
        var dependencyIds = new List<string>();

        WcfTestHarness.WithMultitenantContainer(container, () =>
        {
            var host = WcfTestHarness.CreateHost(typeof(IMultitenantEchoService), baseAddress, new OperationContextTenantIdentificationStrategy());
            WcfTestHarness.WithOpenHost<IMultitenantEchoService>(host, baseAddress, address =>
            {
                WcfTestHarness.Invoke<IMultitenantEchoService>(address, TenantBehavior("1"), channel =>
                {
                    dependencyIds.Add(channel.GetDependencyId());
                    dependencyIds.Add(channel.GetDependencyId());
                });
            });
        });

        // Asserted after the host closes so every instance context has ended.
        Assert.NotEmpty(dependencyIds);
        Assert.All(dependencyIds, id => Assert.True(activity.WasDisposed(id)));
        Assert.Equal(activity.CreatedCount, activity.DisposedCount);
    }

    [WindowsFact]
    public void SeparateSessions_GetSeparateServiceInstances()
    {
        var activity = new DependencyActivity();
        using var container = CreateContainer(activity);
        var baseAddress = WcfTestHarness.CreateBaseAddress();

        WcfTestHarness.WithMultitenantContainer(container, () =>
        {
            var host = WcfTestHarness.CreateHost(typeof(IMultitenantEchoService), baseAddress, new OperationContextTenantIdentificationStrategy());
            WcfTestHarness.WithOpenHost<IMultitenantEchoService>(host, baseAddress, address =>
            {
                Assert.NotEqual(GetInstanceId(address, "1"), GetInstanceId(address, "1"));
            });
        });
    }

    private static MultitenantContainer CreateContainer(DependencyActivity activity)
    {
        var builder = new ContainerBuilder();
        builder.RegisterInstance(activity);
        builder.RegisterType<TrackedDependency>().InstancePerLifetimeScope();
        builder.RegisterType<BaseEchoService>().As<IMultitenantEchoService>();

        var container = new MultitenantContainer(new OperationContextTenantIdentificationStrategy(), builder.Build());
        container.ConfigureTenant("1", b => b.RegisterType<Tenant1EchoService>().As<IMultitenantEchoService>());
        container.ConfigureTenant("2", b => b.RegisterType<Tenant2EchoService>().As<IMultitenantEchoService>());
        return container;
    }

    private static TenantPropagationBehavior<string> TenantBehavior(string? tenantId)
        => new(new StubTenantIdentificationStrategy { TenantId = tenantId });

    private static string GetTenantName(Uri address, string? tenantId)
    {
        string? tenantName = null;
        WcfTestHarness.Invoke<IMultitenantEchoService>(address, TenantBehavior(tenantId), channel => tenantName = channel.GetTenantName());
        return tenantName!;
    }

    private static string GetInstanceId(Uri address, string? tenantId)
    {
        string? instanceId = null;
        WcfTestHarness.Invoke<IMultitenantEchoService>(address, TenantBehavior(tenantId), channel => instanceId = channel.GetInstanceId());
        return instanceId!;
    }
}
