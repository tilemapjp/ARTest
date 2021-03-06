﻿using System;
using Xamarin.Forms;
using ARTest.Android;
using Android.Content;
using Android.Locations;
using Android.Systems;
using Android.OS;
using Android.Hardware;
using Java.Util;
using System.Threading.Tasks;

[assembly: Dependency (typeof (GeoLocator_Android))]

namespace ARTest.Android
{
	public class GeoLocator_Android : IGeoLocator
	{
		public event MagnetEventHandler MagnetReceived;
		public event AccelEventHandler AccelReceived;
		public event LocationEventHandler LocationReceived;
		public event OffsetEventHandler OffsetReceived;
		public event GyroEventHandler GyroReceived;

		public void Start()
		{
			var context = Forms.Context;
			// GPS関連マネージャ生成
			var locationMan = context.GetSystemService(Context.LocationService) 
				as LocationManager;
			// モーションセンサ関連マネージャ生成
			var sensorMan = context.GetSystemService (Context.SensorService) 
				as SensorManager;
			// 加速度センサ
			var accel = sensorMan.GetDefaultSensor (SensorType.Accelerometer);
			// 磁気センサ
			var magnet = sensorMan.GetDefaultSensor (SensorType.MagneticField);
			// ジャイロセンサ
			var gyro = sensorMan.GetDefaultSensor (SensorType.Gyroscope);

			// GPSリスナ登録
			locationMan.RequestLocationUpdates(LocationManager.GpsProvider, 0, 0, 
				// GPSリスナ生成
				new MyLocationListener (latlng =>
					{
						if (this.LocationReceived != null) {
							this.LocationReceived(this, new LocationEventArgs 
								{
									Latitude = latlng.Latitude,
									Longitude = latlng.Longitude
								});
						}
						// Androidでは磁気偏角はGPSの経緯度の値より算出するため、GPSリスナ内で処理することが必要
						if (this.OffsetReceived != null) {
							// 地磁気計算オブジェクト生成
							var geomagnetic = new GeomagneticField (
								(float)latlng.Latitude, (float)latlng.Longitude, (float)latlng.Altitude, new Date().Time);
							
							this.OffsetReceived(this, new OffsetEventArgs 
								{
									// 磁気偏角の値
									offset = geomagnetic.Declination
								});
						}
					}));

			// モーションセンサリスナ生成
			var mySensorListner = new MySensorListner ( ev =>
				{
					var sensorType = ev.Sensor.Type;
					// センサタイプ毎に処理分け
					if (sensorType == SensorType.Accelerometer) {
						if (this.AccelReceived != null) {
							this.AccelReceived(this, new Matrix3EventArgs
								{
									// AndroidはiOS/Windowsと比較して、加速度センサの値の正負が逆になるため、補正
									X = ev.Values[0] * -1.0,
									Y = ev.Values[1] * -1.0,
									Z = ev.Values[2] * -1.0
								});
						}					
					} else if (sensorType == SensorType.MagneticField) {
						if (this.MagnetReceived != null) {
							this.MagnetReceived(this, new Matrix3EventArgs
								{
									X = ev.Values[0],
									Y = ev.Values[1],
									Z = ev.Values[2]
								});
						}						
					} else if (sensorType == SensorType.Gyroscope) {
						if (this.GyroReceived != null) {
							this.GyroReceived (this, new Matrix3EventArgs 
								{
									X = ev.Values[0],
									Y = ev.Values[1],
									Z = ev.Values[2]
								});
						}						
					}
				});

			// モーションセンサリスナ登録
			sensorMan.RegisterListener (mySensorListner, accel, SensorDelay.Normal);
			sensorMan.RegisterListener (mySensorListner, magnet, SensorDelay.Normal);
			sensorMan.RegisterListener (mySensorListner, gyro, SensorDelay.Normal);
		}

		// GPSリスナ
		class MyLocationListener : Java.Lang.Object, ILocationListener
		{
			private readonly Action<Location> _onLocationChanged;

			public MyLocationListener(Action<Location> onLocationChanged)
			{
				_onLocationChanged = onLocationChanged;
			}

			public void OnLocationChanged(Location location)
			{
				_onLocationChanged(location);
			}

			public void OnProviderDisabled(string provider) { }
			public void OnProviderEnabled(string provider) { }
			public void OnStatusChanged(string provider, 
				Availability status, Bundle extras)  { }
		}

		// モーションセンサリスナ
		class MySensorListner : Java.Lang.Object, ISensorEventListener
		{
			private readonly Action<SensorEvent> _onSensorChanged;

			public MySensorListner (Action<SensorEvent> onSensorChanged)
			{
				_onSensorChanged = onSensorChanged;
			}

			public void OnSensorChanged (SensorEvent e) { 
				_onSensorChanged(e);
			}

			public void OnAccuracyChanged (Sensor sensor, SensorStatus accuracy) { }
		}

	}
}