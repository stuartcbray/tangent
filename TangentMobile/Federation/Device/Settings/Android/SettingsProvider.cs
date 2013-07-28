using Android.Content;
using Android.Preferences;
using Android.App;

namespace TheFactorM.Federation.Device.Settings
{
    internal partial class SettingsBase
    {
        partial void InitializeSettingsProvider()
        {
            _activeSettingsProvider = new SettingsProvider(Application.Context);
        }
    }

    /// <summary>
    /// Android specific settings provider
    /// </summary>
    internal class SettingsProvider : ISettingsProvider
    {
        ISharedPreferences _sharedPreferences;

        /// <summary>
        /// Initializes a new instance of the SettingsProvider class.
        /// </summary>
        public SettingsProvider(Context context)
        {
            _sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(context);
        }


        /// <summary>
        /// Loads a string value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetString(string key)
        {
            return _sharedPreferences.GetString(key, null);
        }

        /// <summary>
        /// Saves a string value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetString(string key, string value)
        {
            var editor = _sharedPreferences.Edit();

            editor.PutString(key, value);
            editor.Commit();
        }

        /// <summary>
        /// Loads a boolean value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool GetBoolean(string key)
        {
            return GetBoolean(key, false);
        }

        /// <summary>
        /// Loads a boolean value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public bool GetBoolean(string key, bool defaultValue)
        {
            return _sharedPreferences.GetBoolean(key, defaultValue);
        }

        /// <summary>
        /// Saves a boolean value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetBoolean(string key, bool value)
        {
            var editor = _sharedPreferences.Edit();

            editor.PutBoolean(key, value);
            editor.Commit();
        }

        /// <summary>
        /// Loads a numeric value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int GetInt32(string key)
        {
            return _sharedPreferences.GetInt(key, 0);
        }

        /// <summary>
        /// Saves a numeric value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetInt32(string key, int value)
        {
            var editor = _sharedPreferences.Edit();

            editor.PutInt(key, value);
            editor.Commit();
        }

        /// <summary>
        /// Loads a double value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <remarks>The monodroid implementation may cause precision loss.
        /// The value is actually stored as a floating precision instead
        /// of a double precision</remarks>
        public double GetDouble(string key)
        {
            return (double)_sharedPreferences.GetFloat(key, 0.0F);
        }

        /// <summary>
        /// Saves a double value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <remarks>The monodroid implementation may cause precision loss.
        /// The value is actually stored as a floating precision instead
        /// of a double precision</remarks>
        public void SetDouble(string key, double value)
        {
            var editor = _sharedPreferences.Edit();

            editor.PutFloat(key, (float)value);
            editor.Commit();
        }

        /// <summary>
        /// Loads a long value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long GetLong(string key)
        {
            return _sharedPreferences.GetLong(key, 0L);
        }

        /// <summary>
        /// Saves a long value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetLong(string key, long value)
        {
            var editor = _sharedPreferences.Edit();

            editor.PutLong(key, value);
            editor.Commit();
        }
    }
}