using Windows.Storage;

namespace TheFactorM.Federation.Device.Settings
{
    internal partial class SettingsBase
    {
        partial void InitializeSettingsProvider()
        {
            _activeSettingsProvider = new SettingsProvider();
        }
    }

    /// <summary>
    /// WP7 settings provider
    /// </summary>
    public class SettingsProvider : ISettingsProvider
    {
        /// <summary>
        /// Gets a string from the isolated storage
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetString(string key)
        {
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey(key))
            {
                return ApplicationData.Current.LocalSettings.Values[key] as string;
            }
            return null;
        }

        /// <summary>
        /// Saves a string in the isolated storage
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetString(string key, string value)
        {
            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey(key))
            {
                ApplicationData.Current.LocalSettings.Values.Add(key, value);
            }
            else
            {
                ApplicationData.Current.LocalSettings.Values[key] = value;
            }
        }

        /// <summary>
        /// Gets a boolean from the isolated storage
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool GetBoolean(string key)
        {
            return GetBoolean(key, false);
        }

        /// <summary>
        /// Gets a boolean from the isolated storage
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool GetBoolean(string key, bool defaultIfNotFound)
        {
            bool value = false;

            if (ApplicationData.Current.LocalSettings.Values.ContainsKey(key))
            {
                var settingValue = ApplicationData.Current.LocalSettings.Values[key];
                if (settingValue.GetType() == typeof(bool))
                {
                    value = (bool)settingValue;
                }
            }
            else
            {
                value = defaultIfNotFound;
            }

            return value;
        }
        /// <summary>
        /// Saves a boolean in the isolated storage
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetBoolean(string key, bool value)
        {
            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey(key))
            {
                ApplicationData.Current.LocalSettings.Values.Add(key, value);
            }
            else
            {
                ApplicationData.Current.LocalSettings.Values[key] = value;
            }
        }

        /// <summary>
        /// Gets a integer from the isolated storage
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int GetInt32(string key)
        {
            int value = 0;

            if (ApplicationData.Current.LocalSettings.Values.ContainsKey(key))
            {
                var settingValue = ApplicationData.Current.LocalSettings.Values[key];
                if (settingValue.GetType() == typeof(int))
                {
                    value = (int)settingValue;
                }
            }

            return value;
        }

        /// <summary>
        /// Saves a integer value in the isolated storage
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetInt32(string key, int value)
        {
            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey(key))
            {
                ApplicationData.Current.LocalSettings.Values.Add(key, value);
            }
            else
            {
                ApplicationData.Current.LocalSettings.Values[key] = value;
            }
        }

        /// <summary>
        /// Gets a double from the isolated storage
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public double GetDouble(string key)
        {
            double value = 0;

            if (ApplicationData.Current.LocalSettings.Values.ContainsKey(key))
            {
                var settingValue = ApplicationData.Current.LocalSettings.Values[key];
                if (settingValue.GetType() == typeof(double))
                {
                    value = (double)settingValue;
                }
            }

            return value;
        }

        /// <summary>
        /// Saves a double in the isolated storage
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetDouble(string key, double value)
        {
            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey(key))
            {
                ApplicationData.Current.LocalSettings.Values.Add(key, value);
            }
            else
            {
                ApplicationData.Current.LocalSettings.Values[key] = value;
            }
        }

        /// <summary>
        /// Gets a long from the isolated storage
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long GetLong(string key)
        {
            long value = 0;

            if (ApplicationData.Current.LocalSettings.Values.ContainsKey(key))
            {
                var settingValue = ApplicationData.Current.LocalSettings.Values[key];
                if (settingValue.GetType() == typeof(long))
                {
                    value = (long)settingValue;
                }
            }

            return value;
        }

        /// <summary>
        /// Saves a long in the isolated storage
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetLong(string key, long value)
        {
            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey(key))
            {
                ApplicationData.Current.LocalSettings.Values.Add(key, value);
            }
            else
            {
                ApplicationData.Current.LocalSettings.Values[key] = value;
            }
        }
    }
}