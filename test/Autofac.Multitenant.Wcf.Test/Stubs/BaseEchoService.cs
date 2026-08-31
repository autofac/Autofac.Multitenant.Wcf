// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Autofac.Multitenant.Wcf.Test.Stubs;

/// <summary>
/// Application-level implementation, registered in the application container and
/// used by any tenant that does not override the contract.
/// </summary>
public class BaseEchoService : MultitenantEchoServiceBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseEchoService"/> class.
    /// </summary>
    /// <param name="dependency">
    /// The injected dependency.
    /// </param>
    public BaseEchoService(TrackedDependency dependency)
        : base(dependency)
    {
    }

    /// <inheritdoc/>
    public override string TenantName => "base";
}
