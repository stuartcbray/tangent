using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Tangent
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{

		// class-level declarations
		UIWindow window;
		//
		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			this.window = new UIWindow (UIScreen.MainScreen.Bounds); 

			//---- instantiate a new navigation controller 
			var rootNavigationController = new UINavigationController(); 

			//---- instantiate a new home screen 
			WelcomeViewController welcomeScreen = new WelcomeViewController(); 

			//---- add the home screen to the navigation controller 
			// (it'll be the top most screen) 
			rootNavigationController.PushViewController(welcomeScreen, false); 

			//---- set the root view controller on the window. the nav 
			// controller will handle the rest 
			this.window.RootViewController = rootNavigationController; 
			this.window.MakeKeyAndVisible (); 

			app.RegisterForRemoteNotificationTypes(UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound);

			return true; 
		}

		public static string DeviceToken { get; private set; }


		public override void RegisteredForRemoteNotifications (UIApplication application, NSData deviceToken)
		{
			Console.WriteLine(deviceToken.Description);
			DeviceToken = deviceToken.Description.Replace("<","").Replace(">","");
		}

		public override void FailedToRegisterForRemoteNotifications (UIApplication application, NSError error)
		{
			Console.WriteLine(error);
		}

		public override void ReceivedRemoteNotification (UIApplication application, NSDictionary userInfo)
		{
			Console.WriteLine(userInfo);
		}
	}
}

