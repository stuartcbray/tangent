// ----------------------------------------------------------------------------------
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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Linq;
using System.Globalization;
using System.Net.Http;
using Newtonsoft.Json;

namespace TheFactorM.Federation
{
    /// <summary>
    /// Specialized version for Windows 8
    /// </summary>
    internal class JSONIdentityProviderDiscoveryClient
    {
        internal event EventHandler<GetIdentityProviderListEventArgs> GetIdentityProviderListCompleted;

        internal void GetIdentityProviderListAsync(Uri identityProviderListServiceEndpoint)
        {
            HttpClient webClient = new HttpClient();

            GetStringAsync(webClient, identityProviderListServiceEndpoint);
        }

        private async void GetStringAsync(HttpClient webClient, Uri identityProviderListServiceEndpoint)
        {
            IEnumerable<IdentityProviderInformation> identityProviders = null;
            Exception error = null;
            try
            {
                var result = await webClient.GetStringAsync(identityProviderListServiceEndpoint);

                using (MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(result)))
                {
                    using (var reader = new JsonTextReader(new StreamReader(ms)))
                    {
                        var serializer = new JsonSerializer();
                        identityProviders = serializer.Deserialize<IdentityProviderInformation[]>(reader);
                    }

                    IdentityProviderInformation windowsLiveId = identityProviders.FirstOrDefault(i => i.Name.Equals("Windows Live™ ID", StringComparison.OrdinalIgnoreCase));
                    if (windowsLiveId != null)
                    {
                        string separator = windowsLiveId.LoginUrl.Contains("?") ? "&" : "?";
                        windowsLiveId.LoginUrl = string.Format(CultureInfo.InvariantCulture, "{0}{1}pcexp=false", windowsLiveId.LoginUrl, separator);
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }


            if (null != GetIdentityProviderListCompleted)
            {
                DeviceContext.Current.RunOnForegroundThread(() =>
                GetIdentityProviderListCompleted(this, new GetIdentityProviderListEventArgs(identityProviders, error)));
            }
        }
    }
}
