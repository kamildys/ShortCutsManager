using Microsoft.Win32;
using ShortCuts_Manager.Helpers.Enums;
using ShortCuts_Manager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ShortCuts_Manager.Dialogs
{
    public class AddSingleForm : Window
    {
        public string NameInput { get; set; }
        public string PathInput { get; set; }

        private Label LabelName;
        private Label LabelPath;
        private Label PlbaelPathType;

        private TextBox TextBoxName;

        private TextBox TextBoxPath;

        private RadioButton FileBTN;
        private RadioButton UrlBTN;

        private Button okButton;

        public AddSingleForm()
        {
            Title = Application.Current.MainWindow.Title;
            ResizeMode = ResizeMode.NoResize;
            Width = 250;
            SizeToContent = SizeToContent.Height;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            Owner = Application.Current.MainWindow;

            LabelName = new Label
            {
                Content = "Name:",
                Margin = new Thickness(5, 0, 0, 0),
            };

            TextBoxName = new TextBox
            {
                Margin = new Thickness(10, 0, 10, 0),
                VerticalAlignment = VerticalAlignment.Top
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

            UrlBTN = new RadioButton
            {
                Content = "URL",
                Margin = new Thickness(10, 0, 10, 0),
                VerticalAlignment = VerticalAlignment.Top,
                GroupName = "PathType",
                IsChecked = true,

            };

            okButton = new Button
            {
                Content = "OK",
                Width = 60,
                Margin = new Thickness(10),
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Right,
                BorderThickness = new Thickness(0),
            };
            okButton.Click += OkButton_Click;

            StackPanel panel = new StackPanel();
            panel.Children.Add(LabelName);
            panel.Children.Add(TextBoxName);
            panel.Children.Add(LabelPath);
            panel.Children.Add(TextBoxPath);
            panel.Children.Add(PlbaelPathType);
            panel.Children.Add(UrlBTN);
            panel.Children.Add(FileBTN);
            panel.Children.Add(okButton);

            Content = panel;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            NameInput = TextBoxName.Text;
            PathInput = TextBoxPath.Text;
            DialogResult = true;
        }

        public bool ValidateInput(out string name, out string path, out PathType pathType)
        {
            name = TextBoxName.Text.Trim();
            path = TextBoxPath.Text.Trim();
            pathType = GetSelectedPathType();

            foreach (var item in (Application.Current.MainWindow.DataContext as MainWindowViewModel).SingleShortCutInformation)
            {
                if (item.Name == name)
                {
                    MessageBox.Show("Name already exists", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(path))
            {
                MessageBox.Show("Path and Name cannot be empty", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
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
            else
            {
                return PathType.Url;
            }
        }

        public void SetFormResult(string name, string path)
        {
            NameInput = name;
            PathInput = path;
            TextBoxName.Text = name;
            TextBoxPath.Text = path;
        }
    }
}
