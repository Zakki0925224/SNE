using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SNE.Models.Converters;
using SNE.Models.Editor;
using SNE.Models.Editor.DataModels;
using SNE.Models.Editor.ObservableModels;
using SNE.Models.Editor.ObservableObjects;
using SNE.Models.Shell;
using SNE.Models.Utils;
using SNE.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;

namespace SNE.ViewModels
{
    public class MainWindowViewModel : BindableBase, INotifyPropertyChanged
    {
        // models
        private MainWindow meInstance = App.Current.MainWindow as MainWindow;
        private Timer Timer { get; set; }
        public AudioPlayer AudioPlayer { get; set; } = new AudioPlayer();

        // reactive properties
        public ReactiveProperty<string> Title { get; set; } = new ReactiveProperty<string>("SimpleNotesEditor");
        public ReactiveProperty<double> CurrentTimeSeconds { get; set; }
        public ReactiveProperty<double> TotalTimeSeconds { get; set; }
        public ReactiveProperty<double> Volume { get; set; }
        public ReactiveProperty<double> GridHeight { get; } = new ReactiveProperty<double>(10);
        public ReactiveProperty<double> GridWidth { get; } = new ReactiveProperty<double>(10);
        public ReactiveProperty<Point> NoteGridLineSegment1 { get; set; } = new ReactiveProperty<Point>(new Point(0, 10));
        public ReactiveProperty<Point> NoteGridLineSegment2 { get; set; } = new ReactiveProperty<Point>(new Point(10, 10));
        public ReactiveProperty<Rect> NoteViewPort { get; set; } = new ReactiveProperty<Rect>(new Rect(0, 0, 10, 10));
        public ReactiveProperty<int> NoteSize { get; set; } = new ReactiveProperty<int>(2);
        public ReactiveProperty<int> BPM { get; set; } = new ReactiveProperty<int>(120);
        public ReactiveProperty<int> LPB { get; set; } = new ReactiveProperty<int>(1);
        public ReactiveProperty<int> Lane { get; set; } = new ReactiveProperty<int>(6);
        public ReactiveProperty<int> Offset { get; set; } = new ReactiveProperty<int>(0);
        public ReactiveProperty<bool> ShowEasyNotes { get; set; } = new ReactiveProperty<bool>(true);
        public ReactiveProperty<bool> ShowNormalNotes { get; set; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<bool> ShowHardNotes { get; set; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<bool> IsEditable { get; set; } = new ReactiveProperty<bool>(true);
        public ReactiveProperty<bool> IsInitialized { get; set; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<string> TitleString { get; set; } = new ReactiveProperty<string>("");
        public ReactiveProperty<string> DescString { get; set; } = new ReactiveProperty<string>("");
        public ReactiveCommand MenuItemFileNew_Clicked { get; } = new ReactiveCommand();
        public ReactiveCommand MenuItemFileOpen_Clicked { get; } = new ReactiveCommand();
        public ReactiveCommand MenuItemFileSave_Clicked { get; } = new ReactiveCommand();
        public ReactiveCommand MenuItemFileExportJSONFile_Clicked { get; } = new ReactiveCommand();
        public ReactiveCommand MenuItemFileExit_Clicked { get; } = new ReactiveCommand();
        public ReactiveCommand MenuItemHelpAbout_Clicked { get; } = new ReactiveCommand();
        public ReactiveCommand AudioPlayerPlayPauseButton_Clicked { get; } = new ReactiveCommand();
        public ReactiveCommand AudioPlayerBackButton_Clicked { get; } = new ReactiveCommand();
        public ReactiveCommand AudioPlayerForwardButton_Clicked { get; } = new ReactiveCommand();
        public ReactiveCommand<(object sender, EventArgs e)> Editor_MouseMoved { get; } = new ReactiveCommand<(object sender, EventArgs e)>();
        public ReactiveCommand<(object sender, EventArgs e)> Editor_MouseLeaved { get; } = new ReactiveCommand<(object sender, EventArgs e)>();
        public ReactiveCommand<(object sender, EventArgs e)> Editor_MouseLeftButtonDown { get; } = new ReactiveCommand<(object sender, EventArgs e)>();
        public ReactiveCommand<(object sender, EventArgs e)> Editor_MouseRightButtonDown { get; } = new ReactiveCommand<(object sender, EventArgs e)>();

        private ObservableCollection<Note> _notes = new ObservableCollection<Note>();
        public ObservableCollection<Note> Notes
        {
            get => _notes;
            set => SetProperty(ref _notes, value);
        }

        private ObservableCollection<Note> _filteredNotes = new ObservableCollection<Note>();
        public ObservableCollection<Note> FilteredNotes
        {
            get => _filteredNotes;
            set => SetProperty(ref _filteredNotes, value);
        }

        private ObservableCollection<LaneText> _lane = new ObservableCollection<LaneText>();
        public ObservableCollection<LaneText> LaneTexts
        {
            get => _lane;
            set => SetProperty(ref _lane, value);
        }

        private ObservableCollection<BPMText> _bpm = new ObservableCollection<BPMText>();
        public ObservableCollection<BPMText> BPMTexts
        {
            get => _bpm;
            set => SetProperty(ref _bpm, value);
        }

        private ObservableCollection<MousePointer> _mousePointers = new ObservableCollection<MousePointer>();
        public ObservableCollection<MousePointer> MousePointers
        {
            get => _mousePointers;
            set => SetProperty(ref _mousePointers, value);
        }

        public MainWindowViewModel()
        {
            this.CurrentTimeSeconds = this.AudioPlayer.ToReactivePropertyAsSynchronized(x => x.CurrentTimeSeconds);
            this.TotalTimeSeconds = this.AudioPlayer.ToReactivePropertyAsSynchronized(x => x.TotalTimeSeconds);
            this.Volume = this.AudioPlayer.ToReactivePropertyAsSynchronized(x => x.Volume);
            this.IsInitialized = this.AudioPlayer.ToReactivePropertyAsSynchronized(x => x.IsInitialized);
            SubscribeCommands();

            UpdateLaneUI();
        }

        private void SubscribeCommands()
        {
            this.MenuItemFileNew_Clicked.Subscribe(_ =>
            {
                var fileName = FileDialog.ShowOpenFileDialog("MP3 Audio File (*.mp3)|*.mp3", "Open MP3...", true);

                if (fileName == "")
                    return;
                
                InitializeEditor(fileName);
                UpdateNotesUI();
                UpdateBPMUI();
                UpdateLaneUI();
            });

            this.MenuItemFileOpen_Clicked.Subscribe(_ =>
            {
                var extention = Const.ProjectExtention;
                var fileName = FileDialog.ShowOpenFileDialog($"Simple Notes Editor Project File (*{extention})|*{extention}", "Open project...", true);

                if (!File.Exists(fileName))
                    return;

                using (var reader = new StreamReader(fileName))
                {
                    var jsonString = reader.ReadToEnd();
                    var dataModel = ConvertToExportDataModel.Convert(jsonString);

                    if (!dataModel.IsSet())
                        return;

                    InitializeEditor(dataModel.AudioFilePath);

                    if (!this.IsInitialized.Value)
                        return;

                    this.BPM.Value = dataModel.BPM.Value;
                    this.Lane.Value = dataModel.Lane.Value;
                    this.Offset.Value = dataModel.Offset.Value;
                    this.TitleString.Value = dataModel.Title;
                    this.DescString.Value = dataModel.Description;

                    foreach (var note in dataModel.Notes)
                        this.Notes.Add(note);

                    UpdateNotesUI();
                    UpdateBPMUI();
                    UpdateLaneUI();
                }
            });

            this.MenuItemFileSave_Clicked.Subscribe(_ =>
            {
                var extention = Const.ProjectExtention;
                var fileName = FileDialog.ShowSaveFileDialog($"Simple Notes Editor Project File (*{extention})|*{extention}", "Save project...", true);

                if (fileName == "")
                    return;

                var audioFilePath = this.AudioPlayer.AudioFilePath;
                var bpm = this.BPM.Value;
                var lpb = this.LPB.Value;
                var lane = this.Lane.Value;
                var offset = this.Offset.Value;
                var title = this.TitleString.Value;
                var desc = this.DescString.Value;
                var notes = new List<Note>(this.Notes);

                var model = new ExportDataModel(audioFilePath,
                                                bpm,
                                                lpb,
                                                lane,
                                                offset,
                                                title,
                                                desc,
                                                notes);

                var jsonString = ConvertToJsonData.ConvertToProjectDataJson(model);

                if (jsonString == "")
                {
                    Models.Shell.MessageBox.ShowErrorMessageBox("Failed to generate json data.");
                    return;
                }

                using var writer = new StreamWriter(fileName, false, Encoding.UTF8);
                writer.Write(jsonString);
            });

            this.MenuItemFileExportJSONFile_Clicked.Subscribe(_ =>
            {
                var jsonString = ConvertToJsonData.ConvertToExportJson(this.TitleString.Value, this.DescString.Value, new List<Note>(this.Notes), this.GridHeight.Value, this.BPM.Value, this.Offset.Value);
                var fileName = FileDialog.ShowSaveFileDialog("JSON File (*.json)|*.json", "Save JSON File...", true);

                if (fileName != "")
                    using (var sw = new StreamWriter(fileName, false, Encoding.UTF8)) { sw.Write(jsonString); }
            });

            this.MenuItemFileExit_Clicked.Subscribe(_ =>
            {
                meInstance.Close();
            });

            this.AudioPlayerPlayPauseButton_Clicked.Subscribe(_ =>
            {
                if (this.AudioPlayer.IsPlaying)
                {
                    this.AudioPlayer.Pause();
                    if (this.IsInitialized.Value)
                        this.Timer.Stop();
                }

                else
                {
                    this.AudioPlayer.Play();
                    if (this.IsInitialized.Value)
                        this.Timer.Start();
                }
            });

            this.AudioPlayerBackButton_Clicked.Subscribe(_ =>
            {
                this.AudioPlayer.Back(new TimeSpan(0, 0, 5));
            });

            this.AudioPlayerForwardButton_Clicked.Subscribe(_ =>
            {
                this.AudioPlayer.Forward(new TimeSpan(0, 0, 5));
            });

            this.Editor_MouseLeftButtonDown.Subscribe(x =>
            {
                if (!this.IsInitialized.Value)
                    return;

                var xPos = ((MouseEventArgs)x.e).GetPosition((System.Windows.IInputElement)x.sender).X;
                var yPos = ((MouseEventArgs)x.e).GetPosition((System.Windows.IInputElement)x.sender).Y;

                if (yPos < this.GridHeight.Value * (this.Lane.Value + 1) &&
                    Math.Round(yPos / this.GridHeight.Value) * this.GridHeight.Value > 0 &&
                    this.MousePointers.Count == 1)
                {
                    var pointer = this.MousePointers[0];
                    var level = GetCurrentDifficultyLevel();
                    var index = -1;

                    for (int i = 0; i < this.Notes.Count; i++)
                    {
                        if (this.Notes[i].XPosition == pointer.XPosition &&
                            this.Notes[i].YPosition == pointer.YPosition &&
                            this.Notes[i].DifficultyLevel == level)
                        index = i;
                    }

                        if (level == -1 || index != -1)
                        return;

                    var note = new Note
                    {
                        XPosition = pointer.XPosition,
                        YPosition = pointer.YPosition,
                        DifficultyLevel = level
                    };

                    this.Notes.Add(note);
                    UpdateNotesUI();
                }
            });

            this.Editor_MouseRightButtonDown.Subscribe(x =>
            {
                if (!this.IsInitialized.Value)
                    return;

                var xPos = ((MouseEventArgs)x.e).GetPosition((System.Windows.IInputElement)x.sender).X;
                var yPos = ((MouseEventArgs)x.e).GetPosition((System.Windows.IInputElement)x.sender).Y;

                if (yPos < this.GridHeight.Value * (this.Lane.Value + 1) &&
                    Math.Round(yPos / this.GridHeight.Value) * this.GridHeight.Value > 0 &&
                    this.MousePointers.Count == 1)
                {
                    var pointer = this.MousePointers[0];
                    var level = GetCurrentDifficultyLevel();
                    var index = -1;

                    for (int i = 0; i < this.Notes.Count; i++)
                    {
                        if (this.Notes[i].XPosition == pointer.XPosition &&
                            this.Notes[i].YPosition == pointer.YPosition &&
                            this.Notes[i].DifficultyLevel == level)
                            index = i;
                    }

                    if (index != -1)
                    {
                        this.Notes.RemoveAt(index);
                        UpdateNotesUI();
                    }
                }
            });

            this.Editor_MouseMoved.Subscribe(x =>
            {
                if (!this.IsInitialized.Value)
                    return;

                var xPos = ((MouseEventArgs)x.e).GetPosition((System.Windows.IInputElement)x.sender).X;
                var yPos = ((MouseEventArgs)x.e).GetPosition((System.Windows.IInputElement)x.sender).Y;
                //Debug.Print($"X:{xPos}, Y:{yPos}");

                if (((xPos % (this.GridWidth.Value / this.LPB.Value) < (this.GridWidth.Value / 2) &&
                    yPos % this.GridHeight.Value < (this.GridWidth.Value / 2)) ||
                    (xPos % (this.GridWidth.Value / this.LPB.Value) < (this.GridWidth.Value / 2) &&
                    yPos % this.GridHeight.Value > (this.GridWidth.Value / 2))) &&
                    yPos < this.GridHeight.Value * (this.Lane.Value + 1) - (this.GridWidth.Value / 2))
                {
                    var noteXPos = Math.Round(xPos / (this.GridWidth.Value / this.LPB.Value)) * (this.GridWidth.Value / this.LPB.Value);
                    var noteYPos = Math.Round(yPos / this.GridHeight.Value) * this.GridHeight.Value;

                    if (noteYPos > 0)
                        UpdateMousePointerUI(noteXPos, noteYPos);
                }
            });

            this.Editor_MouseLeaved.Subscribe(_ =>
            {
                this.MousePointers.Clear();
            });

            this.MenuItemHelpAbout_Clicked.Subscribe(_ =>
            {
                var title = AssemblyInfo.GetAssemblyTitle();
                var desc = AssemblyInfo.GetAssemblyDescription();
                var version = AssemblyInfo.GetAssembryVersion();
                var copyright = AssemblyInfo.GetAssemblyCopyright();
                Models.Shell.MessageBox.ShowInfoMessageBox($"{title}\n{version}\n{desc}\n{copyright}");
            });

            this.Lane.Subscribe(_ =>
            {
                if (this.Lane.Value > 0)
                    UpdateLaneUI();
            });

            this.ShowEasyNotes.Subscribe(_ =>
            {
                UpdateNotesUI();
            });

            this.ShowNormalNotes.Subscribe(_ =>
            {
                UpdateNotesUI();
            });

            this.ShowHardNotes.Subscribe(_ =>
            {
                UpdateNotesUI();
            });

            this.LPB.Subscribe(_ =>
            {
                UpdateGridUI();
            });

            this.GridWidth.Subscribe(_ =>
            {
                UpdateGridUI();
            });

            this.GridHeight.Subscribe(_ =>
            {
                UpdateGridUI();
            });

            this.NoteSize.Subscribe(_ =>
            {
                UpdateNotesUI();
            });
        }

        private void InitializeEditor(string audioFilePath)
        {
            if (File.Exists(audioFilePath))
            {
                this.AudioPlayer.Initialize(audioFilePath);
                this.Title.Value = $"{audioFilePath} - SimpleNotesEditor";
                this.IsEditable.Value = false;
                this.TitleString.Value = Path.GetFileNameWithoutExtension(audioFilePath);
                this.Notes.Clear();
                RaisePropertyChanged();
                TimerInitialize();
                this.IsInitialized.Value = true;
            }
            else
            {
                Models.Shell.MessageBox.ShowErrorMessageBox($"\"{audioFilePath}\" was not found.");
                this.IsInitialized.Value = false;
            }
        }

        private int GetCurrentDifficultyLevel()
        {
            var level = -1;

            if (this.ShowEasyNotes.Value) level = 0;
            else if (this.ShowNormalNotes.Value) level = 1;
            else if (this.ShowHardNotes.Value) level = 2;

            return level;
        }

        private void UpdateLaneUI()
        {
            this.LaneTexts.Clear();

            for (int i = 1; i <= this.Lane.Value; i++)
            {
                var lane = new LaneText
                {
                    Text = $"L: {i}",
                    XPosition = 0,
                    YPosition = GridHeight.Value * i - 7
                };

                this.LaneTexts.Add(lane);
            }
        }

        private void UpdateBPMUI()
        {
            this.BPMTexts.Clear();
            var cnt = 1;

            for (int i = 0; i <= this.TotalTimeSeconds.Value * 100; i++)
            {
                if (i % GridWidth.Value == 0)
                {
                    var bpm = new BPMText
                    {
                        Text = cnt.ToString(),
                        XPosition = i + 3,
                        YPosition = 0.5
                    };

                    this.BPMTexts.Add(bpm);
                    cnt++;
                }
            }
        }

        private void UpdateNotesUI()
        {
            this.FilteredNotes.Clear();

            foreach (var note in this.Notes)
            {
                note.Size = this.NoteSize.Value;

                if (this.ShowEasyNotes.Value && note.DifficultyLevel == 0)
                {
                    this.FilteredNotes.Add(note);
                    continue;
                }

                if (this.ShowNormalNotes.Value && note.DifficultyLevel == 1)
                {
                    this.FilteredNotes.Add(note);
                    continue;
                }

                if (this.ShowHardNotes.Value && note.DifficultyLevel == 2)
                {
                    this.FilteredNotes.Add(note);
                    continue;
                }
            }
        }

        private void UpdateGridUI()
        {
            var height = this.GridHeight.Value;
            var lpbToX = this.GridWidth.Value / this.LPB.Value;
            this.NoteGridLineSegment1.Value = new Point(0, height);
            this.NoteGridLineSegment2.Value = new Point(lpbToX, height);
            this.NoteViewPort.Value = new Rect(0, 0, lpbToX, height);
        }

        private void UpdateMousePointerUI(double xPos, double yPos)
        {
            this.MousePointers.Clear();
            var pointer = new MousePointer
            {
                XPosition = xPos,
                YPosition = yPos,
                Size = this.NoteSize.Value
            };
            this.MousePointers.Add(pointer);
        }

        private void TimerInitialize()
        {
            this.Timer = new Timer(50);
            this.Timer.AutoReset = true;
            this.Timer.Elapsed += Timer_Elapsed;

        }

        private async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            await Task.Run(() =>
            {
                if (this.AudioPlayer.IsPlaying)
                    this.CurrentTimeSeconds.Value = this.AudioPlayer.CurrentTimeSeconds;
            });
        }
    }
}
