// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.ServiceModel;

namespace Autofac.Multitenant.Wcf.Test.Stubs;

/// <summary>
/// Metadata buddy class for <see cref="INamedEchoService"/>. Setting
/// <see cref="ServiceBehaviorAttribute.ConfigurationName"/> is the documented
/// reason this feature exists: without it the generated host type's name is what
/// the <c>&lt;service&gt;</c> element in configuration has to match.
/// </summary>
[ServiceBehavior(Name = "NamedEchoService", ConfigurationName = "Autofac.Multitenant.Wcf.Test.Stubs.INamedEchoService")]
public class NamedEchoServiceMetadata
{
}
