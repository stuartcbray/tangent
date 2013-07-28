using System;
using System.Net;


namespace TheFactorM.Federation
{
    /// <summary>
    /// Contains device context information
    /// </summary>
    internal partial class DeviceContext
    {
        #region Private fields

        private static DeviceContext _instance;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the DeviceContext class.
        /// </summary>
        private DeviceContext()
        {
            // Do device specific initialization
            InitializeDeviceSpecific();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Executes an action on a background thread
        /// </summary>
        /// <param name="action">Action to execute</param>
        public void RunOnBackgroundThread(Action action)
        {
            InternalRunOnBackgroundThread(action);
        }

        /// <summary>
        /// Executes an action on a foreground thread
        /// </summary>
        /// <param name="action">Action to execute</param>
        public void RunOnForegroundThread(Action action)
        {
            InternalRunOnForegroundThread(action);
        }

		/// <summary>
		/// Shows the network activity indicator. 
		/// </summary>
		public void ShowNetworkActivityIndicator()
		{
			InternalShowNetworkActivityIndicator();
		}
		
		/// <summary>
		/// Hides the network activity indicator. 
		/// </summary>
		public void HideNetworkActivityIndicator()
		{
			InternalHideNetworkActivityIndicator();
		}
						        
        #endregion

        #region Singleton implementation

        /// <summary>
        /// Gets the current context
        /// </summary>
        public static DeviceContext Current
        {
            get
            {
				if (_instance == null)
				{
					_instance = new DeviceContext();
				}
                return _instance;
            }
        }

        #endregion

        #region Partial methods 

		/// <summary>
		/// Initializes a new instance. 
		/// </summary>
        partial void InitializeDeviceSpecific();
		
        /// <summary>
        /// Executes an action on a background thread
        /// </summary>
        /// <param name="action">Action to execute</param>
        partial void InternalRunOnBackgroundThread(Action action);

        /// <summary>
        /// Executes an action on a foreground thread
        /// </summary>
        /// <param name="action">Action to execute</param>
        partial void InternalRunOnForegroundThread(Action action);

		/// <summary>
		/// Shows the network activity indicator. 
		/// </summary>
		partial void InternalShowNetworkActivityIndicator();
		
		/// <summary>
		/// Hides the network activity indicator. 
		/// </summary>
		partial void InternalHideNetworkActivityIndicator();

		#endregion
    }
}