
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;

namespace TheFactorM.Federation
{
    public class JSONIdentityProviderDiscoveryClient
    {
		private WebClient _webClient;

        internal event EventHandler<GetIdentityProviderListEventArgs> GetIdentityProviderListCompleted;

        internal void GetIdentityProviderListAsync(Uri identityProviderListServiceEndpoint)
        {
            _webClient = new WebClient();

            _webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(webClient_DownloadStringCompleted);
            _webClient.DownloadStringAsync(identityProviderListServiceEndpoint);
        }

        private void webClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
			_webClient.DownloadStringCompleted -= webClient_DownloadStringCompleted;
			_webClient = null;

            IEnumerable<IdentityProviderInformation> identityProviders = null;
            Exception error = e.Error;

            if (null == e.Error)
            {
                try
                {
                    using (var reader = new JsonTextReader(new StringReader(e.Result)))
                    {
                        var serializer = new JsonSerializer();
                        identityProviders = serializer.Deserialize<List<IdentityProviderInformation>>(reader);
                    }
                    IdentityProviderInformation windowsLiveId = identityProviders.FirstOrDefault(i => i.Name.Equals("Windows Live™ ID", StringComparison.OrdinalIgnoreCase));
                    if (windowsLiveId != null)
                    {
                        string separator = windowsLiveId.LoginUrl.Contains("?") ? "&" : "?";
						// pcexp=false will make sure that Windows Live Login appears in "popup" mode
                        windowsLiveId.LoginUrl = string.Format(CultureInfo.InvariantCulture, "{0}{1}pcexp=false", windowsLiveId.LoginUrl, separator);
                    }
                }
                catch (Exception ex)
                {
                    error = ex;
                }
            }

            if (null != GetIdentityProviderListCompleted)
            {
                GetIdentityProviderListCompleted(this, new GetIdentityProviderListEventArgs(identityProviders, error));
            }
        }
    }
}
