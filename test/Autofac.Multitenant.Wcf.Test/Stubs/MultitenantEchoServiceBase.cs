// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Threading;

namespace Autofac.Multitenant.Wcf.Test.Stubs;

/// <summary>
/// Base <see cref="IMultitenantEchoService"/> implementation that takes a
/// <see cref="TrackedDependency"/> so tests can assert constructor injection and
/// lifetime. Each instance gets a unique ID.
/// </summary>
public abstract class MultitenantEchoServiceBase : IMultitenantEchoService
{
    private static int _instanceCounter;

    private readonly TrackedDependency _dependency;
    private readonly string _instanceId;

    /// <summary>
    /// Initializes a new instance of the <see cref="MultitenantEchoServiceBase"/> class.
    /// </summary>
    /// <param name="dependency">
    /// The injected dependency whose ID is exposed via <see cref="GetDependencyId"/>.
    /// </param>
    protected MultitenantEchoServiceBase(TrackedDependency dependency)
    {
        _dependency = dependency ?? throw new ArgumentNullException(nameof(dependency));
        _instanceId = "svc-" + Interlocked.Increment(ref _instanceCounter);
    }

    /// <summary>
    /// Gets the tenant name reported by this implementation.
    /// </summary>
    public abstract string TenantName
    {
        get;
    }

    /// <inheritdoc/>
    public string GetDependencyId() => _dependency.Id;

    /// <inheritdoc/>
    public string GetInstanceId() => _instanceId;

    /// <inheritdoc/>
    public string GetTenantName() => TenantName;

    /// <inheritdoc/>
    public string Echo(string message) => message;
}
