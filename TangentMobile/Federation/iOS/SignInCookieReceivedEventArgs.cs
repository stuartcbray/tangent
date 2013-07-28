using System;
using System.Net;

namespace TheFactorM.Federation
{
	/// <summary>
	/// Sign in cookie received event arguments.
	/// Contains the set of cookies received after sign in.
	/// </summary>
	public class SignInCookieReceivedEventArgs: EventArgs
	{
		public CookieCollection Cookies { get; set; }

		public SignInCookieReceivedEventArgs (CookieCollection cookies)
		{
			Cookies = cookies;
		}
	}
}

