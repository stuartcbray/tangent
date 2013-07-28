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

namespace TheFactorM.Federation
{
    /// <summary>
    /// Provides data for the AccessControlServiceSignIn control SignInCompleted event
    /// </summary>
    public class SignInCompletedEventArgs : EventArgs
    {
        Exception _error;
		string _realm;

        internal SignInCompletedEventArgs(Exception error, string realm)
        {
            _error = error;
			_realm = realm;
        }

        /// <summary>
        /// Gets any exception thrown during while requesting the security token.
        /// </summary>
        /// <remarks>If no error occur the null is returned.</remarks>
        public Exception Error
        {
            get
            {
                return _error;
            }
        }
		
		/// <summary>
		/// Gets the realm for site on which the user signs in. This will be the cookie url.
		/// </summary>
		/// <value>
		/// The realm.
		/// </value>
		public string Realm
		{
			get
			{
				return _realm;
			}
		}
    }
}
