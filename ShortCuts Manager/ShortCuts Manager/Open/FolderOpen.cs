using ShortCuts_Manager.Interfaces;
using System.Diagnostics;
using System.Windows;
using MessageBox = System.Windows.MessageBox;

namespace ShortCuts_Manager.Open;

public class FolderOpen : IFolderOpen
{
    public void OpenFolders(string[] folders)
    {
        foreach (string folder in folders)
        {
            try
            {
                Process.Start(new ProcessStartInfo()
                {
                    FileName = folder,
                    UseShellExecute = true,
                    Verb = "open"
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Path: {0}\n{1}", folder, ex.Message), "ShortCuts Manager", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}