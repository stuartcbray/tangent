using System;
using Android.Webkit;

namespace TheFactorM.Federation.Android
{
	/// <summary>
	/// Sign in web view client.
	/// </summary>
	public class SignInWebViewClient: WebViewClient
	{
		private string _replyToAddress;
		private string _receivedCookies;

		/// <summary>
		/// Initializes a new instance of the <see cref="TheFactorM.Federation.Android.SignInWebViewClient"/> class.
		/// </summary>
		/// <param name="replyToAddress">Reply to address.</param>
		public SignInWebViewClient (string replyToAddress)
		{
			_replyToAddress = replyToAddress;
		}

		public event EventHandler UserSignedIn;
		public event EventHandler SignInOperationFailed;

		/// <summary>
		/// Gets invoked when the page is loaded
		/// </summary>
		/// <param name="view">View.</param>
		/// <param name="url">URL.</param>
		public override void OnPageFinished (WebView view, string url)
		{
			base.OnPageFinished (view, url);

			if (String.Compare(url, _replyToAddress, StringComparison.OrdinalIgnoreCase) == 0) {
				// Get cookies for the URL
				string cookieString = CookieManager.Instance.GetCookie (url);
				CookieSyncManager.Instance.Sync ();

				if (!string.IsNullOrEmpty(cookieString)) {
					_receivedCookies = cookieString;

					if (UserSignedIn != null) {
						UserSignedIn (this, EventArgs.Empty);
					}
				} else {
					if (SignInOperationFailed != null) {
						SignInOperationFailed (this, EventArgs.Empty);
					}
				}
			}
		}

		/// <summary>
		/// Gets the received cookies
		/// </summary>
		/// <value>The received cookies.</value>
		public string ReceivedCookies { get { return _receivedCookies; } }
	}
}

