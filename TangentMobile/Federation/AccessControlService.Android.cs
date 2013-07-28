using System;
using Android.Content;
using Android.App;
using TheFactorM.Federation.Android.Activities;
using TheFactorM.Federation.Android;

namespace TheFactorM.Federation
{
	public partial class AccessControlService
	{
		private Activity _sourceActivity;

		public AccessControlService (Activity activity)
		{
			activity.
		}

		partial void InternalSignin (SignInOperationData operationData)
		{
			var intent = new Intent(Application.Context,typeof(IdentityProvidersListActivity));

			intent.PutExtra (Extras.AcsHomeRealm,operationData.AcsHomeRealm);
			intent.PutExtra (Extras.AcsNamespace,operationData.AcsNamespace);
			intent.PutExtra(Extras.AcsReplyToAddress,operationData.AcsReplyToAddress);
		}
	}
}

