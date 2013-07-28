
namespace TheFactorM.Federation.Device.Settings
{
    /// <summary>
    /// Contract for a platform specific settings provider
    /// </summary>
    internal interface ISettingsProvider
    {
        /// <summary>
        /// Loads a string value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string GetString(string key);

        /// <summary>
        /// Saves a string value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void SetString(string key, string value);

        /// <summary>
        /// Loads a boolean value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool GetBoolean(string key);
        /// <summary>
        /// Loads a boolean value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool GetBoolean(string key, bool defaultIfNotFound);
        /// <summary>
        /// Saves a boolean value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void SetBoolean(string key, bool value);

        /// <summary>
        /// Loads a numeric value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        int GetInt32(string key);

        /// <summary>
        /// Saves a numeric value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void SetInt32(string key, int value);

        /// <summary>
        /// Loads a double value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        double GetDouble(string key);

        /// <summary>
        /// Saves a double value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void SetDouble(string key, double value);

        /// <summary>
        /// Loads a long value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        long GetLong(string key);

        /// <summary>
        /// Saves a long value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void SetLong(string key, long value);
    }
}