using SNE.Models.Editor;
using SNE.ViewModels;
using System.Windows;

namespace SNE.Views
{
    /// <summary>
    /// Interaction logic for PreviewWindow.xaml
    /// </summary>
    public partial class PreviewWindow : Window
    {
        public PreviewWindow(AudioPlayer audioPlayer, string jsonString)
        {
            InitializeComponent();
            ((PreviewWindowViewModel)this.DataContext).AudioPlayer.Value = audioPlayer;
            ((PreviewWindowViewModel)this.DataContext).JsonString.Value = jsonString;
        }
    }
}
