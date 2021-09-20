# SimpleNotesEditor
![lisence](https://img.shields.io/github/license/Zakki0925224/SNE)
![latest-version](https://img.shields.io/github/v/release/Zakki0925224/SNE?include_prereleases)
![lisence](https://img.shields.io/github/downloads/Zakki0925224/SNE/total)

音ゲーの譜面を作るシンプルなエディター。mp3形式の音楽ファイルからjson形式の譜面を出力します。

**ロングノーツ・スライドノーツ等**には対応していません。

## ScreenShots
![](https://user-images.githubusercontent.com/49384910/133747964-ffec1d67-ce29-4fb6-b84a-82bde2c663f3.png)
![](https://user-images.githubusercontent.com/49384910/133747966-c8acccc1-4b69-49aa-81a6-88e4b08bbafe.png)
* 水平方向の拡大率を利用しているため、BPM値を変更すると横幅が縮小・拡大されます

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
* GUID (string) - エディターによって自動的に生成
* NotesData (object[])
    * Time (double) - 秒単位
    * LaneID (int) - 使用するレーン
    * IsActionNote (bool) - 現在未実装のためfalseが既定
    * DifficultyLevel (int) - ノーツ難易度（easy: 0, normal: 1, hard: 2）

## Develop Environment
* Visual Studio 2019
* .NET Core 3.1 WPF

## Libraries
* Prism.Unity
* ReactiveProperty
* NAudio
* Newtonsoft.Json
* QuickConverter

## Latest Download
* [Latest Download (for 64bit windows) - v0.3Alpha](https://github.com/Zakki0925224/SNE/releases/download/v0.3Alpha/SNE.zip)
* [リリース一覧](https://github.com/Zakki0925224/SNE/releases)

## Update Histories
### v0.3Alpha
* 難易度切り替えシステムの追加
* 譜面判別用GUIDの追加
* ノーツ設置判定をもっと広く
* Descriptionテキストボックスの追加
* その他バグ修正

## License
本ソフトウェアおよび使用しているライブラリすべてにMIT Licenseが適用されています。