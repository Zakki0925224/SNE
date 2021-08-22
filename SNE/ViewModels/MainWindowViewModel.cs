using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SNE.Models;
using SNE.Models.Converters;
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
        public ReactiveProperty<bool> IsEditable { get; set; } = new ReactiveProperty<bool>(true);
        public ReactiveProperty<bool> IsInitialized { get; set; } = new ReactiveProperty<bool>(true);
        public ReactiveProperty<string> TitleString { get; set; } = new ReactiveProperty<string>("");
        public ReactiveCommand MenuItemFileNew_Clicked { get; } = new ReactiveCommand();
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
                var fileName = Models.Shell.FileDialog.ShowOpenFileDialog("MP3 Audio File (*.mp3)|*.mp3", "Open MP3...", true);

                if (File.Exists(fileName))
                {
                    this.AudioPlayer.Initialize(fileName);
                    this.Title.Value = $"{fileName} - SimpleNotesEditor";
                    this.IsEditable.Value = false;
                    this.TitleString.Value = Path.GetFileNameWithoutExtension(fileName);
                    this.Notes.Clear();
                    RaisePropertyChanged();
                    UpdateBPMUI();
                }
            });

            this.MenuItemFileExportJSONFile_Clicked.Subscribe(_ =>
            {
                var jsonString = ConvertToJsonData.Convert(this.TitleString.Value, "", new List<Note>(this.Notes), this.GridHeight.Value, this.BPM.Value);
                var fileName = Models.Shell.FileDialog.ShowSaveFileDialog("JSON File (*.json)|*.json", "Save JSON File...", true);

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
                    int index = -1;

                    for (int i = 0; i < this.Notes.Count; i++)
                    {
                        if (this.Notes[i].XPosition == noteXPos &&
                            this.Notes[i].YPosition == noteYPos)
                            index = i;
                    }

                    if (index == -1)
                    {
                        var note = new Note
                        {
                            XPosition = noteXPos,
                            YPosition = noteYPos
                        };

                        this.Notes.Add(note);
                    }
                }
            });

            this.Editor_MouseRightButtonDown.Subscribe(x =>
            {
                var xPos = ((MouseEventArgs)x.e).GetPosition((System.Windows.IInputElement)x.sender).X;
                var yPos = ((MouseEventArgs)x.e).GetPosition((System.Windows.IInputElement)x.sender).Y;
                var noteXPos = Math.Floor(xPos);
                var noteYPos = Math.Floor(yPos);

                //Debug.Print("Right clicked!");

                if (meInstance.Cursor == Cursors.Hand && this.IsInitialized.Value)
                {
                    int index = -1;
                    
                    for (int i = 0; i < this.Notes.Count; i++)
                    {
                        if (this.Notes[i].XPosition == noteXPos &&
                            this.Notes[i].YPosition == noteYPos)
                            index = i;
                    }

                    if (index != -1)
                        this.Notes.RemoveAt(index);
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
                Models.Shell.MessageBox.ShowInfoMessageBox($"{title}\n{version}\n{desc}\n{copyright}");
            });

            this.Lane.Subscribe(_ =>
            {
                if (this.Lane.Value > 0)
                    UpdateLaneUI();
            });
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
            var bpmToSec = 60 / this.BPM.Value;
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
    }
}
