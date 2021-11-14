using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SNE.Models.Editor;
using SNE.Models.Editor.DataModels;
using SNE.Models.Editor.ObservableModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SNE.ViewModels
{
    public class PreviewWindowViewModel : BindableBase
    {
        public ReactiveProperty<string> Title { get; set; } = new ReactiveProperty<string>($"Preview - {Const.AppName}");
        public ReactiveProperty<AudioPlayer> AudioPlayer { get; set; } = new ReactiveProperty<AudioPlayer>();
        public ReactiveProperty<JsonDataModel> DataModel { get; set; } = new ReactiveProperty<JsonDataModel>();

        public ReactiveProperty<double> CurrentTimeSeconds { get; set; }
        public ReactiveProperty<double> TotalTimeSeconds { get; set; }
        public ReactiveProperty<double> Volume { get; set; }
        public ReactiveProperty<bool> ShowEasyNotes { get; set; } = new ReactiveProperty<bool>(true);
        public ReactiveProperty<bool> ShowNormalNotes { get; set; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<bool> ShowHardNotes { get; set; } = new ReactiveProperty<bool>(false);

        private ObservableCollection<Note> _notes = new ObservableCollection<Note>();
        public ObservableCollection<Note> Notes
        {
            get => _notes;
            set => SetProperty(ref _notes, value);
        }

        public List<Note> TargetLaneNotes = new List<Note>();

        public PreviewWindowViewModel()
        {
            this.CurrentTimeSeconds = this.AudioPlayer.ToReactivePropertyAsSynchronized(x => x.Value.CurrentTimeSeconds);
            this.TotalTimeSeconds = this.AudioPlayer.ToReactivePropertyAsSynchronized(x => x.Value.TotalTimeSeconds);
            this.Volume = this.AudioPlayer.ToReactivePropertyAsSynchronized(x => x.Value.Volume);
            SubscribeCommands();
        }

        private void SubscribeCommands()
        {
            this.AudioPlayer.Subscribe(_ =>
            {
                UpdatePreviewUI();
            });

            this.DataModel.Subscribe(_ =>
            {
                UpdatePreviewUI();
            });

            this.ShowEasyNotes.Subscribe(_ =>
            {
                UpdatePreviewUI();
            });

            this.ShowNormalNotes.Subscribe(_ =>
            {
                UpdatePreviewUI();
            });

            this.ShowHardNotes.Subscribe(_ =>
            {
                UpdatePreviewUI();
            });
        }

        private void UpdatePreviewUI()
        {
            if (this.AudioPlayer.Value == null ||
                this.DataModel.Value == null)
                return;

            this.Notes.Clear();
            this.TargetLaneNotes.Clear();

            // get max lane id
            var maxLaneID = 1;
            foreach (var note in this.DataModel.Value.NotesData)
            {
                var laneID = note.LaneID;

                if (maxLaneID < laneID)
                    maxLaneID = laneID;
            }

            // gen target lane notes
            for (int i = 1; i <= maxLaneID; i++)
                this.TargetLaneNotes.Add(new Note() { XPosition = 10 * i, YPosition = 0 });

            // gen notes
            foreach (var note in this.DataModel.Value.NotesData)
            {
                this.Notes.Add(new Note()
                {
                    XPosition = this.TargetLaneNotes[note.LaneID - 1].XPosition,
                    YPosition = 0,
                    DifficultyLevel = note.DifficultyLevel
                });
            }
        }
    }
}
