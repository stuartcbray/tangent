using System;
using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Widget;

namespace TheFactorM.Federation
{
    /// <summary>
    /// Android specific device context
    /// </summary>
    internal partial class DeviceContext
    {
        #region Private fields

        private Context _applicationContext;
        private Handler _uiThreadHandler;

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the application context
        /// </summary>
        [Preserve]
        internal Context ApplicationContext
        {
            get { return _applicationContext; }
        }

        #endregion

        #region Partial methods

        partial void InitializeDeviceSpecific()
        {
            // Initialize a bunch of Android specific components
            _applicationContext = Application.Context;
            _uiThreadHandler = new Handler(_applicationContext.MainLooper);
        }

        partial void InternalRunOnBackgroundThread(Action action)
        {
            action.BeginInvoke(null, null);
        }

        partial void InternalRunOnForegroundThread(Action action)
        {
            _uiThreadHandler.Post(action);
        }

        #endregion
    }
}