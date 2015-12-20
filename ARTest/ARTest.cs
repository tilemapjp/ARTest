using System;
using System.Threading;

using Xamarin.Forms;

namespace ARTest
{
	public class App : Application
	{
		IGeoLocator geoLocator;
		CameraOrientation cameraCalc;

		Button buttonStartGps = new Button
		{
			Text = "Start GPS"
		};

		Label labelLatLon = new Label
		{
			HorizontalTextAlignment = TextAlignment.Center
		};
		Label labelDirection = new Label
		{
			HorizontalTextAlignment = TextAlignment.Center
		};
		Label labelElevation = new Label
		{
			HorizontalTextAlignment = TextAlignment.Center
		};
		Label labelHorizontal = new Label
		{
			HorizontalTextAlignment = TextAlignment.Center
		};
		Label labelAccel = new Label
		{
			HorizontalTextAlignment = TextAlignment.Center
		};
		Label labelMagnet = new Label
		{
			HorizontalTextAlignment = TextAlignment.Center
		};
		Label labelTrueNorth = new Label
		{
			HorizontalTextAlignment = TextAlignment.Center
		};
		Label labelGyro = new Label
		{
			HorizontalTextAlignment = TextAlignment.Center
		};

		public App ()
		{
			// The root page of your application
			MainPage = new ContentPage {
				Content = new StackLayout {
					VerticalOptions = LayoutOptions.Center,
					Children = {
						new Label {
							HorizontalTextAlignment = TextAlignment.Center,
							Text = "Lat/Long"
						},
						labelLatLon,
						new Label {
							HorizontalTextAlignment = TextAlignment.Center,
							Text = "Magnetic Field"							
						},
						labelMagnet,
						new Label {
							HorizontalTextAlignment = TextAlignment.Center,
							Text = "Accelerometer"							
						},
						labelAccel,
						new Label {
							HorizontalTextAlignment = TextAlignment.Center,
							Text = "Direction"
						},
						labelDirection,
						new Label {
							HorizontalTextAlignment = TextAlignment.Center,
							Text = "Elevation"
						},
						labelElevation,
						new Label {
							HorizontalTextAlignment = TextAlignment.Center,
							Text = "Horizontal angle"
						},
						labelHorizontal,
						new Label {
							HorizontalTextAlignment = TextAlignment.Center,
							Text = "True North Offset"
						},
						labelTrueNorth,
						new Label {
							HorizontalTextAlignment = TextAlignment.Center,
							Text = "Gyro Sensor"
						},
						labelGyro
					}
				}
			};
		}

		protected void WriteToText()
		{
			labelMagnet.Text = String.Format ("X:{0:0.000} Y:{1:0.000} Z: {2:0.000}", cameraCalc.MagnetX, cameraCalc.MagnetY, cameraCalc.MagnetZ);
			labelAccel.Text = String.Format ("X:{0:0.000} Y:{1:0.000} Z: {2:0.000}", cameraCalc.AccelX, cameraCalc.AccelY, cameraCalc.AccelZ);
			labelDirection.Text = String.Format("{0:0.00000}", cameraCalc.Azimuth * 180 / Math.PI);
			labelElevation.Text = String.Format("{0:0.00000}", cameraCalc.Pitch * 180 / Math.PI);
			labelHorizontal.Text = String.Format("{0:0.00000}", cameraCalc.Roll * 180 / Math.PI);
			labelTrueNorth.Text = String.Format("{0:0.00000}", cameraCalc.AzimuthOffset);
		}

		protected override async void OnStart ()
		{
			geoLocator = DependencyService.Get<IGeoLocator>();
			cameraCalc = new CameraOrientation ();

			geoLocator.LocationReceived += (_, args) => {
				labelLatLon.Text = String.Format("{0:0.00000},{1:0.00000}", args.Latitude, args.Longitude);
			};
			geoLocator.MagnetReceived += (_, args) => {
				cameraCalc.SetMagnetValues(args.X,args.Y,args.Z);
				cameraCalc.CalcurateOrientation();
				if (cameraCalc.CalcurateOrientation()) {
					WriteToText();
				}
			};
			geoLocator.AccelReceived += (_, args) => {
				cameraCalc.SetAccelValues(args.X,args.Y,args.Z);
				cameraCalc.CalcurateOrientation();
				if (cameraCalc.CalcurateOrientation()) {
					WriteToText();
				}
			};
			geoLocator.OffsetReceived += (_, args) => {
				cameraCalc.SetOffsetValue(args.offset);
				if (cameraCalc.CalcurateOrientation()) {
					WriteToText();
				}
			};
			geoLocator.GyroReceived += (_, args) => {
				labelGyro.Text = String.Format ("X:{0:0.000} Y:{1:0.000} Z: {2:0.000}", args.X * 180 / Math.PI, args.Y * 180 / Math.PI, args.Z * 180 / Math.PI);
			};
				
			await geoLocator.StartAsync();
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}

