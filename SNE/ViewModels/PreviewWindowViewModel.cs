using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SNE.Models.Editor;
using SNE.Models.Editor.ObservableModels;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;
using SNE.Models.Editor.DataModels;

namespace SNE.ViewModels
{
    public class PreviewWindowViewModel : BindableBase
    {
        private Timer Timer { get; set; }

        public ReactiveProperty<string> Title { get; set; } = new ReactiveProperty<string>($"Preview - {Const.AppName}");
        public ReactiveProperty<double> WindowWidth { get; set; } = new ReactiveProperty<double>(640);
        public ReactiveProperty<AudioPlayer> AudioPlayer { get; set; } = new ReactiveProperty<AudioPlayer>();

        public ReactiveProperty<double> CurrentTimeSeconds { get; set; }
        public ReactiveProperty<double> TotalTimeSeconds { get; set; }
        public ReactiveProperty<double> Volume { get; set; }
        public ReactiveProperty<bool> IsInitialized { get; set; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<double> CheckLineYPosition { get; set; } = new ReactiveProperty<double>(350);
        public ReactiveProperty<double> CheckLineWidth { get; set; } = new ReactiveProperty<double>(640);
        public ReactiveProperty<double> LanePositionDistance { get; set; } = new ReactiveProperty<double>(10);
        public ReactiveProperty<int> NotesSlideSpeed { get; set; } = new ReactiveProperty<int>(10);
        public ReactiveProperty<int> NotesSize { get; set; } = new ReactiveProperty<int>(50);
        public ReactiveProperty<int> BPM { get; set; } = new ReactiveProperty<int>();
        public ReactiveProperty<int> Offset { get; set; } = new ReactiveProperty<int>();
        public ReactiveProperty<bool> ShowEasyNotes { get; set; } = new ReactiveProperty<bool>(true);
        public ReactiveProperty<bool> ShowNormalNotes { get; set; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<bool> ShowHardNotes { get; set; } = new ReactiveProperty<bool>(false);

        private ObservableCollection<NoteDataModel> _sharedEditingNotes = null;
        public ObservableCollection<NoteDataModel> SharedEditingNotes
        {
            get => _sharedEditingNotes;
            set => SetProperty(ref _sharedEditingNotes, value);
        }

        private ObservableCollection<ViewNote> _viewNotes = new ObservableCollection<ViewNote>();
        public ObservableCollection<ViewNote> ViewNotes
        {
            get => _viewNotes;
            set => SetProperty(ref _viewNotes, value);
        }

        public ReactiveCommand AudioPlayerPlayPauseButton_Clicked { get; } = new ReactiveCommand();

        public PreviewWindowViewModel()
        {
            this.CurrentTimeSeconds = this.AudioPlayer.ToReactivePropertyAsSynchronized(x => x.Value.CurrentTimeSeconds);
            this.TotalTimeSeconds = this.AudioPlayer.ToReactivePropertyAsSynchronized(x => x.Value.TotalTimeSeconds);
            this.Volume = this.AudioPlayer.ToReactivePropertyAsSynchronized(x => x.Value.Volume);
            this.ViewNotes = new ObservableCollection<ViewNote>();
            SubscribeCommands();
        }

        private void SubscribeCommands()
        {
            this.AudioPlayerPlayPauseButton_Clicked.Subscribe(_ =>
            {
                if (this.AudioPlayer.Value.IsPlaying)
                {
                    if (this.IsInitialized.Value)
                    {
                        this.AudioPlayer.Value.Pause();
                        this.Timer.Stop();
                    }
                }
                else
                {
                    if (this.IsInitialized.Value)
                    {
                        this.AudioPlayer.Value.Play();
                        this.Timer.Start();
                    }
                }
            });

            this.CurrentTimeSeconds.Subscribe(_ => UpdateNotesPosition());
            this.ShowEasyNotes.Subscribe(_ => UpdateNotesUI());
            this.ShowNormalNotes.Subscribe(_ => UpdateNotesUI());
            this.ShowHardNotes.Subscribe(_ => UpdateNotesUI());
            this.NotesSlideSpeed.Subscribe(_ => UpdateNotesUI());
        }

        public void InitializePreviewUI()
        {
            if (this.AudioPlayer.Value == null ||
                this.SharedEditingNotes == null)
                return;

            if (this.IsInitialized.Value)
                return;

            UpdateNotesUI();

            // for debug
            Debug.Print($"AP:{this.AudioPlayer.Value}, BPM:{this.BPM.Value}, Offset:{this.Offset.Value}");

            InitializeTimer();
            this.IsInitialized.Value = true;
        }

        private void UpdateNotesUI()
        {
            if (this.SharedEditingNotes == null)
                return;

            this.ViewNotes.Clear();

            if (this.ShowEasyNotes.Value)
                this.SharedEditingNotes.Where(x => x.DifficultyLevel == 0).ToList().ForEach(x => this.ViewNotes.Add(new ViewNote(new Note() {Size = this.NotesSize.Value, DifficultyLevel = 0}, x.LaneID, x.Time)));

            else if (this.ShowNormalNotes.Value)
                this.SharedEditingNotes.Where(x => x.DifficultyLevel == 1).ToList().ForEach(x => this.ViewNotes.Add(new ViewNote(new Note() { Size = this.NotesSize.Value, DifficultyLevel = 1 }, x.LaneID, x.Time)));

            else
                this.SharedEditingNotes.Where(x => x.DifficultyLevel == 2).ToList().ForEach(x => this.ViewNotes.Add(new ViewNote(new Note() { Size = this.NotesSize.Value, DifficultyLevel = 2 }, x.LaneID, x.Time)));

            UpdateNotesPosition();
        }

        private void UpdateNotesPosition()
        {
            foreach(var note in this.ViewNotes)
            {
                // TODO: Not reflected in view
                //note.Note.XPosition = note.LaneID * this.LanePositionDistance.Value;
                note.Note.XPosition = 100;
                note.Note.YPosition = this.CurrentTimeSeconds.Value / note.Time * this.CheckLineYPosition.Value;
            }
        }

        private void InitializeTimer()
        {
            this.Timer = new Timer(50);
            this.Timer.AutoReset = true;
            this.Timer.Elapsed += Timer_Elapsed;
        }

        private async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            await Task.Run(() =>
            {
                if (this.AudioPlayer.Value.IsPlaying)
                    this.CurrentTimeSeconds.Value = this.AudioPlayer.Value.CurrentTimeSeconds;
            });
        }
    }

    public class ViewNote
    {
        public Note Note { get; set; }
        public int LaneID { get; set; }
        public double Time { get; set; }

        public ViewNote(Note note, int laneID, double time)
        {
            this.Note = note;
            this.LaneID = laneID;
            this.Time = time;
        }
    }
}
