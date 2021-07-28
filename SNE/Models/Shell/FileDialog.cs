using System.Windows.Forms;

namespace SNE.Models.Shell
{
    public static class FileDialog
    {
        public static string ShowOpenFileDialog(string filter, string title, bool restoreDirectory, int filterIndex = 0)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = filter;
                ofd.FilterIndex = filterIndex;
                ofd.Title = title;
                ofd.RestoreDirectory = restoreDirectory;

                if (ofd.ShowDialog() == DialogResult.OK)
                    return ofd.FileName;
                else
                    return "";
            }
        }

        public static string ShowSaveFileDialog(string filter, string title, bool restoreDirectory, int filterIndex = 0)
        {
            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = filter;
                sfd.FilterIndex = filterIndex;
                sfd.Title = title;
                sfd.RestoreDirectory = restoreDirectory;

                if (sfd.ShowDialog() == DialogResult.OK)
                    return sfd.FileName;
                else
                    return "";
            }
        }
    }
}
