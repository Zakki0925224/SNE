using System.Windows;

namespace SNE.Models.Shell
{
    public static class MessageBox
    {
        public static void ShowErrorMessageBox(string message)
        {
            System.Windows.MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void ShowInfoMessageBox(string message)
        {
            System.Windows.MessageBox.Show(message, "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
