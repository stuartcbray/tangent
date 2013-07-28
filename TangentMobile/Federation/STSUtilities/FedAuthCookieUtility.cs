using System;
using System.Linq;
using System.Net;
using System.Diagnostics;

namespace TheFactorM.Federation
{
    public static class FedAuthCookieUtility
    {
        const string COOKIE_FILE = "TheFactorMFederationCookies.dat";
        const string COOKIE_FOLDER = "CookieCache";

        /// <summary>
        /// Parses the cookie string into a set of usable cookies
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="cookieString"></param>
        /// <param name="expires"></param>
        /// <returns>Container containing all parsed cookies.</returns>
        public static CookieCollection ParseCookies(string cookieString, DateTime expires)
        {
            var result = new CookieCollection();

            var cookies = cookieString.Split(new string[] { "; " }, StringSplitOptions.None);

            foreach (string cookie in cookies.Where(c => c.StartsWith("FedAuth")))
            {
                var splitLocation = cookie.IndexOf('=');
                var cookieName = cookie.Substring(0, splitLocation);
                var cookieValue = cookie.Substring(splitLocation + 1);

                result.Add(new Cookie(cookieName, cookieValue) { Expires = expires });
            }

            return result;
        }

        public static string SerializeCookiesToString(CookieCollection cookies)
        {
            string result = "";
            foreach (Cookie c in cookies)
            {
                result += c.Name + "=";
                result += c.Value + "; ";
            }
            return result;
        }

        public static void SaveCookies(Uri domain, CookieContainer container)
        {
            var c = container.GetCookies(domain);
            Settings.Current.CookieExpirationDate = DateTime.Now.AddHours(23);

			Debug.WriteLine ("Serializing cookies");

            string cookieString = SerializeCookiesToString(c);
            // Save to disk
            var util = new Persistence.IsolatedStorageUtil<string>();
            if (util.FileExists(COOKIE_FILE, COOKIE_FOLDER))
            {
                util.DeleteFile(COOKIE_FILE, COOKIE_FOLDER);
            }

            util.SaveData(cookieString, COOKIE_FILE, COOKIE_FOLDER);
        }

        public static bool ReadCookies(CookieContainer container, Uri domain)
        {
            if (Settings.Current.CookieExpirationDate <= DateTime.Now)
            {
                return false;
            }

            var util = new Persistence.IsolatedStorageUtil<string>();
            if (util.FileExists(COOKIE_FILE, COOKIE_FOLDER))
            {
                try
                {
                    string cookieString = util.LoadData(COOKIE_FILE, COOKIE_FOLDER);
                    var collection = ParseCookies(cookieString, Settings.Current.CookieExpirationDate);

                    if (collection.Count > 0)
                    {
                        // pick the first FedAuth cookie and use that domain Uri
                        if (collection["FedAuth"] != null)
                        {
                            foreach (Cookie c in collection)
                            {
                                container.Add(domain, c);
                            }
                        }
                    }
                    return collection.Count > 0;
                }
                catch (Exception ex)
                {
#if DEBUG
					Debug.WriteLine("Deserialize from Isolated storage file failed! Error {0}. \n Deleting file...", ex.Message);
#endif
                    util.DeleteFile(COOKIE_FILE, COOKIE_FOLDER);
                }
            }

            return false;
        }
    }
}
