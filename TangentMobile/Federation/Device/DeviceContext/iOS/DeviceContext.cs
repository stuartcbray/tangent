using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Threading;

namespace TheFactorM.Federation
{
	internal partial class DeviceContext 
	{
        partial void InitializeDeviceSpecific()
		{
		}
		
        /// <summary>
        /// Executes an action on a background thread
        /// </summary>
        /// <param name="action">Action to execute</param>
        partial void InternalRunOnBackgroundThread(Action action)
		{
			ThreadPool.QueueUserWorkItem (delegate {
				try {
					action ();
				} catch (Exception error) {
					Console.WriteLine(error);
					throw;
				}
			});			
		}

        /// <summary>
        /// Executes an action on a foreground thread
        /// </summary>
        /// <param name="action">Action to execute</param>
        partial void InternalRunOnForegroundThread(Action action)
		{
			UIApplication.SharedApplication.BeginInvokeOnMainThread(() => action());
		}
		
		partial void InternalShowNetworkActivityIndicator ()
		{
			UIApplication.SharedApplication.InvokeOnMainThread(
				() => UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true );
		}
		
		partial void InternalHideNetworkActivityIndicator ()
		{
			UIApplication.SharedApplication.InvokeOnMainThread(
				() => UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false );
		}
	}
}

