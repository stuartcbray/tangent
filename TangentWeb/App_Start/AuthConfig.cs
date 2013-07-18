using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Web.WebPages.OAuth;
using TangentWeb.Models;

namespace TangentWeb
{
  public static class AuthConfig
  {
    public static void RegisterAuth()
    {
      // To let users of this site log in using their accounts from other sites such as Microsoft, Facebook, and Twitter,
      // you must update this site. For more information visit http://go.microsoft.com/fwlink/?LinkID=252166

      //OAuthWebSecurity.RegisterMicrosoftClient(
      //    clientId: "",
      //    clientSecret: "");
      
      // Now that we use Access Control for auth, we don't need Forms auth 
      //OAuthWebSecurity.RegisterTwitterClient(
      //    consumerKey: "XIx1pPWCNVASijm3yA25jQ",
      //    consumerSecret: "HOyZMjwtP8VKZQ9L54qjtwv2ZLjMOnAElWbWj5jyY");

      //OAuthWebSecurity.RegisterFacebookClient(
      //  appId: "173849559452771",
      //  appSecret: "da2b50d91a21582011ee2e0f443566f4");

      //OAuthWebSecurity.RegisterGoogleClient();
      //OAuthWebSecurity.RegisterYahooClient();
    }
  }
}
