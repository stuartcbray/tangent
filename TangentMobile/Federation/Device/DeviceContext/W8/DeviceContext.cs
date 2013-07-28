using System;
using System.Net;
using System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using System.Net.NetworkInformation;
using Windows.UI.Popups;
using Windows.System.Threading;
using System.Threading.Tasks;

namespace TheFactorM.Federation
{
    internal partial class DeviceContext
    {
        /// <summary>
        /// Gets or sets the dispatcher to be used for thread handling.
        /// </summary>
        public CoreDispatcher Dispatcher { get; set; }

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
            Task.Run(action);
        }

        /// <summary>
        /// Executes an action on a foreground thread
        /// </summary>
        /// <param name="action">Action to execute</param>
        partial void InternalRunOnForegroundThread(Action action)
        {

            if (Dispatcher == null)
            {
                throw new InvalidOperationException("Must set Dispatcher before using the runOnForegroundthread operation on Windows 8!");
            }

            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(action));
        }

        partial void InternalShowNetworkActivityIndicator()
        {
           // not implemented yet
        }
        partial void InternalHideNetworkActivityIndicator()
        {
          //not implemented yet  
        }
    }
}
