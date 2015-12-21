using System;
using System.Threading.Tasks;

namespace ARTest
{
	// 経緯度イベントのパラメータ
	public class LocationEventArgs : EventArgs
	{
		public double Latitude { get; set; }
		public double Longitude { get; set; }
	}

	// 3軸イベントのパラメータ
	public class Matrix3EventArgs : EventArgs
	{
		public double X { get; set; }
		public double Y { get; set; }
		public double Z { get; set; } 
	}

	// 単独値（磁気偏角）イベントのパラメータ
	public class OffsetEventArgs : EventArgs
	{
		public double offset { get; set; }		
	}

	// 位置データを受信した際のイベントハンドラ
	public delegate void LocationEventHandler(object sender, LocationEventArgs args);
	// 磁気データを受信した際のイベントハンドラ
	public delegate void MagnetEventHandler(object sender, Matrix3EventArgs args);
	// 加速度データを受信した際のイベントハンドラ
	public delegate void AccelEventHandler(object sender, Matrix3EventArgs args);
	// ジャイロデータを受信した際のイベントハンドラ (参考)
	public delegate void GyroEventHandler(object sender, Matrix3EventArgs args);
	// 磁気偏角データを受信した際のイベントハンドラ
	public delegate void OffsetEventHandler(object sender, OffsetEventArgs args);

	// デバイスモーションを利用するための、共通なインターフェース
	public interface IGeoLocator
	{
		void Start();
		event MagnetEventHandler MagnetReceived;
		event AccelEventHandler AccelReceived;
		event GyroEventHandler GyroReceived;
		event LocationEventHandler LocationReceived;
		event OffsetEventHandler OffsetReceived;
	}
}

