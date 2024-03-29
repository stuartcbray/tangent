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
using Windows.Storage;

namespace TheFactorM.Federation
{
    /// <summary>
    /// Specialized version for Windows 8
    /// </summary>
    public class RequestSecurityTokenResponseStore
    {
        private ApplicationDataContainer _isolatedStore = null;
        private RequestSecurityTokenResponse _RSTR = null;
        private const string RequestSecurityTokenResponseSettingKeyName = "TheFactorM.Federation.STSUtilities.RequestSecurityTokenResponseStore";

        /// <summary>
        /// Initializes a new instance of the RequestSecurityTokenResponseStore class. 
        /// </summary>
        public RequestSecurityTokenResponseStore()
        { }

        /// <summary>
        /// Gets or sets the configured RequestSecurityTokenResponse
        /// </summary>
        /// <remarks>Get returns null if no RequestSecurityTokenResponse has been configured. </remarks>
        public RequestSecurityTokenResponse RequestSecurityTokenResponse
        {
            get
            {
                if (null == _RSTR)
                {
                    if (this.Settings.Values.ContainsKey(RequestSecurityTokenResponseSettingKeyName))
                    {
                        _RSTR = ApplicationData.Current.LocalSettings.Values[RequestSecurityTokenResponseSettingKeyName] as RequestSecurityTokenResponse;
                    }
                }

                return _RSTR;
            }

            set
            {
                if (null == value && this.Settings.Values.ContainsKey(RequestSecurityTokenResponseSettingKeyName))
                {
                    this.Settings.Values.Remove(RequestSecurityTokenResponseSettingKeyName);
                }
                else
                {
                    this.Settings.Values[RequestSecurityTokenResponseSettingKeyName] = value;
                }

                _RSTR = value;
            }
        }

        /// <summary>
        /// Gets or sets the security token from the configured RequestSecurityTokenResponse
        /// </summary>
        /// <remarks>Get returns null if no RequestSecurityTokenResponse has been configured. </remarks>
        public string SecurityToken
        {
            get
            {
                return null == RequestSecurityTokenResponse ? null : RequestSecurityTokenResponse.securityToken;
            }
        }

        /// <summary>
        /// Checks if there is a valid RequestSecurityTokenResponse currrenlty in the store.
        /// </summary>
        /// <remarks>Returns true if there is a RequestSecurityTokenResponse in the store and it has not expired,
        /// otherwise retruns false</remarks>
        public bool ContainsValidRequestSecurityTokenResponse()
        {
            if (null == this.RequestSecurityTokenResponse)
            {
                return false;
            }

            return true;
        }

        private ApplicationDataContainer Settings
        {
            get
            {
                if (_isolatedStore == null)
                {
                    _isolatedStore = ApplicationData.Current.LocalSettings;
                }

                return _isolatedStore;
            }
        }
    }
}
