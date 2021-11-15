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
        public PreviewWindow(AudioPlayer audioPlayer, ObservableCollection<Note> filteredNotes)
        {
            InitializeComponent();
            ((PreviewWindowViewModel)this.DataContext).SharedEditingNotes = filteredNotes;
            ((PreviewWindowViewModel)this.DataContext).AudioPlayer.Value = audioPlayer;
        }
    }
}
