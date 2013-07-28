using System;
using TheFactorM.Federation.Android.Activities;
using Android.Content;
using Android.App;
using System.Net;

namespace TheFactorM.Federation.Android
{
	public static class ActivityExtensions
	{
		/// <summary>
		/// Begins the sign in process from the current activity.
		/// </summary>
		/// <param name="activity">Activity.</param>
		/// <param name="acsNamespace">Acs namespace.</param>
		/// <param name="acsHomeRealm">Acs home realm.</param>
		/// <param name="acsReplyToAddress">Acs reply to address.</param>
		public static void BeginSignInOperation (this Activity activity, string acsNamespace, string acsHomeRealm, string acsReplyToAddress)
		{
			Intent intent = new Intent (activity, typeof(IdentityProvidersListActivity));

			// Assign a bunch of extras, containing all the config data needed to talk to the ACS.
			intent.PutExtra (Extras.AcsHomeRealm,acsHomeRealm);
			intent.PutExtra (Extras.AcsNamespace,acsNamespace);
			intent.PutExtra (Extras.AcsReplyToAddress,acsReplyToAddress);

			// Start the actual sign in process for the app.
			activity.StartActivityForResult(intent,RequestCodes.SignInActivity);
		}

		/// <summary>
		/// Finalizes the sign in operation from the OnActivityResult method
		/// </summary>
		/// <returns>The sign in operation.</returns>
		/// <param name="activity">The activity the sign in process was started from</param> 
		/// <param name="requestCode">Request code.</param>
		/// <param name="resultCode">Result code.</param>
		/// <param name="data">Data.</param>
		public static CookieCollection FinalizeSignInOperation(this Activity activity, int requestCode, Result resultCode, Intent data)
		{
			// Check if the result is valid and this data is meant for the sign in operation
			if(resultCode == Result.Ok && requestCode == RequestCodes.SignInActivity)
			{
				Console.WriteLine ("Get a valid result to process.");

				string cookieString = data.GetStringExtra (Extras.ReceivedCookies);
				return FedAuthCookieUtility.ParseCookies(cookieString,Settings.Current.CookieExpirationDate);
			}

			return null;
		}
	}
}

