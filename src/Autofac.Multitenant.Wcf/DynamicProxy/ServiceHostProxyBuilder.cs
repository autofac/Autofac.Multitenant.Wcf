// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Globalization;
using System.Security;
using Autofac.Multitenant.Wcf.Properties;
using Castle.DynamicProxy;
using Castle.DynamicProxy.Generators;

namespace Autofac.Multitenant.Wcf.DynamicProxy
{
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
        /// Validates that the target type to proxy is visible and not generic.
        /// </summary>
        /// <param name="target">
        /// The interface type to proxy.
        /// </param>
        private static void AssertValidType(Type target)
        {
            // This is copied from the DefaultProxyBuilder because we need to
            // validate types but the validation logic is not accessible.
            var isTargetNested = target.IsNested;
            var isNestedAndInternal = isTargetNested && (target.IsNestedAssembly || target.IsNestedFamORAssem);
            var isInternalNotNested = target.IsVisible == false && isTargetNested == false;

            var internalAndVisibleToDynProxy = (isInternalNotNested || isNestedAndInternal) && ProxyUtil.IsAccessible(target);
            var isAccessible = target.IsPublic || target.IsNestedPublic || internalAndVisibleToDynProxy;

            if (!isAccessible)
            {
                throw new GeneratorException(string.Format(CultureInfo.CurrentCulture, Resources.DynamicProxy_InterfaceTypeToProxyNotPublic, target.FullName));
            }

            if (target.IsGenericTypeDefinition)
            {
                throw new GeneratorException(string.Format(CultureInfo.CurrentCulture, Resources.DynamicProxy_InterfaceTypeToProxyIsGeneric, target.FullName));
            }
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
        /// This proxy type creation method uses the
        /// <see cref="ServiceHostInterfaceProxyGenerator"/>
        /// to create the service host proxy type. As this is a very specialized
        /// proxy type, it does not take options like other proxy types.
        /// </para>
        /// </remarks>
        /// <seealso cref="ServiceHostInterfaceProxyGenerator" />
        public virtual Type CreateWcfProxyType(Type interfaceToProxy)
        {
            if (interfaceToProxy == null)
            {
                throw new ArgumentNullException(nameof(interfaceToProxy));
            }

            AssertValidType(interfaceToProxy);
            var generator = new ServiceHostInterfaceProxyGenerator(ModuleScope, interfaceToProxy) { Logger = Logger };
            return generator.GenerateCode(interfaceToProxy, null, ProxyGenerationOptions.Default);
        }
    }
}
