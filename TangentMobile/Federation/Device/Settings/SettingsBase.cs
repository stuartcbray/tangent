using System;

namespace TheFactorM.Federation.Device.Settings
{
    /// <summary>
    /// Basis skeleton for a settings container
    /// </summary>
    internal abstract partial class SettingsBase
    {
        #region Private fields

        private ISettingsProvider _activeSettingsProvider;

        #endregion

        #region Initialization logic

        /// <summary>
        /// Initializes the settings provider
        /// </summary>
        partial void InitializeSettingsProvider();

        /// <summary>
        /// Initializes the settings
        /// </summary>
        internal void Initialize()
        {
            InitializeSettingsProvider();
        }

        #endregion

        #region Protected properties

        /// <summary>
        /// Gets the active settings provider
        /// </summary>
        protected ISettingsProvider SettingsProvider
        {
            get
            {
                return _activeSettingsProvider;
            }
        }
        
        #endregion

    }

    /// <summary>
    /// Generic settings container. Implement this in the application
    /// to provide a way to store and retrieve application settings
    /// </summary>
    /// <typeparam name="TSettings"></typeparam>
    internal abstract class SettingsBase<TSettings>: SettingsBase
        where TSettings : SettingsBase, new()
    {
        #region Singleton implementation

        private static TSettings _instance;
        private static object _lockHandle = new object();

        /// <summary>
        /// Gets the current instance of the settings class
        /// </summary>
        public static TSettings Current
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockHandle)
                    {
                        if (_instance == null)
                        {
                            _instance = new TSettings();
                            _instance.Initialize();
                        }
                    }
                }

                return _instance;
            }
        }

        #endregion
    }
}