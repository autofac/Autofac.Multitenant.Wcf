// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Autofac.Multitenant.Wcf.Test.Stubs;

/// <summary>
/// Tenant-specific implementation registered as an override for tenant 2.
/// </summary>
public class Tenant2EchoService : MultitenantEchoServiceBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Tenant2EchoService"/> class.
    /// </summary>
    /// <param name="dependency">
    /// The injected dependency.
    /// </param>
    public Tenant2EchoService(TrackedDependency dependency)
        : base(dependency)
    {
    }

    /// <inheritdoc/>
    public override string TenantName => "tenant2";
}
