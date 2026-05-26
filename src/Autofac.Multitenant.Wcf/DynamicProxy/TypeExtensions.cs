// <copyright file="TypeExtensions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Autofac.Multitenant.Wcf.DynamicProxy;

using System;

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
