using System;
using CoreLocation;
using Xamarin.Forms;
using ARTest.iOS;
using UIKit;
using CoreMotion;
using Foundation;
using System.Threading.Tasks;

[assembly: Dependency (typeof (GeoLocator_iOS))]

namespace ARTest.iOS
{
	public class GeoLocator_iOS : IGeoLocator
	{
		public event MagnetEventHandler MagnetReceived;
		public event AccelEventHandler AccelReceived;
		public event LocationEventHandler LocationReceived;
		public event OffsetEventHandler OffsetReceived;
		public event GyroEventHandler GyroReceived;

		private readonly CLLocationManager locationMan;
		private readonly CMMotionManager motionMan;

		public GeoLocator_iOS ()
		{
			// GPS関連マネージャ生成
			locationMan = new CLLocationManager();
			// GPSの利用許可をユーザに問うダイアログ表示、許可済みの時はスルーされる
			locationMan.RequestWhenInUseAuthorization ();
			// モーションセンサ関連マネージャ生成
			motionMan = new CMMotionManager ();
		}

		public void Start()
		{
			// GPSリスナ登録
			locationMan.LocationsUpdated += (sender, e) => {
				if (this.LocationReceived != null) {
					var l = e.Locations[e.Locations.Length - 1];

					this.LocationReceived(this, new LocationEventArgs
						{
							Latitude = l.Coordinate.Latitude,
							Longitude = l.Coordinate.Longitude
						});
				}
			};
			// 磁気コンパスリスナ登録
			locationMan.UpdatedHeading += (sender, e) => {
				if (this.MagnetReceived != null) {
					this.MagnetReceived(this, new Matrix3EventArgs
						{
							X = e.NewHeading.X,
							Y = e.NewHeading.Y,
							Z = e.NewHeading.Z
						});
				}
				// iOSでは磁気偏角は磁気コンパスリスナから取得
				if (this.OffsetReceived != null) {
					this.OffsetReceived(this, new OffsetEventArgs
						{
							offset = e.NewHeading.TrueHeading - e.NewHeading.MagneticHeading
						});
				}
			}; 

			// GPS、磁気コンパス観測開始
			locationMan.StartUpdatingLocation ();
			locationMan.StartUpdatingHeading();

			// 加速度センサリスナ登録、観測開始
			motionMan.AccelerometerUpdateInterval = 0.1;
			motionMan.StartAccelerometerUpdates (NSOperationQueue.CurrentQueue, (data, error) =>
				{
					if (this.AccelReceived != null) {
						this.AccelReceived (this, new Matrix3EventArgs {
							X = data.Acceleration.X,
							Y = data.Acceleration.Y,
							Z = data.Acceleration.Z
						});
					}					
				});

			// ジャイロセンサリスナ登録、観測開始
			motionMan.GyroUpdateInterval = 0.1;
			motionMan.StartGyroUpdates (NSOperationQueue.CurrentQueue, (data, error) =>
				{
					if (this.GyroReceived != null) {
						this.GyroReceived (this, new Matrix3EventArgs {
							X = data.RotationRate.x,
							Y = data.RotationRate.y,
							Z = data.RotationRate.z
						});
					}					
				});
		}
	}
}