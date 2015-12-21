# ARTest
[タイムマシンカメラ](http://gugen.jp/entry2015/076)用に開発中のもの

## 現状

[以前のQiita投稿](http://qiita.com/kochizufan/items/16bc7524105a5e4617be)をベースに、
* ロジックだけではなくセンサ取得部分も追加
* Androidにも対応
* Xamarin.Forms利用
してぼちぼちと開発中。
未完成（カメラ画像取り込み等は出来ておらず、方角/仰俯角/水平角の計算結果が表示されるのみ）だが、カメラの向き特定ロジックとしては一定の精度(某AR処理市販本よりは高精度)出ているので、そこだけでも役立つかと思い[Xamarin Advent Calendar 2015](http://qiita.com/advent-calendar/2015/xamarin)に合わせ公開。

## コーディング上気をつけることの説明

### 地磁気の取得
地磁気の取得は、iOSでは[GPS系マネージャ経由](https://github.com/tilemapjp/ARTest/blob/advent2015/iOS/GeoLocator_iOS.cs#L50-L66)、Androidでは[デバイスモーション系マネージャ経由](https://github.com/tilemapjp/ARTest/blob/advent2015/Droid/GeoLocator_Android.cs#L81-L89)で取得する。  
正確には、iOSのデバイスモーション系マネージャにも磁気のハンドラはあるが、こちらを使うとうまくいかない（原因未確認）。

### 磁気偏角の取得
磁気偏角の取得は、iOSでは[地磁気の値の更新時にイベントオブジェクトのプロパティとして取得可能](https://github.com/tilemapjp/ARTest/blob/advent2015/iOS/GeoLocator_iOS.cs#L63)だが、Androidでは[経緯度の値の更新時に、その経緯度の値から計算して取得](https://github.com/tilemapjp/ARTest/blob/advent2015/Droid/GeoLocator_Android.cs#L52-L63)する。

### 加速度センサの正負の方向
iOSとAndroidでは、加速度センサの吐く値の正負の方向が異なる。  
WindowsはiOSと同じ、との情報があったので、iOS側を正とし[Android側を正負逆転](https://github.com/tilemapjp/ARTest/blob/advent2015/Droid/GeoLocator_Android.cs#L73-L79)して、同じ計算結果が出るようにしている。

## 今後行うべきこと

* OnSleepでの観測の停止
* 子スレッドでの監視に移行
* AR機能そのものを実装
