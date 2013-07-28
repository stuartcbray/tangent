
using System;
using System.Net;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Webkit;
using TheFactorM.Federation;

namespace TheFactorM.Federation.Android.Activities
{
    /// <summary>
    /// Sign in activity for the application.
    /// </summary>
    [Activity(Label = "Sign in using provider", ScreenOrientation = ScreenOrientation.Portrait)]
    public partial class SignInActivity : Activity
    {
        #region Instance variables

        /// <summary>Web client used to implement the sign in process.</summary>
        private SignInWebViewClient _webViewClient;
		private WebView _signInLoginView;

        #endregion

        #region Overriden Activity abstracts

        /// <summary>
        /// Called when the activity is being created.
        /// </summary>
        /// <param name="bundle">Saved state.</param>
        protected override void OnCreate(Bundle bundle)
        {
            // Call base implementation
            base.OnCreate(bundle);
		
            _webViewClient = new SignInWebViewClient(Intent.GetStringExtra (Extras.AcsReplyToAddress));
            _webViewClient.UserSignedIn += UserSignedIn;
            _webViewClient.SignInOperationFailed += SignInOperationFailed;

			_signInLoginView = new WebView(this);

			SetContentView(_signInLoginView);

            // Load URL of Sign In page
            _signInLoginView.Settings.JavaScriptEnabled = true;
            _signInLoginView.SetWebViewClient(_webViewClient);
            _signInLoginView.LoadUrl(Intent.GetStringExtra(Extras.IdentityProviderLoginUri));
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Called when the sign in operation failed.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Arguments of the event.</param>
        private void SignInOperationFailed(object sender, EventArgs e)
        {
            // Set the result to Canceled and finish the activity
            SetResult(Result.Canceled);
            Finish();
        }

        /// <summary>
        /// Called when the sign in operation succeeded.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Arguments of the event.</param>
        private void UserSignedIn(object sender, EventArgs e)
        {
			Intent resultIntent = new Intent();
			resultIntent.PutExtra(Extras.ReceivedCookies,_webViewClient.ReceivedCookies);

			SetResult (Result.Ok,resultIntent);

			Finish ();
        }

        #endregion
    }
}