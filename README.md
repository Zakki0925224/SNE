# SimpleNotesEditor
![lisence](https://img.shields.io/github/license/Zakki0925224/SNE)
![latest-version](https://img.shields.io/github/v/release/Zakki0925224/SNE?include_prereleases)
![lisence](https://img.shields.io/github/downloads/Zakki0925224/SNE/total)

音ゲーの譜面を作るシンプルなエディター。mp3形式の音楽ファイルからjson形式の譜面を出力します。

**ロングノーツ・スライドノーツ等**には対応していません。

## ScreenShots
![](https://user-images.githubusercontent.com/49384910/134484873-7642c493-ecfe-42d4-8ec8-145936c5b31d.png)
![](https://user-images.githubusercontent.com/49384910/134484960-0bb23b40-a121-4c0e-8a1d-3a7cecb4f04d.png)
* 水平方向の拡大率を利用しているため、BPM値を変更すると横幅が縮小・拡大されます

## How to use
* mp3形式の音楽ファイルを選択
* 白い十字線にマウスを動かすと黄色いノーツポインタが出現するので、その状態でポインタ外をクリックするとノーツ配置
* 配置されているノーツにポインタを合わせ、配置時と同様にクリックするとノーツ削除
* 編集途中の譜面をプロジェクト（.sneproj）として保存・復元が可能（mp3ファイルのパスを変更する場合、プロジェクトファイルを編集する必要があります）
* json形式での譜面の出力（プロジェクトに使用したmp3ファイル+出力したjsonファイルを使って音ゲーに譜面を実装します）

## Exported musical score sample
```json
{
    "Title":"music title",
    "Description":"music description",
    "BPM":120,
    "GUID":"375ccf82-5983-4038-8a8d-9b32fe6e1b9b",
    "NotesData":[
        {"Time":0.5,"LaneID":1,"IsActionNote":false,"DifficultyLevel":0},
        {"Time":1.0,"LaneID":2,"IsActionNote":false,"DifficultyLevel":0},
        {"Time":1.5,"LaneID":3,"IsActionNote":false,"DifficultyLevel":0},
        {"Time":2.0,"LaneID":1,"IsActionNote":false,"DifficultyLevel":1},
        {"Time":2.5,"LaneID":2,"IsActionNote":false,"DifficultyLevel":1},
        {"Time":3.0,"LaneID":3,"IsActionNote":false,"DifficultyLevel":1},
        {"Time":4.5,"LaneID":1,"IsActionNote":false,"DifficultyLevel":2},
        {"Time":5.0,"LaneID":2,"IsActionNote":false,"DifficultyLevel":2},
        {"Time":5.5,"LaneID":3,"IsActionNote":false,"DifficultyLevel":2}
    ]
}
```

* Title (string)
* Description (string)
* BPM (string)
* GUID (string) - エディターによって自動的に生成（楽曲の識別などに使用します）
* NotesData (object[])
    * Time (double) - 秒単位
    * LaneID (int) - 使用するレーン
    * IsActionNote (bool) - 現在未実装のためfalseが既定
    * DifficultyLevel (int) - ノーツ難易度（easy: 0, normal: 1, hard: 2）

## Develop Environment
* Visual Studio 2019
* .NET Core 3.1 WPF

## Libraries
* Extended.Wpf.Toolkit
* Prism.Unity
* ReactiveProperty
* NAudio
* Newtonsoft.Json
* QuickConverter

## Latest Download
* [Latest Download (for 64bit Windows) - v0.6Alpha](https://github.com/Zakki0925224/SNE/releases/download/v0.6Alpha/SNE.zip)
* [リリース一覧](https://github.com/Zakki0925224/SNE/releases)

## Update Histories
### v0.6Alpha
* テキストボックスの内容が譜面データ・プロジェクトデータに反映されてないバグを修正
* ボタンアイコンの追加
### v0.5Alpha
* レーンIDの数値が適正でなくなるバグを修正
* 再生終了5秒前にBackボタンとForwardボタンが効かなくなるバグを修正
* メニューアイテムに対するショートカットキー機能の追加
### v0.4Alpha
* LPBの追加
* Offset調整の追加
* ノーツ表示サイズ調整の追加
* コントロールの変更
* ノーツ設置判定処理の変更
* レーンが見切れる不具合を修正
### v0.3Alpha
* 難易度切り替えシステムの追加
* 譜面判別用GUIDの追加
* ノーツ設置判定をもっと広く
* Descriptionテキストボックスの追加
* その他バグ修正
### v0.2Alpha
* 再生位置を変更するテキストボックスを追加
* レーン番号とBPM打数表示の追加
* 編集中プロジェクトの途中保存・復元機能の追加
* ノーツ設置・削除位置のマウス判定をより広く修正
* ノーツサイズ・カラーを変更
* その他バグ修正
## License
本ソフトウェアおよび使用しているライブラリすべてにMIT Licenseが適用されています。
