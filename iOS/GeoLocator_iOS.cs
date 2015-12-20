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
			locationMan = new CLLocationManager();
			locationMan.RequestWhenInUseAuthorization ();
			motionMan = new CMMotionManager ();
		}

		public async Task StartAsync()
		{
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
			locationMan.UpdatedHeading += (sender, e) => {
				if (this.MagnetReceived != null) {
					this.MagnetReceived(this, new Matrix3EventArgs
						{
							X = e.NewHeading.X,
							Y = e.NewHeading.Y,
							Z = e.NewHeading.Z
						});
				}
				if (this.OffsetReceived != null) {
					this.OffsetReceived(this, new OffsetEventArgs
						{
							offset = e.NewHeading.TrueHeading - e.NewHeading.MagneticHeading
						});
				}
			}; 

			locationMan.StartUpdatingLocation ();
			locationMan.StartUpdatingHeading();
			/*motionMan.MagnetometerUpdateInterval = 0.1;
			motionMan.StartMagnetometerUpdates (NSOperationQueue.CurrentQueue, (data, CLError) => 
				{
					if (this.MagnetReceived != null) {
						this.MagnetReceived (this, new Matrix3EventArgs {
							X = data.MagneticField.X,
							Y = data.MagneticField.Y,
							Z = data.MagneticField.Z
						});
					}					
				});*/
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