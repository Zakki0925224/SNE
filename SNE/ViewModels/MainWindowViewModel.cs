using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SNE.Models;
using SNE.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
        public ReactiveCommand MenuItemFileNew_Clicked { get; } = new ReactiveCommand();
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

        public MainWindowViewModel()
        {
            this.CurrentTimeSeconds = this.AudioPlayer.ToReactivePropertyAsSynchronized(x => x.CurrentTimeSeconds);
            this.TotalTimeSeconds = this.AudioPlayer.ToReactivePropertyAsSynchronized(x => x.TotalTimeSeconds);
            this.Volume = this.AudioPlayer.ToReactivePropertyAsSynchronized(x => x.Volume);
            SubscribeCommands();
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
                    this.Notes.Clear();
                    RaisePropertyChanged();
                }
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
                var noteXPos = Math.Round(xPos, MidpointRounding.AwayFromZero);
                var noteYPos = Math.Round(yPos, MidpointRounding.AwayFromZero);

                Debug.Print("Clicked!");

                if (meInstance.Cursor == Cursors.Hand)
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
                var noteXPos = Math.Round(xPos, MidpointRounding.AwayFromZero);
                var noteYPos = Math.Round(yPos, MidpointRounding.AwayFromZero);

                Debug.Print("Right clicked!");

                if (meInstance.Cursor == Cursors.Hand)
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
                Debug.Print($"X:{xPos}, Y:{yPos}");

                if (yPos > this.GridHeight.Value * 2 &&
                    yPos < this.GridHeight.Value * (this.Lane.Value + 1) * 2 &&
                    xPos % 10 < 0.5 &&
                    yPos % (this.GridHeight.Value * 2) < 0.5)
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
        }
    }
}
