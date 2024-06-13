﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Windows;

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
                HorizontalAlignment = HorizontalAlignment.Right,
                BorderThickness = new Thickness(0),
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