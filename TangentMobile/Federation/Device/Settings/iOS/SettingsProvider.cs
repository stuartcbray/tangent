using System;
using MonoTouch.Foundation;

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
	/// iOS specific settings provider 
	/// </summary>
	internal class SettingsProvider: ISettingsProvider
	{
		private static NSUserDefaults _userPreferences = NSUserDefaults.StandardUserDefaults;
		
		public SettingsProvider()
		{
			_userPreferences.Synchronize();
		}
		
		#region ISettingsProvider implementation
		public string GetString (string key)
		{
			return _userPreferences.StringForKey(key);
		}

		public void SetString (string key, string value)
		{
			_userPreferences.SetString(value, key);	
			_userPreferences.Synchronize();
		}
		
        /// <summary>
        /// Loads a boolean value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool GetBoolean(string key, bool defaultIfNotFound)
		{
			if (string.IsNullOrEmpty(_userPreferences.StringForKey(key)))
			{
				return defaultIfNotFound;
			}
			return _userPreferences.BoolForKey(key);
		}
		
		public bool GetBoolean (string key)
		{
			return _userPreferences.BoolForKey(key);
		}

		public void SetBoolean (string key, bool value)
		{
			_userPreferences.SetBool(value, key);
			_userPreferences.Synchronize();
		}

		public int GetInt32 (string key)
		{
			return _userPreferences.IntForKey(key);
		}

		public void SetInt32 (string key, int value)
		{
			_userPreferences.SetInt(value, key);
			_userPreferences.Synchronize();
		}

		public double GetDouble (string key)
		{
			return _userPreferences.DoubleForKey(key);
		}

		public void SetDouble (string key, double value)
		{
			_userPreferences.SetDouble(value, key);
			_userPreferences.Synchronize();
		}
		
		public void SetLong (string key, long value)
		{
			_userPreferences.SetString(value.ToString(), key);
			_userPreferences.Synchronize();
		}
		
		public long GetLong(string key)
		{
			long returnValue = 0;
			var longValue = _userPreferences.StringForKey(key);
			long.TryParse(longValue, out returnValue);
			return returnValue;
		}
		
		#endregion
    }
}

