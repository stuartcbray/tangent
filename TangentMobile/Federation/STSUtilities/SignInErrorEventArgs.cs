using System;

namespace TheFactorM.Federation
{
	public class SignInErrorEventArgs: EventArgs
	{
		/// <summary>
		/// The error that occurred during Sign In.
		/// </summary>
		/// <value>The error.</value>
		public Exception Error { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="SignInErrorEventArgs"/> class.
		/// </summary>
		/// <param name="error">Error.</param>
		public SignInErrorEventArgs(Exception error)
		{
			Error = error;
		}
	}
}

