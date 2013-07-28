using System;
using System.Net;
using TheFactorM.Federation;
using MonoTouch.Foundation;

namespace TheFactorM.Federation
{
	/// <summary>
	/// Extension methods and helper methods for handling cookies in a device specific manner.
	/// </summary>
	public static class CookieUtilities
	{		
		/// <summary>
		/// Converts the NSHttpCookie to an System.Net.Cookie
		/// </summary>
		/// <param name="cookie">
		/// The <see cref="NSHttpCookie"/> to be converted.
		/// </param>
		/// <returns>
		/// A <see cref="Cookie"/>, which is a copy of the NSHttpCookie
		/// </returns>
		public static Cookie ToClrCookie(this NSHttpCookie cookie)
		{
			return new System.Net.Cookie()
			{
				Name = cookie.Name,
				Value = cookie.Value,
				Version = (int) cookie.Version,
				Expires = cookie.ExpiresDate != null ? cookie.ExpiresDate.ToDateTime() : DateTime.MaxValue,
				Domain = cookie.Domain,
				Path = cookie.Path,
				Secure = cookie.IsSecure,
				HttpOnly = cookie.IsHttpOnly
			};
		}
	}
}

