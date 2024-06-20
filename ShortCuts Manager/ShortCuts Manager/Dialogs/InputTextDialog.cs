using System.Windows.Controls;
using System.Windows;
using Label = System.Windows.Controls.Label;
using TextBox = System.Windows.Controls.TextBox;
using Button = System.Windows.Controls.Button;
using Application = System.Windows.Application;

namespace ShortCuts_Manager.Dialogs
{
    public class InputTextDialog : Window
    {
        private Label label;
        private TextBox textBox;
        private Button okButton;

        public InputTextDialog(string title)
        {
            Title = Application.Current.MainWindow.Title;
            ResizeMode = ResizeMode.NoResize;
            Width = 250;
            SizeToContent = SizeToContent.Height;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            Owner = Application.Current.MainWindow;
            Icon = Application.Current.MainWindow.Icon;

            label = new Label
            {
                Content = title,
                Margin = new Thickness(5,0,0,0),
            };

            textBox = new TextBox
            {
                Margin = new Thickness(10,0,10,0),
                VerticalAlignment = VerticalAlignment.Top
            };

            okButton = new Button
            {
                Content = "OK",
                Width = 60,
                Margin = new Thickness(10),
                VerticalAlignment = VerticalAlignment.Bottom,
            };
            okButton.Click += OkButton_Click;

            StackPanel panel = new StackPanel();
            panel.Children.Add(label);
            panel.Children.Add(textBox);
            panel.Children.Add(okButton);

            Content = panel;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        public string ResultText
        {
            get { return textBox.Text; }
        }
    }

}
