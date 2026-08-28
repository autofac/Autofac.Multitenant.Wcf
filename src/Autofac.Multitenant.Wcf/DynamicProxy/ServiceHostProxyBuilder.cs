// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Security;
using System.ServiceModel;
using Castle.DynamicProxy;
using Castle.DynamicProxy.Generators;

namespace Autofac.Multitenant.Wcf.DynamicProxy;

/// <summary>
/// Proxy builder that has an additional method to create proxies usable
/// in WCF multitenant hosting.
/// </summary>
/// <remarks>
/// <para>
/// The primary point of interest in this builder type is the
/// <see cref="CreateWcfProxyType"/>
/// method, which is used to create an interface proxy type that is hostable
/// by WCF.
/// </para>
/// </remarks>
[SecurityCritical]
public class ServiceHostProxyBuilder : DefaultProxyBuilder
{
    /// <summary>
    /// Initializes static members of the <see cref="ServiceHostProxyBuilder"/> class.
    /// </summary>
    /// <remarks>
    /// <para>
    /// WCF refuses to host a class marked with <see cref="ServiceContractAttribute"/>
    /// that also implements an interface marked with it, so the generated proxy
    /// must not receive a copy of the attribute from the contract interface.
    /// Castle's opt-out list is the only supported way to suppress that copy, and
    /// it is process-wide: no proxy generated anywhere in this process will carry
    /// a replicated <see cref="ServiceContractAttribute"/>.
    /// </para>
    /// </remarks>
    static ServiceHostProxyBuilder()
    {
        AttributesToAvoidReplicating.Add<ServiceContractAttribute>();
    }

    /// <summary>
    /// Creates an interface proxy type that can be used by the WCF host.
    /// </summary>
    /// <param name="interfaceToProxy">The service interface to proxy.</param>
    /// <returns>
    /// A <see cref="Type"/> that is a proxy for the interface specified
    /// by <paramref name="interfaceToProxy" /> that will be able to be
    /// hosted by WCF.
    /// </returns>
    /// <remarks>
    /// <para>
    /// As this is a very specialized proxy type, it does not take options like
    /// other proxy types - the generation options are derived from the service
    /// interface itself.
    /// </para>
    /// <para>
    /// The generated type receives the class-level attributes of the metadata buddy
    /// class named by <see cref="ServiceMetadataTypeAttribute"/>. It also receives any
    /// non-inheritable attributes Castle replicates from the contract interface, which
    /// releases before 7.0 suppressed wholesale - Castle 5 removed the extension point
    /// that allowed that. In practice only consumer-declared attributes are affected,
    /// since the WCF contract attributes other than <see cref="ServiceContractAttribute"/>
    /// are inheritable and were never replicated.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="interfaceToProxy" /> is <see langword="null" />.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown by Castle if <paramref name="interfaceToProxy" /> is an open generic
    /// or is not accessible to the proxy generator. Prior to Castle.Core 5 this was
    /// a <c>GeneratorException</c>, which Castle has since removed.
    /// </exception>
    public virtual Type CreateWcfProxyType(Type interfaceToProxy)
    {
        if (interfaceToProxy == null)
        {
            throw new ArgumentNullException(nameof(interfaceToProxy));
        }

        return this.CreateInterfaceProxyTypeWithTargetInterface(interfaceToProxy, Type.EmptyTypes, CreateProxyGenerationOptions(interfaceToProxy));
    }

    /// <summary>
    /// Builds the generation options used to emit the WCF hosting proxy type.
    /// </summary>
    /// <param name="interfaceToProxy">The service interface to proxy.</param>
    /// <returns>
    /// Options that replicate the class-level attributes of the metadata buddy
    /// class, if the service interface names one with a
    /// <see cref="ServiceMetadataTypeAttribute"/>.
    /// </returns>
    private static ProxyGenerationOptions CreateProxyGenerationOptions(Type interfaceToProxy)
    {
        var options = new ProxyGenerationOptions();
        var metadataBuddyType = interfaceToProxy.GetMetadataClassType();
        if (metadataBuddyType != null)
        {
            foreach (var attribute in metadataBuddyType.GetCustomAttributesData())
            {
                options.AdditionalAttributes.Add(attribute.ToCustomAttributeInfo());
            }
        }

        return options;
    }
}
