using System.Windows;
using System.Windows.Data;

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
    }
}