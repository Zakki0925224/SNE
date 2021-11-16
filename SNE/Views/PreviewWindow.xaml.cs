using SNE.Models.Editor;
using SNE.Models.Editor.ObservableModels;
using SNE.ViewModels;
using System.Collections.ObjectModel;
using System.Windows;

namespace SNE.Views
{
    /// <summary>
    /// Interaction logic for PreviewWindow.xaml
    /// </summary>
    public partial class PreviewWindow : Window
    {
        public PreviewWindow(AudioPlayer audioPlayer, ObservableCollection<Note> filteredNotes, int bpm, int offset)
        {
            InitializeComponent();
            var vm = (PreviewWindowViewModel)this.DataContext;
            vm.SharedEditingNotes = filteredNotes;
            vm.AudioPlayer.Value = audioPlayer;
            vm.BPM.Value = bpm;
            vm.Offset.Value = offset;
            vm.InitializePreviewUI();
        }
    }
}
