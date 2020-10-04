// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Globalization;
using System.Security;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Autofac.Multitenant.Wcf.Properties;
using Castle.DynamicProxy;

namespace Autofac.Multitenant.Wcf.DynamicProxy
{
    /// <summary>
    /// Proxy generator used in multitenant service hosting.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The WCF service host has very specific requirements around the object type that
    /// you pass in when you call <see cref="ServiceHostFactory.CreateServiceHost(Type,Uri[])"/>.
    /// </para>
    /// <para>
    /// If you have a type that has a <see cref="ServiceContractAttribute"/>
    /// on it and it implements an interface that has <see cref="ServiceContractAttribute"/>
    /// on it, the WCF service host complains that you can't have two different
    /// service contracts.
    /// </para>
    /// <para>
    /// The proxy generator uses a <see cref="ServiceHostProxyBuilder"/>
    /// to build the proxy types. This is specifically interesting in the
    /// <see cref="CreateWcfProxy"/> method, which uses some special overrides
    /// and additions in the builder.
    /// </para>
    /// <para>
    /// The builder, when called through <see cref="CreateWcfProxy"/>,
    /// generates proxy types that ignore non-inherited
    /// attributes on the service interface (e.g.,
    /// <see cref="ServiceContractAttribute"/>)
    /// so when the proxy type is generated, it doesn't bring over anything
    /// that will cause WCF host initialization to fail or get confused.
    /// </para>
    /// </remarks>
    [SecurityCritical]
    public class ServiceHostProxyGenerator : ProxyGenerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceHostProxyGenerator"/> class.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The proxy generator uses a <see cref="ServiceHostProxyBuilder"/>
        /// to build the proxy types.
        /// </para>
        /// </remarks>
        public ServiceHostProxyGenerator()
            : base(new ServiceHostProxyBuilder())
        {
        }

        /// <summary>
        /// Creates a proxy object that can be used by the WCF service host.
        /// </summary>
        /// <param name="interfaceToProxy">
        /// The WCF service interface for service implementations.
        /// </param>
        /// <param name="target">
        /// The target of the proxy object that will receive the actual calls.
        /// </param>
        /// <returns>
        /// An object that implements the interface <paramref name="interfaceToProxy" />
        /// and proxies calls to the <paramref name="target" />.
        /// </returns>
        /// <remarks>
        /// <para>
        /// When initializing the service host, call this method with a dummy
        /// <paramref name="target" /> object, just to create the dynamic proxy
        /// type for the first time and get the service host up and running.
        /// Subsequent proxies for that interface should have a valid target
        /// implementation type to which service calls will be proxied.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="interfaceToProxy" /> or <paramref name="target" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para>
        /// Thrown if:
        /// </para>
        /// <list type="bullet">
        /// <item>
        /// <term><paramref name="interfaceToProxy" /> is not an interface.</term>
        /// </item>
        /// <item>
        /// <term><paramref name="interfaceToProxy" /> is an open generic.</term>
        /// </item>
        /// <item>
        /// <term><paramref name="target" /> cannot be cast to <paramref name="interfaceToProxy" />.</term>
        /// </item>
        /// </list>
        /// </exception>
        public object CreateWcfProxy(Type interfaceToProxy, object target)
        {
            if (interfaceToProxy == null)
            {
                throw new ArgumentNullException(nameof(interfaceToProxy));
            }

            if (!interfaceToProxy.IsInterface)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.DynamicProxy_InterfaceTypeToProxyNotInterface, interfaceToProxy.FullName), nameof(interfaceToProxy));
            }

            if (interfaceToProxy.IsGenericTypeDefinition)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.DynamicProxy_InterfaceTypeToProxyIsGeneric, interfaceToProxy.FullName), nameof(interfaceToProxy));
            }

            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (!interfaceToProxy.IsInstanceOfType(target))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.DynamicProxy_ProxyTargetDoesNotImplementInterface, target.GetType().FullName, interfaceToProxy.FullName), nameof(target));
            }

            if (interfaceToProxy.GetCustomAttributes(typeof(ServiceContractAttribute), false).Length == 0)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.DynamicProxy_InterfaceTypeToProxyNotServiceContract, interfaceToProxy.FullName));
            }

            Type type = this.CreateWcfProxyType(interfaceToProxy);
            return Activator.CreateInstance(type, Array.Empty<IInterceptor>(), target);
        }

        /// <summary>
        /// Creates the WCF service interface proxy type or retrieves it from cache.
        /// </summary>
        /// <param name="interfaceToProxy">
        /// The interface type that will be proxied.
        /// </param>
        /// <returns>
        /// A generated proxy type that can be used to proxy calls to actual
        /// service implementations.
        /// </returns>
        protected virtual Type CreateWcfProxyType(Type interfaceToProxy)
        {
            return ((ServiceHostProxyBuilder)this.ProxyBuilder).CreateWcfProxyType(interfaceToProxy);
        }
    }
}
