using ShortCuts_Manager.Interfaces;
using System.Diagnostics;
using System.Windows;

namespace ShortCuts_Manager.Open
{
    public class FileOpen : IFileOpen
    {
        public void OpenFiles(string[] paths)
        {
            foreach (string path in paths)
            {
                try
                {
                    Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("Path: {0}\n{1}", path, ex.Message), "ShortCuts Manager", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}