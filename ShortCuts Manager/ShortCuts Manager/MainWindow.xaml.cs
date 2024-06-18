﻿using System.IO;
using ShortCuts_Manager.Dialogs;
using System.Windows;
using DragEventHandler = System.Windows.Forms.DragEventHandler;
using MessageBox = System.Windows.MessageBox;

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
                if (files.Length > 1)
                {
                    MessageBox.Show("Please drop only one file or folder at a time.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (files.Length == 1)
                {
                    string path = files[0];
                    string name = Path.GetFileName(path);

                    (System.Windows.Application.Current.MainWindow.DataContext as MainWindowViewModel).OpenAddWindow(name, path);
                }
            }
        }
    }
}