﻿// ----------------------------------------------------------------------------------
// Microsoft Developer & Platform Evangelism
// 
// Copyright (c) Microsoft Corporation. All rights reserved.
// 
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// ----------------------------------------------------------------------------------
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
// ----------------------------------------------------------------------------------

//---------------------------------------------------------------------------------
// Copyright 2010 Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License"); 
// You may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

// THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, 
// INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR 
// CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, 
// MERCHANTABLITY OR NON-INFRINGEMENT. 

// See the Apache 2 License for the specific language governing 
// permissions and limitations under the License.
//---------------------------------------------------------------------------------

using System;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using System.Globalization;
using System.Linq;
using TheFactorM.Federation;

namespace TheFactorM.Federation
{
    /// <summary>
    /// This control is used to aquire a token from ACS using passive protocals between ACS and the Identity Provider.
    /// </summary>
    public partial class AccessControlServiceSignIn : UserControl
    {
        private Uri _identityProviderDiscoveryService = null;
        private bool _navigatingToIdentityProvider = false;
        private object _navigatingToIdentityProviderLock = new object();
        private string _realm = null;
        private string _serviceNamespace = null;
        private RequestSecurityTokenResponseStore _rstrStore;
        private IdentityProviderInformation _selectedIdentityProvider = null;
        private bool _setBrowserVisible = false;
        private object _setBrowserVisibleLock = new object();

        /// <summary>
        /// Occurs when a security token that issued by ACS and ready to be presented to the application.
        /// </summary>
        public event EventHandler<RequestSecurityTokenResponseCompletedEventArgs> RequestSecurityTokenResponseCompleted;

        /// <summary>
        /// Occurs when the user selects an identity provider to log in with.
        /// </summary>
        public event EventHandler<IdentityProviderInformationEventArgs> NavigatingToIdentityProvider;

        /// <summary>
        /// Initializes a new instance of the AccessControlServiceSignInControl class. 
        /// </summary>
        public AccessControlServiceSignIn()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets whether there is at least one state that the control can navigate back from.
        /// <value>True if a least one state can be undone, or false otherwise.</value>
        /// </summary>
        public bool CanGoBack { get { return _navigatingToIdentityProvider; } }

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

        public CookieContainer CookieContainer
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the service base url to be used for the sign in cookies.
        /// required to scope the cookie to the right server
        /// </summary>
        public string ServiceUrl { get; set; }

        /// <summary>
        /// this is the destination Url where the ACS redirects to
        /// this is the page that returns the cookie using the script notify
        /// it must be the full qualified url including http://
        /// e.g. 
        /// </summary>
        public string LogonPageDestinationUrl { get; set; }

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

            Uri identityProviderDiscovery = new Uri(
                string.Format(
                    "https://{0}.accesscontrol.windows.net/v2/metadata/IdentityProviders.js?protocol=javascriptnotify&realm={1}&version=1.0",
                    _serviceNamespace,
                    HttpUtility.UrlEncode(_realm)),
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
                    ShowProgressBar(String.Empty);
                    IdentityProviderList_Refresh(_identityProviderDiscoveryService);
                    _navigatingToIdentityProvider = false;
                }
            }
        }

        private void IdentityProviderList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IdentityProviderInformation identityProvider = IdentityProviderList.SelectedItem as IdentityProviderInformation;

            NavigateToIdentityProvider(identityProvider);

            //
            // Reset to default value
            //
            IdentityProviderList.SelectedIndex = -1;
        }

        private void NavigateToIdentityProvider(IdentityProviderInformation identityProvider)
        {
            if (null != identityProvider)
            {
                ShowProgressBar(String.Format(String.Format("Contacting {0}", identityProvider.Name)));

                if (null != this.NavigatingToIdentityProvider)
                {
                    this.NavigatingToIdentityProvider(this, new IdentityProviderInformationEventArgs(identityProvider));
                }

                lock (_navigatingToIdentityProviderLock)
                {
                    _navigatingToIdentityProvider = true;

                    BrowserSigninControl.Navigated += this.SignInWebBrowserControl_Navigated;
                    BrowserSigninControl.Navigating += this.SignInWebBrowserControl_Navigating;
                    BrowserSigninControl.ScriptNotify += this.SignInWebBrowserControl_ScriptNotify;
                    _selectedIdentityProvider = identityProvider;
                    BrowserSigninControl.NavigateToString("<html><head><title></title></head><body></body></html>");
                }
            }
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
                IdentityProviderList.ItemsSource = e.Result;
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

            string message = e.Value;
            ShowProgressBar("Signing In");

            if (message.Contains("done"))
            {
                // we are logged in, now get the cookie
                GetCookieAndCacheInDeviceContextContainer(ServiceUrl ,message.Substring(message.IndexOf(':')), CookieContainer);
            }
        }


        private void SignInWebBrowserControl_Navigated(object sender, NavigationEventArgs e)
        {
            // Navigate if at empty page, and a idp hrd url is available
            if (null == e.Uri && null != _selectedIdentityProvider)
            {
                BrowserSigninControl.Navigate(new Uri(_selectedIdentityProvider.LoginUrl));
            }
            // This is a patch to make the library work under Windows Phone 7.1
            else if (null != e.Uri && string.IsNullOrEmpty(e.Uri.ToString()) && null != _selectedIdentityProvider)
            {
                BrowserSigninControl.Navigate(new Uri(_selectedIdentityProvider.LoginUrl));
            }
            else
            {
                // don't show the logon page of the site, because that shows a white screen
                if (e.Uri == new Uri(LogonPageDestinationUrl))
                    return;

                if (_navigatingToIdentityProvider)
                {
                    lock (_setBrowserVisibleLock)
                    {
                        _setBrowserVisible = true;
                    }

                    Thread show = new Thread(() =>
                    {
                        System.Threading.Thread.CurrentThread.Join(250);

                        lock (_setBrowserVisibleLock)
                        {
                            if (_setBrowserVisible && _navigatingToIdentityProvider)
                            {
                                Dispatcher.BeginInvoke(() => { ShowBrowser(); });
                            }
                        }
                    });

                    show.Start();
                }
            }
        }
        private void GetCookieAndCacheInDeviceContextContainer(string serviceUrl, string cookieString, CookieContainer container)
        {
            if (string.IsNullOrEmpty(cookieString))
            {
                DisplayErrorMessage("Sign in failed.");
            }
            else
            {
                // decode the cookies back so they are usable
                var cookiesBase64Encoded = cookieString.Split(' ');
                foreach (var cookie in cookiesBase64Encoded.Where(c => c.StartsWith("FedAuth")))
                {
                     var splitLocation = cookie.IndexOf('=');
                     var cookieName = cookie.Substring(0, splitLocation);
                     var cookieValue = cookie.Substring(splitLocation + 1);
                     cookieValue = Base64Decode(cookieValue);
                     container.Add(new Uri(serviceUrl), new Cookie(cookieName, cookieValue) { Expires = DateTime.Now.AddHours(23) });
                }
                FedAuthCookieUtility.SaveCookies(container.GetCookies(new Uri(serviceUrl)));
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
        private void SignInWebBrowserControl_Navigating(object sender, NavigatingEventArgs e)
        {
            lock (_setBrowserVisibleLock)
            {
                _setBrowserVisible = false;
                ShowProgressBar(null);
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
            MessageBox.Show(message);
        }

        private void HideAll()
        {
            identityProviderDiscovery.Visibility = Visibility.Collapsed;
            BrowserSigninControl.Visibility = Visibility.Collapsed;
            progressBar.Visibility = Visibility.Collapsed;
        }

        private void ShowBrowser()
        {
            HideAll();
            BrowserSigninControl.Visibility = Visibility.Visible;

        }

        private void ShowIdentityProviderSelection()
        {
            HideAll();
            identityProviderDiscovery.Visibility = Visibility.Visible;
        }

        private void ShowProgressBar(string message)
        {
            HideAll();
            if (null != message)
            {
                progressBarLabel.Text = message;
            }

            progressBar.Visibility = Visibility.Visible;
        }

        private void TextBlock_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            (sender as TextBlock).Opacity = .5;
        }

        private void TextBlock_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            (sender as TextBlock).Opacity = 1;
        }


        private void TextBlock_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            (sender as TextBlock).Opacity = 1;
        }

    }
}

