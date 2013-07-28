using System;

namespace TheFactorM.Federation
{
	public class IdentityProviderSelectedEventArgs: EventArgs
	{
		public IdentityProviderSelectedEventArgs ()
		{
		}
		
		/// <summary>
		/// Gets or sets the identity provider where to do the authentication.
		/// </summary>
		public IdentityProviderInformation IdentityProvider
		{
			get; set;
		}
	}
}

