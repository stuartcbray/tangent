using System;
using System.Collections.Generic;
using System.Text;
using TheFactorM.Federation;
using System.Diagnostics;


namespace TheFactorM.Federation
{
    /// <summary>
    /// Controller that provides functionality to logon the current user
    /// </summary>
    public class SignInController
    {
        #region Private fields
		
		private const string ACS_URI_TEMPLATE = "https://{0}.accesscontrol.windows.net:443/v2/metadata/IdentityProviders.js?protocol=wsfederation&realm={1}%2f&reply_to={2}&version=1.0";
        private JSONIdentityProviderDiscoveryClient _identityProviderDiscoveryClient;
		private string _acsNamespace;
		private string _homeRealm;
		private string _replyToAddress;

        #endregion

		#region Events

		/// <summary>
		/// Occurs when identity provider list is available for display.
		/// </summary>
		public event EventHandler<GetIdentityProviderListEventArgs> IdentityProviderListAvailable;

		/// <summary>
		/// Occurs when there is an error in the sign in process.
		/// The handler should display the error to the user.
		/// </summary>
		public event EventHandler<SignInErrorEventArgs> SignInError;

		#endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the SignInController class.
        /// </summary>
        /// <param name="acsNamespace">Namespace of the ACS</param>
        /// <param name="homeRealm">Home realm of the RP application</param>
		/// <param name="replyToAddress">The address the user must be redirected to after sign in</param>
        public SignInController(string acsNamespace, string homeRealm, string replyToAddress)
        {
			_acsNamespace = Uri.EscapeDataString(acsNamespace);
			_homeRealm = Uri.EscapeDataString(homeRealm);
			_replyToAddress = Uri.EscapeDataString(replyToAddress);
            _identityProviderDiscoveryClient = ServiceFactory.CreateIdentityProviderDiscoveryService();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Loads the identity providers available to the application
        /// </summary>
        public void LoadIdentityProviders()
        {
			string uri = string.Format(ACS_URI_TEMPLATE, _acsNamespace, _homeRealm, _replyToAddress);
            Uri identityProviderDiscoveryEndpoint = new Uri(uri, UriKind.RelativeOrAbsolute);			

            _identityProviderDiscoveryClient.GetIdentityProviderListCompleted += OnGetIdentityProvidersCompleted;
            _identityProviderDiscoveryClient.GetIdentityProviderListAsync(identityProviderDiscoveryEndpoint);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Callback for the GetIdentityProvidersList method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnGetIdentityProvidersCompleted(object sender, GetIdentityProviderListEventArgs e)
        {
			DeviceContext.Current.HideNetworkActivityIndicator();

			if (e.Error != null) {
				Console.WriteLine ("Error retrieving IdentityProviders list: " + e.Error);
				if (SignInError != null) {
					SignInError (this, new SignInErrorEventArgs (e.Error));
				}
			} else {
				if (IdentityProviderListAvailable != null) {
					IdentityProviderListAvailable (this, e);
				}
			}
        }

        #endregion
    }
}
