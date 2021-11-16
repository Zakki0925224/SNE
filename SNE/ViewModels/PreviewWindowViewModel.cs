using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SNE.Models.Editor;
using SNE.Models.Editor.ObservableModels;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;

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
        public ReactiveProperty<double> CheckLineYCoordinate { get; set; } = new ReactiveProperty<double>(400);
        public ReactiveProperty<double> CheckLineWidth { get; set; } = new ReactiveProperty<double>(600);
        public ReactiveProperty<int> BPM { get; set; } = new ReactiveProperty<int>();
        public ReactiveProperty<int> Offset { get; set; } = new ReactiveProperty<int>();

        private ObservableCollection<Note> _sharedEditingNotes = null;
        public ObservableCollection<Note> SharedEditingNotes
        {
            get => _sharedEditingNotes;
            set => SetProperty(ref _sharedEditingNotes, value);
        }

        private ObservableCollection<Note> _viewNotes = new ObservableCollection<Note>();
        public ObservableCollection<Note> ViewNotes
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
        }

        public void InitializePreviewUI()
        {
            if (this.AudioPlayer.Value == null ||
                this.SharedEditingNotes == null)
                return;

            if (this.IsInitialized.Value)
                return;

            // TODO: generate preview ui

            // for debug
            Debug.Print($"AP:{this.AudioPlayer.Value}, BPM:{this.BPM.Value}, Offset:{this.Offset.Value}");

            InitializeTimer();
            this.IsInitialized.Value = true;
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
}
