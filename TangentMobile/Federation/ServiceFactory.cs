using System;
using System.ServiceModel;
using TheFactorM.Federation;

namespace TheFactorM.Federation
{
    /// <summary>
    /// Produces new instances of the necessary service agents
    /// </summary>
    public partial class ServiceFactory
    {
        /// <summary>
        /// Creates a new instance of the JSON identity provider discovery service
        /// </summary>
        /// <returns></returns>
        public static JSONIdentityProviderDiscoveryClient CreateIdentityProviderDiscoveryService()
        {
            return new JSONIdentityProviderDiscoveryClient();
        }
	}
}