// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Autofac.Multitenant.Wcf.Test.Stubs;

/// <summary>
/// Tenant-specific implementation registered as an override for tenant 1.
/// </summary>
public class Tenant1EchoService : MultitenantEchoServiceBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Tenant1EchoService"/> class.
    /// </summary>
    /// <param name="dependency">
    /// The injected dependency.
    /// </param>
    public Tenant1EchoService(TrackedDependency dependency)
        : base(dependency)
    {
    }

    /// <inheritdoc/>
    public override string TenantName => "tenant1";
}
