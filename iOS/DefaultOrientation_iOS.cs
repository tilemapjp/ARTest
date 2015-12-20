using System;
using Xamarin.Forms;
using ARTest.iOS;

[assembly: Dependency (typeof (DefaultOrientation_iOS))]

namespace ARTest.iOS
{
	public class DefaultOrientation_iOS
	{
		public DefaultOrientation_iOS ()
		{
		}

		public CameraOrientationConstant GetDefaultOrientation() {
			return CameraOrientationConstant.Portrait;
		}
	}
}

