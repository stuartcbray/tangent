using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace TheFactorM.Federation.W8.Controls
{
    public sealed partial class AccessControlServiceSignIn : UserControl
    {
        #region Private fields

        private Uri _identityProviderDiscoveryService = null;
        private bool _navigatingToIdentityProvider = false;
        private object _navigatingToIdentityProviderLock = new object();
        private string _realm = null;
        private string _replyAddress = null;
        private string _serviceNamespace = null;
        private RequestSecurityTokenResponseStore _rstrStore;
        private IdentityProviderInformation _selectedIdentityProvider = null;
        private bool _setBrowserVisible = false;
        private object _setBrowserVisibleLock = new object();
        private IEnumerable<IdentityProviderInformation> _identityProviders;
        private bool _isLoading;

        // added this container, so it can be used when we pick up the token from the acs call redirects
        private CookieContainer _securityTokenCookieContainer;
        // added for fast W8 submission to stor, to get the name identifier so we can communicate with Signal R
        private string _nameIdentifier;
        private string _identityProvider;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the AccessControlServiceSignInControl class. 
        /// </summary>
        public AccessControlServiceSignIn()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        #endregion

        #region Public events

        /// <summary>
        /// Occurs when a security token that issued by ACS and ready to be presented to the application.
        /// </summary>
        public event EventHandler<RequestSecurityTokenResponseCompletedEventArgs> RequestSecurityTokenResponseCompleted;

        /// <summary>
        /// this event is fired when the user clicks the back arrow on the authentication control.
        /// </summary>
        public event EventHandler RequestSecurityTokenResponseCancled;
        /// <summary>
        /// Occurs when the user selects an identity provider to log in with.
        /// </summary>
        public event EventHandler<IdentityProviderInformationEventArgs> NavigatingToIdentityProvider;

        #endregion

        #region Public properties

        public bool UseWSFederation { get; set; }

        /// <summary>
        /// Gets whether there is at least one state that the control can navigate back from.
        /// <value>True if a least one state can be undone, or false otherwise.</value>
        /// </summary>
        public bool CanGoBack
        {
            get { return _navigatingToIdentityProvider; }
        }

        /// <summary>
        /// Gets and Sets the RequestSecurityTokenResponseStore which is used to store
        /// the RequestSecurityTokenResponse returned from ACS
        /// </summary>
        public RequestSecurityTokenResponseStore RequestSecurityTokenResponseStore
        {
            get { return _rstrStore; }
            set { _rstrStore = value; }
        }

        public string Realm
        {
            get { return _realm; }
            set { _realm = value; }
        }

        public string ServiceNamespace
        {
            get { return _serviceNamespace; }
            set { _serviceNamespace = value; }
        }

        public string ReplyAddress
        {
            get { return _replyAddress; }
            set { _replyAddress = value; }
        }

        /// use this property to read the cookieContainer so you can use it to do authenticated SOAP or Rest calls using fedauth cookies
        public CookieContainer SecurityTokenCookieContainer
        {
            get { return _securityTokenCookieContainer; }
            set { _securityTokenCookieContainer = value; }
        }

        /// <summary>
        /// use this property to read the NameIdenfier of the logged on user, after the completed event.
        /// </summary>
        public string NameIdentifier
        {
            get { return _nameIdentifier; }
            set { _nameIdentifier = value; }
        }

        public string IdentityProvider 
        { 
            get{return _identityProvider;}
            set { _identityProvider = value; } 
        }


        #endregion

        #region Public methods

        /// <summary>
        /// Initiates an asynchronous request which prompts user to sign into an identity provider, from the list returned by the
        /// call to the discover service returns a security token via the RequestSecurityTokenResponseCompleted event handler. 
        /// </summary>
        /// 
        /// <remarks>
        /// Initiates a token request from ACS following these steps:
        /// 1) Get the list of configured Identity Providers from ACS by calling the discovery service
        /// 2) Once the user selects their identity provider, navigate to the sign in page of the provider
        /// 3) Using the WebBrowser control to complete the passive token request complete
        /// 4) Get the token
        /// 5) If a RequestSecurityTokenResponseStore is specified, set the token.
        /// 6) return the token using the RequestSecurityTokenResponseCompleted callback
        /// </remarks>
        /// <param name="identityProviderDiscoveryService">The Identity provider discovery service from the ACS managment portal.</param>
        public void GetSecurityToken(Uri identityProviderDiscoveryService)
        {
            _identityProviderDiscoveryService = identityProviderDiscoveryService;
            IdentityProviderList_Refresh(_identityProviderDiscoveryService);
        }

        public void GetSecurityToken()
        {
            if (null == _realm)
            {
                throw new InvalidOperationException("Realm was not set");
            }

            if (null == _serviceNamespace)
            {
                throw new InvalidOperationException("ServiceNamespace was not set");
            }

            string IDENTITYPROVIDER_SERVICE_URI = null;
            if (!UseWSFederation)
            {

                IDENTITYPROVIDER_SERVICE_URI = string.Format(
                    "https://{0}.accesscontrol.windows.net/v2/metadata/IdentityProviders.js?protocol=javascriptnotify&realm={1}&version=1.0",
                    _serviceNamespace, WebUtility.UrlEncode(_realm));
            }
            else
            {
                if (_replyAddress == null)
                {
                    throw new InvalidOperationException("When using wsfederation you need to set the ReplyAdress on the control, where the javascript is found to hand back the cookie to the control");

                }

                IDENTITYPROVIDER_SERVICE_URI = String.Format(CultureInfo.InvariantCulture,
                "https://{0}.accesscontrol.windows.net/v2/metadata/IdentityProviders.js?protocol=wsfederation&realm={1}&version=1.0",
                _serviceNamespace, _realm);
            }

            Uri identityProviderDiscovery = new Uri(
                IDENTITYPROVIDER_SERVICE_URI,
                UriKind.Absolute
                );

            GetSecurityToken(identityProviderDiscovery);
        }

        /// <summary>
        /// Performs a backward navigation action, transitioning the control to a previous state. 
        /// <remarks>
        /// If the control is in a state that it cannot go backwards, no action is taken.
        /// </remarks>
        /// </summary>
        public void GoBack()
        {
            lock (_navigatingToIdentityProviderLock)
            {
                if (_navigatingToIdentityProvider)
                {
                    ShowProgressBar();
                    IdentityProviderList_Refresh(_identityProviderDiscoveryService);
                    _navigatingToIdentityProvider = false;
                }
            }
        }

        #endregion

        #region Private methods

        private void NavigateToIdentityProvider(IdentityProviderInformation identityProvider)
        {
            if (null != identityProvider)
            {
                ShowProgressBar();

                if (null != this.NavigatingToIdentityProvider)
                {
                    this.NavigatingToIdentityProvider(this, new IdentityProviderInformationEventArgs(identityProvider));
                }

                lock (_navigatingToIdentityProviderLock)
                {
                    _navigatingToIdentityProvider = true;

                    BrowserSigninControl.LoadCompleted += BrowserSigninControl_LoadCompleted;
                    //windows 8 special code to allow script notify
                    List<Uri> allowedUris = new List<Uri>();
                    allowedUris.Add(new Uri(this.ReplyAddress));
                    allowedUris.Add(new Uri(string.Format("https://{0}.accesscontrol.windows.net/v2/wsfederation",_serviceNamespace)));
                    BrowserSigninControl.AllowedScriptNotifyUris = allowedUris;
                    BrowserSigninControl.NavigationFailed += BrowserSigninControl_NavigationFailed;
                    BrowserSigninControl.ScriptNotify += this.SignInWebBrowserControl_ScriptNotify;
                    _selectedIdentityProvider = identityProvider;
                    BrowserSigninControl.NavigateToString("<html><head><title></title></head><body></body></html>");
                }
            }
        }

        void BrowserSigninControl_NavigationFailed(object sender, WebViewNavigationFailedEventArgs e)
        {
            DisplayErrorMessage(e.WebErrorStatus.ToString());
        }


        private void IdentityProviderList_Refresh(Uri identityProviderDiscoveryService)
        {
            JSONIdentityProviderDiscoveryClient jsonClient = new JSONIdentityProviderDiscoveryClient();
            jsonClient.GetIdentityProviderListCompleted += new EventHandler<GetIdentityProviderListEventArgs>(IdentityProviderList_RefreshCompleted);
            jsonClient.GetIdentityProviderListAsync(identityProviderDiscoveryService);
        }

        private void IdentityProviderList_RefreshCompleted(object sender, GetIdentityProviderListEventArgs e)
        {
            if (null == e.Error)
            {
                this.identityProviderList.ItemsSource = e.Result;
                ShowIdentityProviderSelection();
            }
            else
            {
                DisplayErrorMessage(string.Format(
                    CultureInfo.InvariantCulture,
                    "There was an error retrieving the Identity Providers list. Please check your internet connection. {0}",
                    e.Error.Message));
            }
        }

        private void SignInWebBrowserControl_ScriptNotify(object sender, NotifyEventArgs e)
        {
            BrowserSigninControl.ScriptNotify -= this.SignInWebBrowserControl_ScriptNotify;

            RequestSecurityTokenResponse rstr = null;
            string message = e.Value;

            Exception exception = null;
            try
            {
                ShowProgressBar();
                // added feature to support FEDAUTH cookies and SOAP usign Basic HTTP
                if (message.Contains("done"))
                {
                    // we are logged in, now get the cookie
                    GetCookieAndCacheInDeviceContextContainer(Realm, message.Substring(message.IndexOf(':')));
                }
                else // defaut handling conform toolkit
                {
                    rstr = RequestSecurityTokenResponse.FromJSON(e.Value);

                    if (null == rstr)
                    {
                        DisplayErrorMessage("Failed reading RSTR");
                        exception = new InvalidOperationException("Failed to get a valid RequestSecurityTokenResponse");
                    }

                    if (null != _rstrStore)
                    {
                        _rstrStore.RequestSecurityTokenResponse = rstr;
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayErrorMessageFromException(ex);
                exception = ex;
            }

            if (null != RequestSecurityTokenResponseCompleted)
            {
                RequestSecurityTokenResponseCompleted(this, new RequestSecurityTokenResponseCompletedEventArgs(rstr, exception));
            }
        }

        private void GetCookieAndCacheInDeviceContextContainer(string serviceUrl, string cookieString)
        {
            if (string.IsNullOrEmpty(cookieString))
            {
                DisplayErrorMessage("Sign in failed.");
            }
            else
            {
                // decode the cookies back so they are usable
                var cookiesBase64Encoded = cookieString.Split(' ');
                SecurityTokenCookieContainer = new CookieContainer();
                foreach (var cookie in cookiesBase64Encoded.Where(c => c.StartsWith("FedAuth")))
                {
                    var splitLocation = cookie.IndexOf('=');
                    var cookieName = cookie.Substring(0, splitLocation);
                    var cookieValue = cookie.Substring(splitLocation + 1);
                    cookieValue = Base64Decode(cookieValue);
                    //TODO: decode the lifetime from the token, for now jsut amke it valid for 23 hrs
                    SecurityTokenCookieContainer.Add(new Uri(serviceUrl), new Cookie(cookieName, cookieValue) { Expires = DateTime.Now.AddHours(23) });
                }
            }

            if (null != RequestSecurityTokenResponseCompleted)
            {
                RequestSecurityTokenResponseCompleted(this, new RequestSecurityTokenResponseCompletedEventArgs(null, null));
            }

        }

        private string Base64Decode(string data)
        {
            try
            {
                System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
                System.Text.Decoder utf8Decode = encoder.GetDecoder();

                byte[] todecode_byte = Convert.FromBase64String(data);
                int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
                char[] decoded_char = new char[charCount];
                utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
                string result = new String(decoded_char);
                return result;
            }
            catch (Exception e)
            {
                throw new Exception("Error in base64Decode" + e.Message);
            }
        }

        void BrowserSigninControl_LoadCompleted(object sender, NavigationEventArgs e)
        {
            // here we decide if we want to show or hode the browser control
            // we only want to show it when it is at the idp page

            // Navigate if at empty page, and a idp hrd url is available
            if (null == e.Uri && null != _selectedIdentityProvider)
            {
                BrowserSigninControl.Navigate(new Uri(_selectedIdentityProvider.LoginUrl));
            }
            else
            {
                // hide the browser as soon as we are done with the pasive logon and the redirect fest starts to ACS and to our reply adress
                if (e.Uri.ToString().Contains("accesscontrol.windows.net"))
                {
                    HideBrowser();
                    ShowProgressBar("Signing in");
                }
                if (e.Uri.ToString().ToLower().Contains(ReplyAddress.ToLower()))
                {
                    // browser is already hidden, so do our thing and get out of here
                    var tokendata = BrowserSigninControl.InvokeScript("getCookie", null);
                    NameIdentifier = BrowserSigninControl.InvokeScript("getNameIdentifier", null);
                    IdentityProvider = BrowserSigninControl.InvokeScript("getIdentityProviderName", null);
                    GetCookieAndCacheInDeviceContextContainer(Realm, tokendata.Substring(tokendata.IndexOf(':')));
                    if (null != RequestSecurityTokenResponseCompleted)
                    {
                        RequestSecurityTokenResponseCompleted(this, new RequestSecurityTokenResponseCompletedEventArgs(null, null));
                    }
                }
                else
                {
                    lock (_setBrowserVisibleLock)
                    {
                        _setBrowserVisible = true;
                        if (_setBrowserVisible && _navigatingToIdentityProvider)
                        {
                            ShowBrowser();
                        }
                    }
                }
            }
        }

        private void DisplayErrorMessageFromException(Exception e)
        {
            if (null != e)
            {
                DisplayErrorMessage(e.Message);
            }
        }

        private void DisplayErrorMessage(string message)
        {
            // ensure foreground thread here, otherwise you get an access denied
            DeviceContext.Current.RunOnForegroundThread(() =>
            {
                    MessageDialog dlg = new MessageDialog(message);
                    dlg.ShowAsync();
            });
        }

        private void HideAll()
        {
            identityProviderDiscovery.Visibility = Visibility.Collapsed;
            BrowserSigninControl.Visibility = Visibility.Collapsed;
            progressBar.Visibility = Visibility.Collapsed;
            Message.Text = "";
        }

        private void ShowBrowser()
        {
            HideAll();
            BrowserSigninControl.Visibility = Visibility.Visible;
        }
        private void HideBrowser()
        {
            BrowserSigninControl.Visibility = Visibility.Collapsed;
        }

        private void ShowIdentityProviderSelection()
        {
            HideAll();
            identityProviderDiscovery.Visibility = Visibility.Visible;
        }

        private void ShowProgressBar()
        {
            HideAll();
            progressBar.Visibility = Visibility.Visible;
        }

        private void ShowProgressBar(string message)
        {
            Message.Text = message;
            ShowProgressBar();
        }

        private void OnIdentityProviderSelected(object sender, ItemClickEventArgs e)
        {
            IdentityProviderInformation identityProvider = e.ClickedItem as IdentityProviderInformation;
            NavigateToIdentityProvider(identityProvider);
        }

        #endregion

        private void GoBack(object sender, RoutedEventArgs e)
        {
            // call the cancle event , container should now hide the control
            if (RequestSecurityTokenResponseCancled != null)
            {
                RequestSecurityTokenResponseCancled.Invoke(this, new EventArgs());
            }
        }

    }
}

