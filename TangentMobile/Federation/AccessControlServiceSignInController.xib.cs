
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using MonoTouch.ObjCRuntime;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace TheFactorM.Federation
{
    /// <summary>
    /// iOS UIViewController for login.
    /// NOTE: you must leave this controller XIB + class in the root of the project, otherwise Xamarin.iOS won't be able
    /// to use the control from this iOS library.
    /// </summary>
	public partial class AccessControlServiceSignInController : UIViewController
	{
		private IEnumerable<IdentityProviderInformation> _identityProviders;
		private SignInController _controller;
		private string _acsNamespace;
		private string _homeRealm;
		private Uri _cookieDomainUri;
		private CookieContainer _cookieContainer;

		public event EventHandler<SignInCookieReceivedEventArgs> SignInCookieReceived;

		/// <summary>
		/// Gets or sets the reply to address.
		/// </summary>
		/// <value>
		/// The reply to address.
		/// </value>
		internal string ReplyToAddress { get; set; }

		/// <summary>
		/// The IdP that was selected by the user.
		/// </summary>
		internal IdentityProviderInformation SelectedIdentityProvider { get; set; }
		
		/// <summary>
		/// Indicates wether or not the control is in the status "navigating to the IdP"
		/// </summary>
		internal bool NavigatingToIdentityProvider { get; set; }
		
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for items that need 
		// to be able to be created from a xib rather than from managed code
		
		public AccessControlServiceSignInController (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public AccessControlServiceSignInController (NSCoder coder) : base(coder)
		{
			Initialize ();
		}
		
		/// <summary>
		/// Initializes a new instance of the controller.
		/// </summary>
		/// <param name="acsNamespace">
		/// A <see cref="System.String"/> containing the namespace of the ACS service.
		/// </param>
		/// <param name="homeRealm">
		/// A <see cref="System.String"/> containing the home realm of the RP application.
		/// </param>
		/// <param name="cookieDomainUri">
		/// The Uri under which the sign in cookies are returned from the server.
	    /// </param>
		/// <param name="container">
		/// A <see cref="System.Net.CookieContainer"/> used to store login token cookies in.
	    /// </param>
		public AccessControlServiceSignInController(string acsNamespace, string homeRealm, string replyToAddress, Uri cookieDomainUri, CookieContainer container): base("AccessControlServiceSignInController", null)
		{
			_acsNamespace = acsNamespace;
			_homeRealm = homeRealm;
			ReplyToAddress = replyToAddress;
			_cookieContainer = container;
			_cookieDomainUri = cookieDomainUri;
			Initialize();
		}
				
		void Initialize ()
		{
			_controller = new SignInController(_acsNamespace, _homeRealm, ReplyToAddress);
			_controller.IdentityProviderListAvailable += HandleIdentityProviderListAvailable;
			_controller.SignInError += HandleSignInError;
		}

		void HandleSignInError (object sender, SignInErrorEventArgs e)
		{
			InvokeOnMainThread (() => {
				DisplayErrorMessage ("An error occurred while siging in: " + e.Error.Message);
			});
		}

		void HandleIdentityProviderListAvailable (object sender, GetIdentityProviderListEventArgs e)
		{
			InvokeOnMainThread (() => {
				Items = e.Result;
			});
		}
		
		#endregion
		
		public static Selector MySelector
		{
			get 
			{
				return new Selector("HandleSwipe");
			}
		}		
		
		private class SwipeRecognizerDelegate : MonoTouch.UIKit.UIGestureRecognizerDelegate
		{
			public override bool ShouldReceiveTouch(UIGestureRecognizer recognizer, UITouch touch)
			{
				return true;
			}
		}		
				
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			var browserDelegate = new SignInWebViewDelegate(this);
			browserDelegate.SignInCookieReceived += OnSignInCookieReceived;
			webBrowser.Delegate = browserDelegate;
			
			// define the gesture recognizer 
			MonoTouch.UIKit.UISwipeGestureRecognizer sgr = new MonoTouch.UIKit.UISwipeGestureRecognizer();
			 
			// add the target to it, we put the instance itself of the controller 
			// and the class instance selector
			sgr.AddTarget(this, AccessControlServiceSignInController.MySelector);
			
			// add the swipe direction, there are 4 of them (left, right, up, down). If other than one swipe is 
			// needed then more recognizers must be defined and added to the view - each for the direction
			sgr.Direction = UISwipeGestureRecognizerDirection.Left;
			
			// also assign the delegate
			sgr.Delegate = new SwipeRecognizerDelegate();
			 
			// and last, add the recognizer to this view to take actions
			this.View.AddGestureRecognizer(sgr);
			
			HideAll();
			_controller.LoadIdentityProviders();
		}
		
		[Export("HandleSwipe")]
		public void HandleSwipe(UISwipeGestureRecognizer recognizer)
		{
			HideAll();
			_controller.LoadIdentityProviders();
		}
		
		private void OnSignInCookieReceived(object sender, SignInCookieReceivedEventArgs e)
		{
			_cookieContainer.Add(e.Cookies);
			FedAuthCookieUtility.SaveCookies(_cookieDomainUri, _cookieContainer);

			if (SignInCookieReceived != null)
			{
				// Propagate event
				SignInCookieReceived(this, e);
			}

			// the cookies are in the cookie container, so this view is done.			
			HideAll();

			if (NavigationController != null)
			{				
				// go back
				NavigationController.PopViewControllerAnimated(true);
			}
			else
			{
				DismissViewController(true, null);
			}
		}
		
		private void OnNavigateToIdp(object sender, IdentityProviderSelectedEventArgs e)
		{
			NavigateToDetails (e.IdentityProvider);
		}
		
        /// <summary>
        /// Gets whether there is at least one state that the control can navigate back from.
        /// <value>True if a least one state can be undone, or false otherwise.</value>
        /// </summary>
        public bool CanGoBack { get { return NavigatingToIdentityProvider; } }
						
        public void DisplayErrorMessageForException(Exception e)
        {
            if (null != e)
            {
                DisplayErrorMessage(e.Message);
            }
        }

        private void DisplayErrorMessage(string message)
        {
			Console.WriteLine(message);
        }

        private void HideAll()
        {
			subTitle.Hidden = true;
			identityProviderList.Hidden = true;
			webBrowser.Hidden = true;
			progressIndicator.StopAnimating();
			progressIndicator.Hidden = true;
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
		}

        internal void ShowBrowser()
        {
            HideAll();
			var name = SelectedIdentityProvider.Name == null ? "identity provider" : SelectedIdentityProvider.Name.Replace("???", "");
			subTitle.Text = "Log on to " + name + ". Swipe left to reset.";
			subTitle.Hidden = false;
			webBrowser.Hidden = false;
        }

        private void ShowIdentityProviderSelection()
        {
            HideAll();
			subTitle.Text = "Choose your identity provider";
			subTitle.Hidden = false;
			identityProviderList.Hidden = false;
        }

        public void ShowProgressBar(string message)
        {
            HideAll();
			progressIndicator.Hidden = false;
			progressIndicator.StartAnimating();
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
            if (null != message)
            {
				// TODO: add message text to progress indicator
            }
        }
		
		public bool UseIdentityProviderImages { get; set; }
			
	    #region IListUIHandler implementation

		/// <summary>
        /// Gets or sets the available identity providers for the user to choose from
        /// </summary>
		public IEnumerable<IdentityProviderInformation> Items
		{
			get
			{
				return _identityProviders;
			}
			set
			{
				_identityProviders = value;
				var source = new IdentityProviderTableViewSource(_identityProviders, UseIdentityProviderImages);
				identityProviderList.Source = source;
				source.IdentityProviderSelected += OnNavigateToIdp;
				identityProviderList.ReloadData();
                ShowIdentityProviderSelection();
			}
		}
		
		/// <summary>
        /// Perform the actual login operation using the specified identity provider
        /// </summary>
        /// <param name="identityProvider"></param>
        public void NavigateToDetails(IdentityProviderInformation identityProvider)
		{
            if (null != identityProvider)
            {
                ShowProgressBar(String.Format(String.Format("Contacting {0}", identityProvider.Name)));
				SelectedIdentityProvider = identityProvider;
                NavigatingToIdentityProvider = true;
				// start with an empty page, after which the browser will hop to the IdP.
				webBrowser.LoadHtmlString("<html><head><title></title></head><body></body></html>", new NSUrl (NSBundle.MainBundle.BundlePath, true));
            }			
		}

        /// <summary>
        /// Shows a progress indicator
        /// </summary>
        /// <param name="displayText">Text to display</param>
        public void ShowProgressIndicator(string displayText)
		{
		}

        /// <summary>
        /// Hides the current progress indicator
        /// </summary>
        public void HideProgressIndicator()
		{
		}

        /// <summary>
        /// Displays an error message
        /// </summary>
        /// <param name="message"></param>
        public void ShowErrorMessage(string message)
		{
			this.MessageBox("Error", message);
		}
		
		public void Close()
		{
			if (NavigationController != null)
			{
				NavigationController.PopViewControllerAnimated(true);
			}
		}

		private void MessageBox(string title, string message)
		{
			using(UIAlertView alert = new UIAlertView())
			{
				alert.Title = title;
				alert.Message = message;
				alert.AddButton("OK");
				alert.Show();
			}
		}
				
		#endregion
	}
}

