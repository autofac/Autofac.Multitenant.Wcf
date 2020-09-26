// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.ServiceModel;
using Autofac.Integration.Wcf;

namespace Autofac.Multitenant.Wcf
{
    /// <summary>
    /// Specifies the metadata class to associate with a service implementation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When hosting a service in a multitenant environment using <see cref="AutofacHostFactory"/>
    /// and the dynamic proxy generation that occurs therein, you can't really
    /// mark your "service implementation class" with metadata attributes like
    /// the <see cref="ServiceBehaviorAttribute"/>. Also,
    /// since many of these attributes require you to mark a class rather than
    /// the service interface, there's no way to otherwise specify them.
    /// </para>
    /// <para>
    /// This attribute works similar to the <c>System.ComponentModel.DataAnnotations.MetadataTypeAttribute</c>
    /// and allows you to mark a service interface such that the generated dynamic
    /// proxy class will use a "metadata buddy class" to retrieve various attributes
    /// that should be applied during generation.
    /// </para>
    /// <para>
    /// Mark your service interface with one of these attributes, then create an
    /// empty class with the <see cref="ServiceBehaviorAttribute"/>
    /// (or whatever) associated with it. The dynamic proxy generation will copy
    /// these attributes, thus allowing you to still make use of service metadata info.
    /// </para>
    /// <para>
    /// This is particularly handy when choosing to specify a service name. Since
    /// WCF usually infers the service name (which is also the name of the service
    /// element in configuration that it uses to set up the service host) from the
    /// type of the implementation, a dynamic proxy implementation results in
    /// service names like <c>Castle.Proxies.IMyServiceProxy_1</c>. To manually
    /// specify the name, you use the <see cref="ServiceBehaviorAttribute.Name"/>
    /// property, which can be associated with a service metadata buddy class.
    /// </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Interface)]
    public sealed class ServiceMetadataTypeAttribute : Attribute
    {
        /// <summary>
        /// Gets the metadata class type.
        /// </summary>
        /// <value>
        /// A <see cref="Type"/> indicating the class to be used to
        /// gather metadata for the service implementation proxy.
        /// </value>
        public Type MetadataClassType { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceMetadataTypeAttribute"/> class.
        /// </summary>
        /// <param name="metadataClassType">The metadata class type for specifying service metadata.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown if <paramref name="metadataClassType" /> is <see langword="null" />.
        /// </exception>
        public ServiceMetadataTypeAttribute(Type metadataClassType)
        {
            if (metadataClassType == null)
            {
                throw new ArgumentNullException(nameof(metadataClassType));
            }

            this.MetadataClassType = metadataClassType;
        }
    }
}
