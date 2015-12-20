using System;
//using MonoTouch.UIKit;
//using MonoTouch.CoreLocation;

namespace ARTest
{
	public enum CameraOrientationConstant : int
	{
		Portrait = 0,
		LandscapeRightUp = 90,
		PortraitUpsideDown = 180,
		LandscapeLeftUp = -90
	}
		
	public class CameraOrientation
	{
		private static CameraOrientation _manager = null;

		public double Azimuth { get; private set; }
		public double Pitch   { get; private set; }
		public double Roll    { get; private set; }

		public double AccelX  { get; private set; }
		public double AccelY  { get; private set; }
		public double AccelZ  { get; private set; }

		public double MagnetX { get; private set; }
		public double MagnetY { get; private set; }
		public double MagnetZ { get; private set; }

		//public UIDeviceOrientation Orientation { get; private set; }

		public double AzimuthOffset { get; private set; }

		public CameraOrientationConstant ScreenOrientation { get; private set; }

		private bool Guard = false;

		public CameraOrientation ()
		{
			Azimuth = 0.0;
			Pitch   = 0.0;
			Roll    = 0.0;
			ScreenOrientation = CameraOrientationConstant.LandscapeRightUp;
			//Orientation = UIDeviceOrientation.Portrait;
		}

		public static CameraOrientation manager()
		{
			if (_manager == null) {
				_manager = new CameraOrientation ();
			}
			return _manager;
		}

		public void SetScreenOrientation (CameraOrientationConstant orientation)
		{
			ScreenOrientation = orientation;
		}

		public void SetAccelValues(double X, double Y, double Z)//(UIAcceleration accel)
		{
			AccelX = X;
			AccelY = Y;
			AccelZ = Z;
		}

		public void SetMagnetValues(double X, double Y, double Z)//(CLHeading heading)
		{
			MagnetX = X;
			MagnetY = Y;
			MagnetZ = Z;
		}

		public void SetOffsetValue (double offset)
		{
			AzimuthOffset = offset * Math.PI / 180.0;
		}

		static public double GetAbsoluteValue3D(double x, double y, double z)
		{
			return Math.Sqrt (x * x + y * y + z * z);
		}

		/*public void CalcurateOrientation () {
			Console.WriteLine ("Bx {0} By {1} Bz {2} Ax {3} Ay {4} Az {5}", MagnetX, MagnetY, MagnetZ, AccelX, AccelY, AccelZ);
			double hX, hY, hZ;
			double mX, mY, mZ;
			double aX, aY, aZ;

			double accelAbsolute  = GetAbsoluteValue3D (AccelX,  AccelY,  AccelZ);
			double magnetAbsolute = GetAbsoluteValue3D (MagnetX, MagnetY, MagnetZ);

			hX = -(MagnetY * AccelZ - MagnetZ * AccelY) / (accelAbsolute * magnetAbsolute);
			hY = -(MagnetX * AccelZ - MagnetZ * AccelX) / (accelAbsolute * magnetAbsolute);
			hZ = -(MagnetX * AccelY - MagnetY * AccelX) / (accelAbsolute * magnetAbsolute);

			mX = (MagnetX * (AccelY * AccelY - AccelZ * AccelZ) -
				MagnetY * AccelX * AccelY - MagnetZ * AccelZ * AccelX)
				/ (accelAbsolute * accelAbsolute * magnetAbsolute);
			mY = (MagnetY * (AccelZ * AccelZ - AccelX * AccelX) -
				MagnetZ * AccelY * AccelZ - MagnetX * AccelX * AccelY)
				/ (accelAbsolute * accelAbsolute * magnetAbsolute);
			mZ = (MagnetZ * (AccelX * AccelX - AccelY * AccelY) -
				MagnetX * AccelZ * AccelX - MagnetY * AccelY * AccelZ)
				/ (accelAbsolute * accelAbsolute * magnetAbsolute);

			aX = -AccelX / accelAbsolute;
			aY = -AccelY / accelAbsolute;
			aZ = -AccelZ / accelAbsolute;

			//Azimuth = -Math.Atan2 (mX, hX) + AzimuthOffset;
			Azimuth = Math.Atan2 (-mZ, -hZ) + AzimuthOffset;

			while (Azimuth < 0)
				Azimuth += Math.PI * 2.0;
			//Pitch   = -Math.Asin (aZ);
			Pitch = Math.Asin (-aZ);
			//Roll    = Math.Atan2 (aX, aY);
			Roll = Math.Atan2 (aX, -aY);

			//Console.WriteLine ("{0} {1} {2}", Azimuth, Pitch, Roll);
		}*/

		public bool CalcurateOrientation () {
			double a, b, c, d, e, f, g, h, i;

			double accelAbsolute  = GetAbsoluteValue3D (AccelX,  AccelY,  AccelZ);
			double magnetAbsolute = GetAbsoluteValue3D (MagnetX, MagnetY, MagnetZ);

			if (Guard) {
				return false;
			} else {
				Guard = true;
			}

			a = -(MagnetY * AccelZ - MagnetZ * AccelY) / (accelAbsolute * magnetAbsolute);
			b = -(MagnetZ * AccelX - MagnetX * AccelZ) / (accelAbsolute * magnetAbsolute);
			c = -(MagnetX * AccelY - MagnetY * AccelX) / (accelAbsolute * magnetAbsolute);

			d = (MagnetX * (AccelY * AccelY + AccelZ * AccelZ) -
				MagnetY * AccelX * AccelY - MagnetZ * AccelZ * AccelX)
				/ (accelAbsolute * accelAbsolute * magnetAbsolute);
			e = (MagnetY * (AccelZ * AccelZ + AccelX * AccelX) -
				MagnetZ * AccelY * AccelZ - MagnetX * AccelX * AccelY)
				/ (accelAbsolute * accelAbsolute * magnetAbsolute);
			f = (MagnetZ * (AccelX * AccelX + AccelY * AccelY) -
				MagnetX * AccelZ * AccelX - MagnetY * AccelY * AccelZ)
				/ (accelAbsolute * accelAbsolute * magnetAbsolute);

			g = -AccelX / accelAbsolute;
			h = -AccelY / accelAbsolute;
			i = -AccelZ / accelAbsolute;

			//ここはデバイス座標
			/*Azimuth = Math.Atan2 (b, e) + AzimuthOffset;
			while (Azimuth < 0)
				Azimuth += Math.PI * 2.0;
			Pitch = Math.Asin (h);
			Roll = Math.Atan2 (-g, i);*/

			//ここはカメラ座標(縦)
			var tAzimuth = Math.Atan2 (c, f) - Math.PI + AzimuthOffset;
			if (!Double.IsNaN (tAzimuth)) {
				Azimuth = tAzimuth;
				while (Azimuth < 0)
					Azimuth += Math.PI * 2.0;
			}
			var tPitch = Math.Asin (-i);
			if (!Double.IsNaN (tPitch))
				Pitch = tPitch;
			var tRoll = Math.Atan2 (-g, h);
			if (!Double.IsNaN (tRoll)) {
				Roll = tRoll + ((double)ScreenOrientation * Math.PI / 180.0);
				if (Roll >= Math.PI) {
					Roll -= Math.PI * 2.0;
				}
				/*var edge = 0.5235987755982988f;
				if (Math.Abs (Roll) < edge && Orientation != UIDeviceOrientation.Portrait) {
					Orientation = UIDeviceOrientation.Portrait;
				} else if (Roll < -2.0f * edge && Orientation != UIDeviceOrientation.LandscapeLeft) {
					Orientation = UIDeviceOrientation.LandscapeLeft;
				} else if (Roll > 2.0f * edge && Orientation != UIDeviceOrientation.LandscapeRight) {
					Orientation = UIDeviceOrientation.LandscapeRight;
				}
				Console.WriteLine ("Orientation {0}",Orientation);*/
			}

			Guard = false;
			return true;

			//Console.WriteLine ("{0} {1} {2}", Azimuth, Pitch, Roll);
		}
	}
}

