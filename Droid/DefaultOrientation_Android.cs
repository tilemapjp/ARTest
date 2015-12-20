using System;
using Xamarin.Forms;
using ARTest.Droid;
using Android.Views;
using Android.Content;
using Android.Content.Res;

[assembly: Dependency (typeof (DefaultOrientation_Android))]

namespace ARTest.Droid
{
	public class DefaultOrientation_Android
	{
		public DefaultOrientation_Android ()
		{
		}

		public CameraOrientationConstant GetDefaultOrientation() {
			var context = Forms.Context;
			var windowManager = context.GetSystemService(Context.WindowService) 
				as IWindowManager;
			var config = context.Resources.Configuration;
			var rotation = windowManager.DefaultDisplay.Rotation;

			if (((rotation == SurfaceOrientation.Rotation0 || rotation == SurfaceOrientation.Rotation180) &&
			    config.Orientation == Orientation.Landscape)
			    || ((rotation == SurfaceOrientation.Rotation90 || rotation == SurfaceOrientation.Rotation270) &&
			    config.Orientation == Orientation.Portrait)) {
				return CameraOrientationConstant.LandscapeRightUp;
			} else {
				return CameraOrientationConstant.Portrait;
			}
		}
	}
}

