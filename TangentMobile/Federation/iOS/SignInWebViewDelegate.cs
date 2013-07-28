using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Linq;
using TheFactorM.Federation;
using System.Diagnostics;

namespace TheFactorM.Federation
{
	public class SignInWebViewDelegate: UIWebViewDelegate
	{
		private AccessControlServiceSignInController _vc;
		
		/// <summary>
		/// Fires when sign in cookies were received and put into the CookieContainer.
		/// </summary>
		public event EventHandler<SignInCookieReceivedEventArgs> SignInCookieReceived;
		
		public SignInWebViewDelegate (AccessControlServiceSignInController vc): base()
		{
			_vc = vc;
		}
		
		public override void LoadStarted (UIWebView webView)
		{
			InvokeOnMainThread (() =>
			{ 
				_vc.ShowProgressBar(null);
			});
		}

		public override bool ShouldStartLoad (UIWebView webView, NSUrlRequest request, UIWebViewNavigationType navigationType)
		{
			Console.WriteLine ("Opening {0}", request.Url.AbsoluteString);
			return true;
		}

		public override void LoadingFinished (UIWebView webView)
		{			
#if DEBUG
			Console.WriteLine("Navigated to: " + webView.Request.Url.ToString());
#endif
			if (string.Compare(webView.Request.Url.Path, NSBundle.MainBundle.BundlePath, StringComparison.OrdinalIgnoreCase) == 0 && _vc.SelectedIdentityProvider != null)
			{
				var url = new NSUrl(_vc.SelectedIdentityProvider.LoginUrl);
				webView.LoadRequest(new NSUrlRequest(url));
			}
			else
			{
				InvokeOnMainThread(() => _vc.ShowBrowser());

				if (_vc.NavigatingToIdentityProvider)
				{
					var requestUrl = webView.Request.Url.ToString ().TrimEnd ('/');
					if (requestUrl.Equals(_vc.ReplyToAddress, StringComparison.OrdinalIgnoreCase))
					{
						var cookies = NSHttpCookieStorage.SharedStorage.Cookies.Where(c => c.Name.StartsWith("FedAuth"));
						var urlString = string.Empty;
						var cookieCollection = new System.Net.CookieCollection();

						if (cookies != null)
						{
#if DEBUG
							Console.WriteLine(cookies.Count().ToString() + " cookies");
#endif
						}

						foreach (NSHttpCookie cookie in cookies)
						{
							if (string.IsNullOrEmpty(urlString))
							{
								urlString = cookie.Domain;
							}

#if DEBUG
							if (cookie.ExpiresDate != null)
							{
								Console.WriteLine("NSHttpCookie {0} expires {1}", cookie.Name, cookie.ExpiresDate.ToString());
							}
#endif

							var netCookie = cookie.ToClrCookie();
							cookieCollection.Add(netCookie);
						}
											
						if (this.SignInCookieReceived != null)
						{
							SignInCookieReceived(this, new SignInCookieReceivedEventArgs(cookieCollection));
						}
					}
				}
			}
		}
	}
}

