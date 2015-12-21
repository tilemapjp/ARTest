using System;

namespace ARTest
{
	// デバイスの方向を定義するenum
	public enum CameraOrientationConstant : int
	{
		Portrait = 0,
		LandscapeRightUp = 90,
		PortraitUpsideDown = 180,
		LandscapeLeftUp = -90
	}
		
	// カメラの方向を計算するクラス
	public class CameraOrientation
	{
		// シングルトン用
		private static CameraOrientation _manager = null;

		// 方角（北=0,東=90,南=180,西=270）
		public double Azimuth { get; private set; }
		// 仰角（正）/俯角（負）
		public double Pitch   { get; private set; }
		// 水平角（右回転正、左回転負）
		public double Roll    { get; private set; }

		// 加速度X,Y,Z
		public double AccelX  { get; private set; }
		public double AccelY  { get; private set; }
		public double AccelZ  { get; private set; }

		// 磁気X,Y,Z
		public double MagnetX { get; private set; }
		public double MagnetY { get; private set; }
		public double MagnetZ { get; private set; }

		// 磁気偏角
		public double AzimuthOffset { get; private set; }

		// 水平角をゼロとしたいデバイスの向きを、CameraOrientationConstantで指定。デフォルトはPortrait
		public CameraOrientationConstant WantScreenOrientation { get; private set; }

		private bool Guard = false;

		// Constructor
		public CameraOrientation (
			CameraOrientationConstant wantOrientation = CameraOrientationConstant.Portrait)
		{
			Azimuth = 0.0;
			Pitch   = 0.0;
			Roll    = 0.0;
			WantScreenOrientation = wantOrientation;
		}

		// Singleton method
		public static CameraOrientation manager(
			CameraOrientationConstant wantOrientation = CameraOrientationConstant.Portrait)
		{
			if (_manager == null) {
				_manager = new CameraOrientation (wantOrientation);
			}
			return _manager;
		}

		// 加速度の3軸値を変更がかかったタイミングで更新
		public void SetAccelValues(double X, double Y, double Z)
		{
			AccelX = X;
			AccelY = Y;
			AccelZ = Z;
		}

		// 磁気の3軸値を変更がかかったタイミングで更新
		public void SetMagnetValues(double X, double Y, double Z)
		{
			MagnetX = X;
			MagnetY = Y;
			MagnetZ = Z;
		}

		// 磁気偏角を変更がかかったタイミングで更新
		public void SetOffsetValue (double offset)
		{
			AzimuthOffset = offset * Math.PI / 180.0;
		}

		// 絶対値計算用
		static public double GetAbsoluteValue3D(double x, double y, double z)
		{
			return Math.Sqrt (x * x + y * y + z * z);
		}
			
		// カメラの方向計算 (http://qiita.com/kochizufan/items/16bc7524105a5e4617be)
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
				Roll = tRoll + ((double)WantScreenOrientation * Math.PI / 180.0);
				if (Roll > Math.PI) {
					Roll -= Math.PI * 2.0;
				} else if (Roll <= -1.0 * Math.PI) {
					Roll += Math.PI * 2.0;
				}
			}

			Guard = false;
			return true;
		}
	}
}

