using System.IO;
using ShortCuts_Manager.Dialogs;
using System.Windows;
using System.Windows.Data;
using DragEventHandler = System.Windows.Forms.DragEventHandler;

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

            this.AllowDrop = true;

            this.DragEnter += new System.Windows.DragEventHandler(Window_DragEnter);
            this.Drop += new System.Windows.DragEventHandler(Window_Drop);
        }

        private void TopBar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Maximalize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : this.WindowState = WindowState.Maximized;
        }

        private void Minimalize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

	private void Filter_SelectionChanged(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(SingleShortCutInformationList.ItemsSource).Refresh();
            CollectionViewSource.GetDefaultView(GroupShortCutInformationList.ItemsSource).Refresh();
        }

        private void ClearFilter_Click(object sender, RoutedEventArgs e)
        {
            Filter.Text = null;
        }
	private void Window_DragEnter(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                e.Effects = System.Windows.DragDropEffects.Copy;
            }
            else
            {
                e.Effects = System.Windows.DragDropEffects.None;
            }
        }

        private void Window_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
                if (files.Length > 0)
                {
                    string path = files[0];
                    string name = Path.GetFileName(path);

                    (System.Windows.Application.Current.MainWindow.DataContext as MainWindowViewModel).OpenAddWindow(name, path);
                }
            }
        }

        private void Window_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
                if (files.Length > 0)
                {
                    string path = files[0];
                    string name = Path.GetFileName(path);

                    (System.Windows.Application.Current.MainWindow.DataContext as MainWindowViewModel).OpenAddWindow(name, path);
                }
            }
        }
    }
}