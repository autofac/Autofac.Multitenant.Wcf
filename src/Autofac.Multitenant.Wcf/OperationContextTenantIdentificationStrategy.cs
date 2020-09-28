// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Linq;
using System.ServiceModel;

namespace Autofac.Multitenant.Wcf
{
    /// <summary>
    /// An <see cref="ITenantIdentificationStrategy"/>
    /// implementation that gets the tenant ID from a <see cref="TenantIdentificationContextExtension"/>
    /// attached to the current <see cref="OperationContext"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Use this <see cref="ITenantIdentificationStrategy"/>
    /// if you are using the <see cref="TenantIdentificationContextExtension"/>
    /// as the mechanism for tracking which tenant a given operation is running
    /// under.
    /// </para>
    /// <para>
    /// For example, you could use an <see cref="System.ServiceModel.Dispatcher.IDispatchMessageInspector"/>
    /// that gets the tenant ID from an incoming header and adds a
    /// <see cref="TenantIdentificationContextExtension"/>
    /// to the current <see cref="OperationContext"/> with
    /// the tenant ID value. Then you could register this provider as the
    /// mechanism for determining the tenant ID when resolving multitenant dependencies.
    /// </para>
    /// <para>
    /// The <see cref="TenantPropagationBehavior{TTenantId}"/>
    /// does exactly that - adds the tenant ID to outbound messages on the client
    /// and parses them on the service side. For a usage example, see
    /// <see cref="TenantPropagationBehavior{TTenantId}"/>.
    /// </para>
    /// </remarks>
    /// <seealso cref="TenantIdentificationContextExtension"/>
    /// <seealso cref="TenantPropagationBehavior{TTenantId}"/>
    public class OperationContextTenantIdentificationStrategy : ITenantIdentificationStrategy
    {
        /// <summary>
        /// Attempts to identify the tenant from the current operation context.
        /// </summary>
        /// <param name="tenantId">The current tenant identifier.</param>
        /// <returns>
        /// <see langword="true"/> if the tenant could be identified; <see langword="false"/>
        /// if not.
        /// </returns>
        /// <remarks>
        /// <para>
        /// The <paramref name="tenantId" /> will be the <see cref="object"/> value from the first
        /// <see cref="TenantIdentificationContextExtension"/>
        /// found on the current <see cref="OperationContext"/>,
        /// or <see langword="null" /> if there is no extension found on the
        /// operation context.
        /// </para>
        /// </remarks>
        public bool TryIdentifyTenant(out object tenantId)
        {
            tenantId = null;
            var context = OperationContext.Current;
            var extension = context?.Extensions.OfType<TenantIdentificationContextExtension>().FirstOrDefault();
            if (extension == null)
            {
                return false;
            }

            tenantId = extension.TenantId;
            return true;
        }
    }
}
