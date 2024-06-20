using ShortCuts_Manager.Helpers.Enums;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Application = System.Windows.Application;
using Button = System.Windows.Controls.Button;
using Label = System.Windows.Controls.Label;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using RadioButton = System.Windows.Controls.RadioButton;
using TextBox = System.Windows.Controls.TextBox;

namespace ShortCuts_Manager.Dialogs
{
    public class AddSingleForm : Window
    {
        public string NameInput { get; set; }
        public string PathInput { get; set; }
        public PathType PathType { get; set; }

        private Label LabelName;
        private Label LabelPath;
        private Label PlbaelPathType;

        private TextBox TextBoxName;
        private TextBox TextBoxPath;

        public RadioButton FileBTN;
        public RadioButton FolderBTN;
        public RadioButton UrlBTN;

        private Button okButton;

        public AddSingleForm(string name = null, string path = null, bool? isFolder = null, bool? isFile = null, bool? isUrl = null)
        {
            Title = Application.Current.MainWindow.Title;
            ResizeMode = ResizeMode.NoResize;
            Width = 250;
            SizeToContent = SizeToContent.Height;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            Owner = Application.Current.MainWindow;
            Icon = Application.Current.MainWindow.Icon;

            this.KeyDown += new System.Windows.Input.KeyEventHandler(OnKeyDownHandler);

            LabelName = new Label
            {
                Content = "Name:",
                Margin = new Thickness(5, 0, 0, 0),
            };

            TextBoxName = new TextBox
            {
                Margin = new Thickness(10, 0, 10, 0),
                VerticalAlignment = VerticalAlignment.Top,
            };

            LabelPath = new Label
            {
                Content = "Path:",
                Margin = new Thickness(5, 0, 0, 0),
            };

            TextBoxPath = new TextBox
            {
                Margin = new Thickness(10, 0, 10, 0),
                VerticalAlignment = VerticalAlignment.Top
            };

            PlbaelPathType = new Label
            {
                Content = "Path type:",
                Margin = new Thickness(5, 0, 0, 0),
            };

            FileBTN = new RadioButton
            {
                Content = "File",
                Margin = new Thickness(10, 0, 10, 0),
                VerticalAlignment = VerticalAlignment.Top,
                GroupName = "PathType",
                IsChecked = isFile == true,
            };

            FileBTN.Checked += (s, e) =>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();

                if (openFileDialog.ShowDialog() == true)
                {
                    TextBoxName.Text = openFileDialog.SafeFileName;
                    TextBoxPath.Text = openFileDialog.FileName;
                }
            };

            FolderBTN = new RadioButton
            {
                Content = "Folder",
                Margin = new Thickness(10, 0, 10, 0),
                VerticalAlignment = VerticalAlignment.Top,
                GroupName = "PathType",
                IsChecked = isFolder == true,
            };

            FolderBTN.Checked += (s, e) =>
            {
                using (var folderBrowserDialog = new FolderBrowserDialog())
                {
                    var result = folderBrowserDialog.ShowDialog();

                    if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
                    {
                        TextBoxName.Text = System.IO.Path.GetFileName(folderBrowserDialog.SelectedPath);
                        TextBoxPath.Text = folderBrowserDialog.SelectedPath;
                    }
                }
            };

            UrlBTN = new RadioButton
            {
                Content = "URL",
                Margin = new Thickness(10, 0, 10, 0),
                VerticalAlignment = VerticalAlignment.Top,
                GroupName = "PathType",
                IsChecked = isUrl == true,
            };

            okButton = new Button
            {
                Content = "OK",
                Width = 60,
                Margin = new Thickness(10),
                VerticalAlignment = VerticalAlignment.Bottom,
            };
            okButton.Click += OkButton_Click;

            if (!string.IsNullOrEmpty(name))
            {
                TextBoxName.Text = name;
            }

            if (!string.IsNullOrEmpty(path))
            {
                TextBoxPath.Text = path;
            }

            if (isUrl.HasValue && isUrl == true)
            {
                TextBoxName.Text = SuggestNameFromUrl(path);
            }

            StackPanel panel = new StackPanel();
            panel.Children.Add(LabelName);
            panel.Children.Add(TextBoxName);
            panel.Children.Add(LabelPath);
            panel.Children.Add(TextBoxPath);
            panel.Children.Add(PlbaelPathType);
            panel.Children.Add(UrlBTN);
            panel.Children.Add(FileBTN);
            panel.Children.Add(FolderBTN);
            panel.Children.Add(okButton);

            Content = panel;
        }
        private string SuggestNameFromUrl(string url)
        {
            try
            {
                Uri uri = new Uri(url);
                return uri.Host.Replace("www.", "");
            }
            catch
            {
                return "New Shortcut";
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            NameInput = TextBoxName.Text;
            PathInput = TextBoxPath.Text;
            PathType = GetSelectedPathType();

            DialogResult = true;
        }

        private void OnKeyDownHandler(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OkButton_Click(this, new RoutedEventArgs());
            }
        }

        public bool ValidateInput()
        {
            var name = TextBoxName.Text.Trim();
            var path = TextBoxPath.Text.Trim();
            var pathType = GetSelectedPathType();

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(path))
            {
                MessageBox.Show("Path and Name cannot be empty", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if ((Application.Current.MainWindow.DataContext as MainWindowViewModel).SingleShortCutInformation.Any(x => x.Name == name))
            {
                MessageBox.Show("Name already exists", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private PathType GetSelectedPathType()
        {
            if (FileBTN.IsChecked == true)
            {
                return PathType.File;
            }
            else if (FolderBTN.IsChecked == true) 
            {
                return PathType.Folder;
            }
            else
            {
                return PathType.Url;
            }
        }
    }
}
