using System;
using System.Threading;

using Xamarin.Forms;

namespace ARTest
{
	public class App : Application
	{
		IGeoLocator geoLocator;
		CameraOrientation cameraCalc;

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

		// レイアウト作成
		public App ()
		{
			// The root page of your application
			MainPage = new ContentPage {
				Content = new StackLayout {
					VerticalOptions = LayoutOptions.Center,
					Children = {
						CreateLabel("Lat/Long"),
						labelLatLon,
						CreateLabel("Magnetic Field"),							
						labelMagnet,
						CreateLabel("Accelerometer"),							
						labelAccel,
						CreateLabel("Direction"),
						labelDirection,
						CreateLabel("Elevation"),
						labelElevation,
						CreateLabel("Horizontal angle"),
						labelHorizontal,
						CreateLabel("True North Offset"),
						labelTrueNorth,
						CreateLabel("Gyro Sensor"),
						labelGyro
					}
				}
			};
		}

		protected Label CreateLabel(String label) {
			return new Label {
				HorizontalTextAlignment = TextAlignment.Center,
				Text = label
			};
		}

		// 計算結果のUI書き込み
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

			// 位置データを受信した際の処理
			geoLocator.LocationReceived += (_, args) => {
				labelLatLon.Text = String.Format("{0:0.00000},{1:0.00000}", args.Latitude, args.Longitude);
			};
			// 磁気データを受信した際の処理
			geoLocator.MagnetReceived += (_, args) => {
				cameraCalc.SetMagnetValues(args.X,args.Y,args.Z);
				cameraCalc.CalcurateOrientation();
				if (cameraCalc.CalcurateOrientation()) {
					WriteToText();
				}
			};
			// 加速度データを受信した際の処理
			geoLocator.AccelReceived += (_, args) => {
				cameraCalc.SetAccelValues(args.X,args.Y,args.Z);
				cameraCalc.CalcurateOrientation();
				if (cameraCalc.CalcurateOrientation()) {
					WriteToText();
				}
			};
			// 磁気偏角データを受信した際の処理
			geoLocator.OffsetReceived += (_, args) => {
				cameraCalc.SetOffsetValue(args.offset);
				if (cameraCalc.CalcurateOrientation()) {
					WriteToText();
				}
			};
			// ジャイロデータを受信した際の処理
			geoLocator.GyroReceived += (_, args) => {
				labelGyro.Text = String.Format ("X:{0:0.000} Y:{1:0.000} Z: {2:0.000}", args.X * 180 / Math.PI, args.Y * 180 / Math.PI, args.Z * 180 / Math.PI);
			};
				
			// 観測開始
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

