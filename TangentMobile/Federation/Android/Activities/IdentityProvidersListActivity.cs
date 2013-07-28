using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using TheFactorM.Federation;
using TheFactorM.Federation.Android.Adapters;
using System;

namespace TheFactorM.Federation.Android.Activities
{
    /// <summary>
    /// Activity that allows the user to select the identity provider he/she wants to use to sign in to the application.
    /// </summary>
    [Activity(Label = "Select identity provider", ScreenOrientation = ScreenOrientation.Portrait)]
	public partial class IdentityProvidersListActivity : ListActivity
    {
        #region Private fields

        private SignInController _controller;
		string _acsReplyToAddress;
        private IdentityProviderListAdapter _listAdapter;
		private IEnumerable<IdentityProviderInformation> _identityProviders;

        #endregion

        #region Base class overrides

        /// <summary>
        /// Called when the activity is being created.
        /// </summary>
        /// <param name="bundle">Saved state.</param>
        protected override void OnCreate(Bundle bundle)
        {
            // Call base implementation
            base.OnCreate(bundle);

			// Get the settings from the caller
			string acsNamespace = Intent.GetStringExtra(Extras.AcsNamespace);
			string acsHomeRealm = Intent.GetStringExtra(Extras.AcsHomeRealm);
			_acsReplyToAddress = Intent.GetStringExtra (Extras.AcsReplyToAddress);

            // Initialize controller to get identity providers
			_controller = new SignInController(acsNamespace, acsHomeRealm,_acsReplyToAddress);
        }

		/// <summary>
		/// Handles the resume event.
		/// </summary>
		protected override void OnResume ()
		{
			base.OnResume ();

			// Load the identity providers from the server.
			_controller.IdentityProviderListAvailable += OnIdentityProvidersListAvailable;
			_controller.LoadIdentityProviders();
		}
	
        /// <summary>
        /// Called when a child activity has finished executing.
        /// </summary>
        /// <param name="requestCode">Code that was used to request the child activity.</param>
        /// <param name="resultCode">Result of the child activity.</param>
        /// <param name="data">Additional data provider by the child activity.</param>
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            // Call base implementation first
            base.OnActivityResult(requestCode, resultCode, data);

            // Make sure it was the SignIn activity that completed and that it completed succesfully
            if (requestCode == RequestCodes.SignInActivity)
            {
                if (resultCode == Result.Ok)
				{
					Intent resultIntent = new Intent();
					resultIntent.PutExtra (Extras.ReceivedCookies,data.GetStringExtra (Extras.ReceivedCookies));

                    SetResult(Result.Ok,resultIntent);
                    Finish();
                } else if(resultCode == Result.Canceled) {
					SetResult(Result.Canceled);
					Finish();
				}
            }
        }

        /// <summary>
        /// Called when an item from the list is selected.
        /// </summary>
        /// <param name="l">ListView that raised the event.</param>
        /// <param name="v">View that was selected.</param>
        /// <param name="position">Position of the item that was selected.</param>
        /// <param name="id">Id of the item that was selected.</param>
        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {
            // Call base implementation
            base.OnListItemClick(l, v, position, id);

            // Use the controller to perform the sign in
			Intent intent = new Intent(this,typeof(SignInActivity));
			intent.PutExtra(Extras.IdentityProviderLoginUri,_listAdapter[position].LoginUrl);
			intent.PutExtra(Extras.AcsReplyToAddress,_acsReplyToAddress);

			StartActivityForResult(intent,RequestCodes.SignInActivity);
        }

        #endregion

		#region Private methods

		void OnIdentityProvidersListAvailable (object sender, GetIdentityProviderListEventArgs e)
		{
			this.RunOnUiThread(() => {
				_identityProviders = e.Result;
				_listAdapter = new IdentityProviderListAdapter(this,_identityProviders);
				
				this.ListAdapter = _listAdapter;
			});
		}

		#endregion
    }
}