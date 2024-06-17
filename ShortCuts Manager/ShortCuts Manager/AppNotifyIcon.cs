using ShortCuts_Manager.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace ShortCuts_Manager
{
    public class AppNotifyIcon
    {
        public object ElementToRun { get; set; }

        public void InitNotifyIcon()
        {
            var NotifyIcon = new NotifyIcon()
            {
                Icon = new Icon(System.Windows.Application.GetResourceStream(new Uri("pack://application:,,,/Resources/icon.ico"))?.Stream),
                Visible = true,
                Text = "ShortCuts Manager",
            };

            var singleMenuItem = new ToolStripMenuItem
            {
                Text = "Single",
            };
            singleMenuItem.DropDownOpening += SingleMenuItem_DropDownOpening;
            singleMenuItem.DropDownItems.AddRange(new ToolStripMenuItem[] { new ToolStripMenuItem() { } });

            var groupsMenuItem = new ToolStripMenuItem
            {
                Text = "Groups",
            };
            groupsMenuItem.DropDownOpening += GroupsMenuItem_DropDownOpening;
            groupsMenuItem.DropDownItems.AddRange(new ToolStripMenuItem[] { new ToolStripMenuItem() { } });

            var contextMenuStrip = new ContextMenuStrip();
            contextMenuStrip.ShowImageMargin = false;
            contextMenuStrip.Items.AddRange(new ToolStripItem[]
            {
                singleMenuItem,
                groupsMenuItem
            });

            NotifyIcon.ContextMenuStrip = contextMenuStrip;
            NotifyIcon.Click += NotifyIcon_Click;
        }

        private void SingleMenuItem_DropDownOpening(object? sender, EventArgs e)
        {
            var senderToolStripMenuItem = sender as ToolStripMenuItem;

            senderToolStripMenuItem.DropDownItems.Clear();

            var dropDownItems = new List<ToolStripMenuItem> { };

            foreach (var info in (System.Windows.Application.Current.MainWindow.DataContext as MainWindowViewModel).SingleShortCutInformation)
            {
                dropDownItems.Add(new ToolStripMenuItem()
                {
                    Text = info.Name,
                    Command = (System.Windows.Application.Current.MainWindow.DataContext as MainWindowViewModel).RunSpecificCommand,
                    CommandParameter = info
                });
            }

            senderToolStripMenuItem.DropDownItems.AddRange(dropDownItems.ToArray());
        }

        private void GroupsMenuItem_DropDownOpening(object? sender, EventArgs e)
        {
            var senderToolStripMenuItem = sender as ToolStripMenuItem;

            senderToolStripMenuItem.DropDownItems.Clear();

            var dropDownItems = new List<ToolStripMenuItem> { };

            foreach (var info in (System.Windows.Application.Current.MainWindow.DataContext as MainWindowViewModel).GroupShortCutsInformation)
            {
                dropDownItems.Add(new ToolStripMenuItem()
                {
                    Text = info.Name,
                    Command = (System.Windows.Application.Current.MainWindow.DataContext as MainWindowViewModel).RunSpecificCommand,
                    CommandParameter = info
                });
            }

            senderToolStripMenuItem.DropDownItems.AddRange(dropDownItems.ToArray());
        }

        private void NotifyIcon_Click(object? sender, EventArgs e)
        {
            if ((e as System.Windows.Forms.MouseEventArgs).Button == MouseButtons.Right) return;

            System.Windows.Application.Current.MainWindow.Show();
            System.Windows.Application.Current.MainWindow.WindowState = System.Windows.WindowState.Normal;
            System.Windows.Application.Current.MainWindow.Activate();
        }
    }
}
