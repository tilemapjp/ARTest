using System;

namespace ARTest
{
	// デバイス毎のデフォルトのデバイス方向を取得するインタフェース
	// ※ Nexus 10 等では、デバイスの長辺が上になる（Landscape）など、モーションセンサの座標軸定義がスマホと異なるため
	public interface IDefaultOrientation
	{
		CameraOrientationConstant GetDefaultOrientation();
	}
}

