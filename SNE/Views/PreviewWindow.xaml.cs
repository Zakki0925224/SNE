using SNE.Models.Editor;
using SNE.Models.Editor.DataModels;
using SNE.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace SNE.Views
{
    /// <summary>
    /// Interaction logic for PreviewWindow.xaml
    /// </summary>
    public partial class PreviewWindow : Window
    {
        public PreviewWindow(AudioPlayer audioPlayer, List<NoteDataModel> filteredNotes, int bpm, int offset, double lanePositionDistance)
        {
            InitializeComponent();
            var vm = (PreviewWindowViewModel)this.DataContext;
            vm.SharedEditingNotes = new ObservableCollection<NoteDataModel>(filteredNotes);
            vm.AudioPlayer.Value = audioPlayer;
            vm.BPM.Value = bpm;
            vm.Offset.Value = offset;
            vm.LanePositionDistance.Value = lanePositionDistance;
            vm.InitializePreviewUI();
        }
    }
}
