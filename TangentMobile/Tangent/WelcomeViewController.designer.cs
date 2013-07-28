// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace Tangent
{
	[Register ("WelcomeViewController")]
	partial class WelcomeViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIButton btnLogin { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton btnShowTangent { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel tangentLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (btnLogin != null) {
				btnLogin.Dispose ();
				btnLogin = null;
			}

			if (btnShowTangent != null) {
				btnShowTangent.Dispose ();
				btnShowTangent = null;
			}

			if (tangentLabel != null) {
				tangentLabel.Dispose ();
				tangentLabel = null;
			}
		}
	}
}
