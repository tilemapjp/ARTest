using System;
using System.Threading.Tasks;

namespace ARTest
{
	// 経緯度イベントのパラメーター
	public class LocationEventArgs : EventArgs
	{
		public double Latitude { get; set; }
		public double Longitude { get; set; }
	}

	public class Matrix3EventArgs : EventArgs
	{
		public double X { get; set; }
		public double Y { get; set; }
		public double Z { get; set; } 
	}

	public class OffsetEventArgs : EventArgs
	{
		public double offset { get; set; }		
	}

	// 位置を受信した際のイベントハンドラー
	public delegate void LocationEventHandler(object sender, LocationEventArgs args);
	public delegate void MagnetEventHandler(object sender, Matrix3EventArgs args);
	public delegate void AccelEventHandler(object sender, Matrix3EventArgs args);
	public delegate void GyroEventHandler(object sender, Matrix3EventArgs args);
	public delegate void OffsetEventHandler(object sender, OffsetEventArgs args);

	// GPS を利用するための、共通なインターフェース
	public interface IGeoLocator
	{
		Task StartAsync();
		event MagnetEventHandler MagnetReceived;
		event AccelEventHandler AccelReceived;
		event GyroEventHandler GyroReceived;
		event LocationEventHandler LocationReceived;
		event OffsetEventHandler OffsetReceived;
	}
}

