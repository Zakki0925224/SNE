using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SNE.Models.Converters;
using SNE.Models.Editor;
using SNE.Models.Editor.DataModels;
using SNE.Models.Editor.ObservableModels;
using SNE.Models.Shell;
using SNE.Models.Utils;
using SNE.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Input;

namespace SNE.ViewModels
{
    public class MainWindowViewModel : BindableBase, INotifyPropertyChanged
    {
        // models
        private MainWindow meInstance = App.Current.MainWindow as MainWindow;
        public AudioPlayer AudioPlayer { get; set; } = new AudioPlayer();

        // reactive properties
        public ReactiveProperty<string> Title { get; set; } = new ReactiveProperty<string>("SimpleNotesEditor");
        public ReactiveProperty<double> CurrentTimeSeconds { get; set; }
        public ReactiveProperty<double> TotalTimeSeconds { get; set; }
        public ReactiveProperty<double> Volume { get; set; }
        public ReactiveProperty<double> GridHeight { get; set; } = new ReactiveProperty<double>(5);
        public ReactiveProperty<double> BPM { get; set; } = new ReactiveProperty<double>(120);
        public ReactiveProperty<int> Lane { get; set; } = new ReactiveProperty<int>(6);
        public ReactiveProperty<bool> ShowEasyNotes { get; set; } = new ReactiveProperty<bool>(true);
        public ReactiveProperty<bool> ShowNormalNotes { get; set; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<bool> ShowHardNotes { get; set; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<bool> IsEditable { get; set; } = new ReactiveProperty<bool>(true);
        public ReactiveProperty<bool> IsInitialized { get; set; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<string> TitleString { get; set; } = new ReactiveProperty<string>("");
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
                var extention = ProjectInfo.ProjectExtention;
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
                    this.TitleString.Value = dataModel.Title;
                    // description = datamodel.Description;

                    foreach (var note in dataModel.Notes)
                        this.Notes.Add(note);

                    UpdateNotesUI();
                    UpdateBPMUI();
                    UpdateLaneUI();
                }
            });

            this.MenuItemFileSave_Clicked.Subscribe(_ =>
            {
                var extention = ProjectInfo.ProjectExtention;
                var fileName = FileDialog.ShowSaveFileDialog($"Simple Notes Editor Project File (*{extention})|*{extention}", "Save project...", true);

                if (fileName == "")
                    return;

                var audioFilePath = this.AudioPlayer.AudioFilePath;
                var bpm = this.BPM.Value;
                var lane = this.Lane.Value;
                var title = this.TitleString.Value;
                var desc = "";
                var notes = new List<Note>(this.Notes);

                var model = new ExportDataModel(audioFilePath,
                                                bpm,
                                                lane,
                                                title,
                                                desc,
                                                notes);

                var jsonString = ConvertToJsonData.ConvertToProjectDataJson(model);

                if (jsonString == "")
                {
                    MessageBox.ShowErrorMessageBox("Failed to generate json data.");
                    return;
                }

                using (var writer = new StreamWriter(fileName, false, Encoding.UTF8)) { writer.Write(jsonString); }
            });

            this.MenuItemFileExportJSONFile_Clicked.Subscribe(_ =>
            {
                var jsonString = ConvertToJsonData.ConvertToExportJson(this.TitleString.Value, "", new List<Note>(this.Notes), this.GridHeight.Value, this.BPM.Value);
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
                    this.AudioPlayer.Pause();

                else
                    this.AudioPlayer.Play();
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
                var xPos = ((MouseEventArgs)x.e).GetPosition((System.Windows.IInputElement)x.sender).X;
                var yPos = ((MouseEventArgs)x.e).GetPosition((System.Windows.IInputElement)x.sender).Y;
                var noteXPos = Math.Floor(xPos);
                var noteYPos = Math.Floor(yPos);

                //Debug.Print("Clicked!");

                if (meInstance.Cursor == Cursors.Hand && this.IsInitialized.Value)
                {
                    var index = -1;
                    var level = -1;

                    if (this.ShowEasyNotes.Value) level = 0;
                    else if (this.ShowNormalNotes.Value) level = 1;
                    else if (this.ShowHardNotes.Value) level = 2;

                    for (int i = 0; i < this.Notes.Count; i++)
                    {
                        if (this.Notes[i].XPosition == noteXPos &&
                            this.Notes[i].YPosition == noteYPos &&
                            this.Notes[i].DifficultyLevel == level)
                            index = i;
                    }

                    if (index == -1)
                    {
                        var note = new Note
                        {
                            XPosition = noteXPos,
                            YPosition = noteYPos,
                            DifficultyLevel = level
                        };

                        this.Notes.Add(note);
                        UpdateNotesUI();
                    }
                }
                else if (meInstance.Cursor != Cursors.Hand && this.IsInitialized.Value)
                {
                    //var sec = ConvertXcoordinateToSecond.Convert(xPos);
                    //this.CurrentTimeSeconds.Value = sec;
                }
            });

            this.Editor_MouseRightButtonDown.Subscribe(x =>
            {
                var xPos = ((MouseEventArgs)x.e).GetPosition((System.Windows.IInputElement)x.sender).X;
                var yPos = ((MouseEventArgs)x.e).GetPosition((System.Windows.IInputElement)x.sender).Y;
                var noteXPos = Math.Floor(xPos);
                var noteYPos = Math.Floor(yPos);

                var level = -1;

                if (this.ShowEasyNotes.Value) level = 0;
                else if (this.ShowNormalNotes.Value) level = 1;
                else if (this.ShowHardNotes.Value) level = 2;

                //Debug.Print("Right clicked!");

                if (meInstance.Cursor == Cursors.Hand && this.IsInitialized.Value)
                {
                    int index = -1;

                    for (int i = 0; i < this.Notes.Count; i++)
                    {
                        if (this.Notes[i].XPosition == noteXPos &&
                            this.Notes[i].YPosition == noteYPos &&
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
                var xPos = ((MouseEventArgs)x.e).GetPosition((System.Windows.IInputElement)x.sender).X;
                var yPos = ((MouseEventArgs)x.e).GetPosition((System.Windows.IInputElement)x.sender).Y;
                //Debug.Print($"X:{xPos}, Y:{yPos}");

                if (yPos > this.GridHeight.Value * 2 &&
                    yPos < this.GridHeight.Value * (this.Lane.Value + 1) * 2 &&
                    xPos % 10 < 0.9 &&
                    yPos % (this.GridHeight.Value * 2) < 0.9 &&
                    this.IsInitialized.Value)
                {
                    meInstance.Cursor = Cursors.Hand;
                }
                else
                {
                    meInstance.Cursor = Cursors.Arrow;
                }
            });

            this.Editor_MouseLeaved.Subscribe(_ => 
            {
                meInstance.Cursor = Cursors.Arrow;
            });

            this.MenuItemHelpAbout_Clicked.Subscribe(_ =>
            {
                var title = AssemblyInfo.GetAssemblyTitle();
                var desc = AssemblyInfo.GetAssemblyDescription();
                var version = AssemblyInfo.GetAssembryVersion();
                var copyright = AssemblyInfo.GetAssemblyCopyright();
                MessageBox.ShowInfoMessageBox($"{title}\n{version}\n{desc}\n{copyright}");
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
                this.IsInitialized.Value = true;
            }
            else
            {
                MessageBox.ShowErrorMessageBox($"\"{audioFilePath}\" was not found.");
                this.IsInitialized.Value = false;
            }
        }

        private void UpdateLaneUI()
        {
            this.LaneTexts.Clear();

            for (int i = 1; i <= this.Lane.Value; i++)
            {
                var lane = new LaneText();
                lane.Text = $"L: {i}";
                lane.XPosition = 0;
                lane.YPosition = GridHeight.Value * (2 * i - 1);

                this.LaneTexts.Add(lane);
            }
        }

        private void UpdateBPMUI()
        {
            this.BPMTexts.Clear();
            var cnt = 1;

            for (int i = 0; i <= this.TotalTimeSeconds.Value * 100; i++)
            {
                if (i % 10 == 0)
                {
                    var bpm = new BPMText();
                    bpm.Text = cnt.ToString();
                    bpm.XPosition = i + 4;
                    bpm.YPosition = 1;

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
    }
}
