using System.Windows.Interop;
using Application = System.Windows.Application;
using Keyboard = ShortCuts_Manager.Helpers.Keyboard;

namespace ShortCuts_Manager
{
    public class AppNotifyIcon
    {
        public object ElementToRun { get; set; }

        public void InitNotifyIcon()
        {
            var NotifyIcon = new NotifyIcon()
            {
                Icon = new Icon(Application.GetResourceStream(new Uri("pack://application:,,,/Resources/icon.ico"))?.Stream),
                Visible = true,
                Text = "ShortCuts Manager",
            };

            var singleMenuItem = new ToolStripMenuItem
            {
                Text = "     " + "Single" + "          ",
            };
            singleMenuItem.DropDownOpening += SingleMenuItem_DropDownOpening;
            singleMenuItem.DropDownItems.AddRange(new ToolStripMenuItem[] { new ToolStripMenuItem() { } });
            singleMenuItem.DropDown.MaximumSize = new Size(500, 200);

            var groupsMenuItem = new ToolStripMenuItem
            {
                Text = "     " + "Groups" + "          ",
            };
            groupsMenuItem.DropDownOpening += GroupsMenuItem_DropDownOpening;
            groupsMenuItem.DropDownItems.AddRange(new ToolStripMenuItem[] { new ToolStripMenuItem() { } });
            groupsMenuItem.DropDown.MaximumSize = new Size(500, 200);

            var contextMenuStrip = new ContextMenuStrip();
            contextMenuStrip.ShowImageMargin = false;

            contextMenuStrip.Items.AddRange(new ToolStripItem[]
            {
                singleMenuItem,
                new ToolStripSeparator(),
                groupsMenuItem,
            });

            NotifyIcon.ContextMenuStrip = contextMenuStrip;
            NotifyIcon.Click += NotifyIcon_Click;
            ComponentDispatcher.ThreadFilterMessage += ComponentDispatcher_ThreadFilterMessage;
        }

        private void SingleMenuItem_DropDownOpening(object? sender, EventArgs e)
        {
            SetMenuItems(
                sender: sender as ToolStripMenuItem,
                elements: (Application.Current.MainWindow.DataContext as MainWindowViewModel).SingleShortCutInformation);
        }

        private void GroupsMenuItem_DropDownOpening(object? sender, EventArgs e)
        {
            SetMenuItems(
                sender: sender as ToolStripMenuItem,
                elements: (Application.Current.MainWindow.DataContext as MainWindowViewModel).GroupShortCutsInformation);
        }

        private void SetMenuItems<T>(ToolStripMenuItem sender, IEnumerable<T> elements)
        {
            var senderToolStripMenuItem = sender as ToolStripMenuItem;

            senderToolStripMenuItem.DropDownItems.Clear();

            var dropDownItems = new List<ToolStripMenuItem> { };

            foreach (var info in elements)
            {
                dropDownItems.Add(new ToolStripMenuItem()
                {
                    Text = (info as dynamic).Name,
                    Command = (Application.Current.MainWindow.DataContext as MainWindowViewModel).RunSpecificCommand,
                    CommandParameter = info,
                });
            }

            senderToolStripMenuItem.DropDownItems.AddRange(dropDownItems.ToArray());
        }

        private void NotifyIcon_Click(object? sender, EventArgs e)
        {
            if ((e as System.Windows.Forms.MouseEventArgs).Button == MouseButtons.Right) return;

            Application.Current.MainWindow.Show();
            Application.Current.MainWindow.WindowState = System.Windows.WindowState.Normal;
            Application.Current.MainWindow.Activate();
        }

        private void ComponentDispatcher_ThreadFilterMessage(ref MSG msg, ref bool handled)
        {
            const int WM_MOUSEWHEEL = 0x020A;

            if (msg.message == WM_MOUSEWHEEL)
            {
                int delta = 0;
                unchecked
                {
                    int wParam = (int)msg.wParam;
                    delta = (short)((wParam >> 16) & 0xFFFF);
                }
                OnMouseWheel(delta);
                handled = true;
            }
        }

        private void OnMouseWheel(long delta)
        {
            if (delta > 0)
            {
                Keyboard.KeyUp();
            }
            else
            {
                Keyboard.KeyDown();
            }
        }
    }
}
