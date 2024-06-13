using System.Runtime.InteropServices;
using System.Windows;

namespace ShortCuts_Manager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel vm)
        {
            InitializeComponent();

            DataContext = vm;
        }
    }
}