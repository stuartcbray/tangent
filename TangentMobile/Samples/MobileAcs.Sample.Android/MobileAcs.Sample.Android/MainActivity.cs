using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using TheFactorM.Federation.Android;

namespace MobileAcs.Sample.Android
{
	[Activity (Label = "MobileAcs.Sample.Android", MainLauncher = true)]
	public class Activity1 : Activity
	{
		TextView _loggedOnTextView;
		bool _signedIn;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			_loggedOnTextView = FindViewById<TextView>(Resource.Id.loggedOnTextView);
			_loggedOnTextView.Text = "You are not currently logged on.";
		}

		protected override void OnResume ()
		{
			base.OnResume ();

			if(!_signedIn) {
			// Start the signin operation from within the app, after you have determined
			// that the current login isn't available or expired.
			// The result of the signin operation will be returned in the OnActivityResult method.
			this.BeginSignInOperation("yourAcsNamespace", 
				"http://yourapp.cloudapp.net","Your-reply-address");
			}
		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult (requestCode, resultCode, data);

			// Finalize the signin operation. 
			// This gets you the ACS cookies, required to keep you signed in to the 
			// services you use in your app.
			var cookies =  this.FinalizeSignInOperation(requestCode,resultCode,data);

			if(cookies != null && cookies.Count > 0)
			{
				// Yay, received the ACS cookies. Store them somewhere safe
				// in your app and continue to use the app as normal.

				_signedIn = true;
				_loggedOnTextView.Text = "You are now logged on";
			}
		}
	}
}


