using System;
using TheFactorM.Federation.Device.Settings;

namespace TheFactorM.Federation
{
	internal partial class Settings: SettingsBase<Settings>
	{
        /// <summary>
        /// Gets or sets when the cookie(s) expire.
        /// </summary>
        public DateTime CookieExpirationDate
        {
            get
            {
                string cookieExpirationDate = SettingsProvider.GetString("cookieExpirationDate");
                if(cookieExpirationDate == null)
                {
                    return DateTime.MinValue;
                }
                else
                {
                    return DateTime.Parse(cookieExpirationDate);
                }
            }
            set
            {
                 SettingsProvider.SetString("cookieExpirationDate", value.ToString());
            }
        }
	}
}

