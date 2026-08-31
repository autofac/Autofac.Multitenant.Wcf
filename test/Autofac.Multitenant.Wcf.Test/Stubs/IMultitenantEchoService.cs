// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.ServiceModel;

namespace Autofac.Multitenant.Wcf.Test.Stubs;

/// <summary>
/// Service contract for multitenant hosting tests. Operations expose enough state
/// to assert which tenant's implementation answered, whether successive calls hit
/// the same instance, and whether dependencies were injected and released.
/// </summary>
[ServiceContract]
public interface IMultitenantEchoService
{
    /// <summary>
    /// Returns the ID of the injected dependency, proving constructor injection
    /// reached the live service instance.
    /// </summary>
    /// <returns>
    /// The injected dependency's identifier.
    /// </returns>
    [OperationContract]
    string GetDependencyId();

    /// <summary>
    /// Returns a per-instance identifier so callers can tell whether successive
    /// calls hit the same service instance.
    /// </summary>
    /// <returns>
    /// The service instance identifier.
    /// </returns>
    [OperationContract]
    string GetInstanceId();

    /// <summary>
    /// Returns the name of the implementation that answered, identifying the tenant
    /// the call was routed to.
    /// </summary>
    /// <returns>
    /// The answering implementation's tenant name.
    /// </returns>
    [OperationContract]
    string GetTenantName();

    /// <summary>
    /// Echoes the supplied message back to the caller.
    /// </summary>
    /// <param name="message">
    /// The message to echo.
    /// </param>
    /// <returns>
    /// The same message.
    /// </returns>
    [OperationContract]
    string Echo(string message);
}
