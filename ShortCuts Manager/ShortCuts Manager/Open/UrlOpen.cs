using ShortCuts_Manager.Interfaces;
using System.Runtime.InteropServices;

namespace ShortCuts_Manager.Open
{
    public class UrlOpen : IUrlOpen
    {
        public void OpenUrlsInDefaultBrowser(string[] urls)
        {
            foreach (string url in urls)
            {
                OpenUrlInDefaultBrowser(url: url);
            }
        }

        [DllImport("shell32.dll")]
        private static extern IntPtr ShellExecute(IntPtr hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, int nShowCmd);

        private static void OpenUrlInDefaultBrowser(string url)
        {
            ShellExecute(
                hwnd: IntPtr.Zero,
                lpOperation: "open",
                lpFile: url,
                lpParameters: null,
                lpDirectory: null,
                nShowCmd: 1);
        }
    }
}