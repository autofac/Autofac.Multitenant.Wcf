// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Autofac.Multitenant.Wcf.DynamicProxy
{
    /// <summary>
    /// Extension methods for the <see cref="Type"/> class.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Gets the metadata buddy class type, if any, as marked by a
        /// <see cref="ServiceMetadataTypeAttribute"/>.
        /// </summary>
        /// <param name="interfaceType">The service interface type from which to retrieve the metadata class.</param>
        /// <returns>
        /// The metadata type for the service interface as specified by a
        /// <see cref="ServiceMetadataTypeAttribute"/>,
        /// if it exists; otherwise <see langword="null" />.
        /// </returns>
        public static Type GetMetadataClassType(this Type interfaceType)
        {
            if (interfaceType == null)
            {
                throw new ArgumentNullException(nameof(interfaceType));
            }

            var attributes = (ServiceMetadataTypeAttribute[])interfaceType.GetCustomAttributes(typeof(ServiceMetadataTypeAttribute), false);
            return attributes.Length == 0 ? null : attributes[0].MetadataClassType;
        }
    }
}
