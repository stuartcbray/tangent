using System.IO.IsolatedStorage;

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
            string value = null;
            IsolatedStorageSettings.ApplicationSettings.TryGetValue(key, out value);

            return value;
        }

        /// <summary>
        /// Saves a string in the isolated storage
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetString(string key, string value)
        {
            IsolatedStorageSettings.ApplicationSettings[key] = value;
            IsolatedStorageSettings.ApplicationSettings.Save();
        }

        /// <summary>
        /// Gets a boolean from the isolated storage
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool GetBoolean(string key)
        {
            bool value = false;
            IsolatedStorageSettings.ApplicationSettings.TryGetValue(key, out value);

            return value;
        }

        /// <summary>
        /// Gets a boolean from the isolated storage
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool GetBoolean(string key, bool defaultIfNotFound)
        {
            bool value = false;
            if (IsolatedStorageSettings.ApplicationSettings.TryGetValue(key, out value))
            {
                return value;
            }
            else
            {
                return defaultIfNotFound;
            }
        }
        /// <summary>
        /// Saves a boolean in the isolated storage
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetBoolean(string key, bool value)
        {
            IsolatedStorageSettings.ApplicationSettings[key] = value;
            IsolatedStorageSettings.ApplicationSettings.Save();
        }

        /// <summary>
        /// Gets a integer from the isolated storage
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int GetInt32(string key)
        {
            int value = 0;
            IsolatedStorageSettings.ApplicationSettings.TryGetValue(key, out value);

            return value;
        }

        /// <summary>
        /// Saves a integer value in the isolated storage
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetInt32(string key, int value)
        {
            IsolatedStorageSettings.ApplicationSettings[key] = value;
            IsolatedStorageSettings.ApplicationSettings.Save();
        }

        /// <summary>
        /// Gets a double from the isolated storage
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public double GetDouble(string key)
        {
            double value = 0;
            IsolatedStorageSettings.ApplicationSettings.TryGetValue(key, out value);

            return value;
        }

        /// <summary>
        /// Saves a double in the isolated storage
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetDouble(string key, double value)
        {
            IsolatedStorageSettings.ApplicationSettings[key] = value;
            IsolatedStorageSettings.ApplicationSettings.Save();
        }

        /// <summary>
        /// Gets a long from the isolated storage
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long GetLong(string key)
        {
            long value = 0;
            IsolatedStorageSettings.ApplicationSettings.TryGetValue(key, out value);

            return value;
        }

        /// <summary>
        /// Saves a long in the isolated storage
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetLong(string key, long value)
        {
            IsolatedStorageSettings.ApplicationSettings[key] = value;
            IsolatedStorageSettings.ApplicationSettings.Save();
        }
    }
}