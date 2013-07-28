using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Threading;
using TheFactorM.Fedearation;

namespace TheFactorM.Federation
{
    internal partial class DeviceContext
    {
        /// <summary>
        /// Initializes the device context
        /// </summary>
        partial void InitializeDeviceSpecific()
        {
        }

        /// <summary>
        /// Executes an action on a background thread
        /// </summary>
        /// <param name="action">Action to execute</param>
        partial void InternalRunOnBackgroundThread(Action action)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback((o) => action()));
        }

        /// <summary>
        /// Executes an action on a foreground thread
        /// </summary>
        /// <param name="action">Action to execute</param>
        partial void InternalRunOnForegroundThread(Action action)
        {
            Deployment.Current.Dispatcher.BeginInvoke(action);
        }

        partial void InternalShowNetworkActivityIndicator()
        {
            DeviceContext.Current.RunOnForegroundThread(() =>
                {
                    GlobalLoading.Instance.IsLoading = true;
                });
        }
        partial void InternalHideNetworkActivityIndicator()
        {
            DeviceContext.Current.RunOnForegroundThread(() =>
                {
                    GlobalLoading.Instance.IsLoading = false;
                });
        }


    }
}
