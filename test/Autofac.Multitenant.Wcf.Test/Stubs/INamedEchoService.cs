// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.ServiceModel;

namespace Autofac.Multitenant.Wcf.Test.Stubs;

/// <summary>
/// Service contract that names a metadata buddy class, so tests can assert the
/// buddy class attributes reach the generated host type and therefore the service
/// description WCF matches against XML configuration.
/// </summary>
[ServiceContract]
[ServiceMetadataType(typeof(NamedEchoServiceMetadata))]
public interface INamedEchoService
{
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
