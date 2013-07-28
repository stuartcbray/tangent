using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using TheFactorM.Federation;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Tangent
{
	public partial class WelcomeViewController : UIViewController
	{
		AccessControlServiceSignInController loginViewController;
		CookieContainer globalCookieContainer;
		public static bool authenticated;

		public WelcomeViewController () : base ("WelcomeViewController", null)
		{
			globalCookieContainer = new CookieContainer ();
			loginViewController = new 
				AccessControlServiceSignInController ("tangent", 
			                                      "https://tangent.azurewebsites.net", 
				                                  "https://tangent.azurewebsites.net", 
				                                      new Uri ("https://tangent.azurewebsites.net"), 
			                                      globalCookieContainer);

			loginViewController.SignInCookieReceived += HandleSignInCookieReceived;
		}

		void HandleSignInCookieReceived (object sender, SignInCookieReceivedEventArgs e)
		{
			// For example: set an application wide flag to indicate that the user has signed in
			// or start a webservice call to the application web service to discover the user's identity
			// and/or fetch their data

			new UIAlertView("Authenticated", "You are now logged in to Tangent!"
			                , null, "ok", null).Show(); 

			authenticated = true;
			btnLogin.Hidden = true;
			btnShowTangent.Hidden = false;
			this.NavigationController.PopViewControllerAnimated (true);
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			bool stillValid = FedAuthCookieUtility.ReadCookies(globalCookieContainer, new Uri("https://tangent.azurewebsites.net"));
			if (stillValid) {
				authenticated = true;
				btnLogin.Hidden = true;
			} else {
				btnShowTangent.Hidden = true;
			}

			// Perform any additional setup after loading the view, typically from a nib.
			this.btnLogin.TouchUpInside += (sender, e) => {
				this.NavigationController.PushViewController(this.loginViewController, true);
			};

			this.btnShowTangent.TouchUpInside += (sender, e) => {
				var c = globalCookieContainer.GetCookies(new Uri("https://tangent.azurewebsites.net"));
				string cookieString = FedAuthCookieUtility.SerializeCookiesToString(c);

				var client = new WebClient();
				client.Headers[HttpRequestHeader.Cookie] = cookieString;
				client.Headers[HttpRequestHeader.ContentType] = "application/json";
			
				var tangent = client.DownloadString ("https://tangent.azurewebsites.net/api/tangents/1");

				JToken token = JObject.Parse(tangent);
				string text = (string)token.SelectToken("Text");
				tangentLabel.Text = text;
			};



		}
	}
}

