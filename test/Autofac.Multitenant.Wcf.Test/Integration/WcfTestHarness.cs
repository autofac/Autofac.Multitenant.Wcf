// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.ServiceModel;
using System.ServiceModel.Description;
using Autofac.Integration.Wcf;

namespace Autofac.Multitenant.Wcf.Test.Integration;

/// <summary>
/// Helpers for standing up a real multitenant WCF <see cref="ServiceHost"/> over a
/// <see cref="NetNamedPipeBinding"/>, invoking it through a client channel, and
/// tearing everything down.
/// </summary>
/// <remarks>
/// <para>
/// These helpers exercise the full WCF client/dispatcher/instance-provider
/// pipeline. Host construction works anywhere, but opening a host runs on Windows
/// only - Mono/macOS does not support named-pipe hosting.
/// </para>
/// <para>
/// Channels are created one at a time rather than reused because
/// <see cref="NetNamedPipeBinding"/> is sessionful: a channel is a session, a
/// session is an instance context, and an instance context is one tenant lifetime
/// scope. Routing a second tenant through an open channel would reuse the first
/// tenant's scope.
/// </para>
/// </remarks>
internal static class WcfTestHarness
{
    /// <summary>
    /// Creates a unique base address so concurrent or repeated runs do not
    /// collide on a pipe name.
    /// </summary>
    /// <returns>
    /// A unique <c>net.pipe</c> base address.
    /// </returns>
    public static Uri CreateBaseAddress()
        => new("net.pipe://localhost/autofac-multitenant-wcf-test/" + Guid.NewGuid().ToString("N"));

    /// <summary>
    /// Creates a multitenant service host for a service contract interface, wired
    /// the way the documentation prescribes: the host reads the tenant ID from
    /// inbound message headers into the operation context.
    /// </summary>
    /// <param name="contractType">
    /// The service contract interface to host. The multitenant data provider turns
    /// this into the concrete type WCF actually hosts.
    /// </param>
    /// <param name="baseAddress">
    /// The base address for the host.
    /// </param>
    /// <param name="tenantIdentificationStrategy">
    /// The strategy handed to the service-side propagation behavior.
    /// </param>
    /// <returns>
    /// An unopened host for <paramref name="contractType"/>.
    /// </returns>
    public static ServiceHost CreateHost(Type contractType, Uri baseAddress, ITenantIdentificationStrategy tenantIdentificationStrategy)
    {
        var host = (ServiceHost)new AutofacServiceHostFactory().CreateServiceHost(contractType.AssemblyQualifiedName!, new[] { baseAddress });
        host.Opening += (sender, args) => host.Description.Behaviors.Add(new TenantPropagationBehavior<string>(tenantIdentificationStrategy));
        return host;
    }

    /// <summary>
    /// Adds a named-pipe endpoint, opens the host, runs the given body against the
    /// endpoint address, then closes the host.
    /// </summary>
    /// <typeparam name="TContract">
    /// The service contract interface.
    /// </typeparam>
    /// <param name="serviceHost">
    /// The host to open. Closed by this method.
    /// </param>
    /// <param name="baseAddress">
    /// The base address the host was created with.
    /// </param>
    /// <param name="body">
    /// Receives the endpoint address of the open host.
    /// </param>
    public static void WithOpenHost<TContract>(ServiceHost serviceHost, Uri baseAddress, Action<Uri> body)
    {
        var address = new Uri(baseAddress, "service");
        serviceHost.AddServiceEndpoint(typeof(TContract), new NetNamedPipeBinding(), address);
        serviceHost.Open();

        try
        {
            body(address);
        }
        finally
        {
            serviceHost.Close();
        }
    }

    /// <summary>
    /// Creates a client channel, runs the given interaction, then closes the
    /// channel and factory.
    /// </summary>
    /// <typeparam name="TContract">
    /// The service contract interface.
    /// </typeparam>
    /// <param name="address">
    /// The endpoint address to connect to.
    /// </param>
    /// <param name="clientBehavior">
    /// An endpoint behavior to add to the client, or <see langword="null" /> for
    /// none. Pass a <see cref="TenantPropagationBehavior{TTenantId}"/> to send a
    /// tenant ID with the request.
    /// </param>
    /// <param name="act">
    /// The client interaction to run.
    /// </param>
    public static void Invoke<TContract>(Uri address, IEndpointBehavior? clientBehavior, Action<TContract> act)
        where TContract : class
    {
        var channelFactory = new ChannelFactory<TContract>(new NetNamedPipeBinding(), new EndpointAddress(address));
        try
        {
            if (clientBehavior != null)
            {
                channelFactory.Endpoint.EndpointBehaviors.Add(clientBehavior);
            }

            var channel = channelFactory.CreateChannel();
            var succeeded = false;
            try
            {
                act(channel);
                ((IClientChannel)(object)channel).Close();
                succeeded = true;
            }
            finally
            {
                if (!succeeded)
                {
                    ((IClientChannel)(object)channel).Abort();
                }
            }
        }
        finally
        {
            ((IDisposable)channelFactory).Dispose();
        }
    }

    /// <summary>
    /// Runs a test body with the given multitenant container and the multitenant
    /// data provider installed on <see cref="AutofacHostFactory"/>, restoring the
    /// previous state afterward.
    /// </summary>
    /// <param name="container">
    /// The multitenant container to install.
    /// </param>
    /// <param name="test">
    /// The test body to run.
    /// </param>
    public static void WithMultitenantContainer(MultitenantContainer container, Action test)
    {
        AutofacHostFactory.Container = container;
        AutofacHostFactory.ServiceImplementationDataProvider = new MultitenantServiceImplementationDataProvider();
        try
        {
            test();
        }
        finally
        {
            AutofacHostFactory.Container = null;
            AutofacHostFactory.ServiceImplementationDataProvider = null;
        }
    }
}
