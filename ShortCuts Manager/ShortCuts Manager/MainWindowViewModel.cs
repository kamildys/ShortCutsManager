using ShortCuts_Manager.Dialogs;
using ShortCuts_Manager.ExtensionMethods;
using ShortCuts_Manager.Helpers;
using ShortCuts_Manager.Helpers.Enums;
using ShortCuts_Manager.Interfaces;
using ShortCuts_Manager.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MessageBox = System.Windows.MessageBox;

namespace ShortCuts_Manager
{
    public class MainWindowViewModel
    {
        //private System.Windows.Forms.NotifyIcon NotifyIcon;

        private IUrlOpen? urlOpen;
        private IFileOpen? fileOpen;
        private IFolderOpen? folderOpen;
        private IDataBase? dataBase;

        public int MainTabSelectedIndex { get; set; }

        public ObservableCollection<SingleShortCutInformation> SingleShortCutInformation { get; set; } = new ObservableCollection<SingleShortCutInformation>();
        public List<SingleShortCutInformation> SelectedSingleShortCutInformation
        {
            get
            {
                return SingleShortCutInformation.Where(x => x.IsSelected).ToList();
            }
        }

        public ObservableCollection<GroupShortCutsInformation> GroupShortCutsInformation { get; set; } = new ObservableCollection<GroupShortCutsInformation>();
        public GroupShortCutsInformation? SelectedGroupShortCutsInformation { get; set; }

        public MainWindowViewModel(IUrlOpen urlOpen, IFileOpen fileOpen, IFolderOpen folderOpen, IDataBase dataBase)
        {
            this.urlOpen = urlOpen;
            this.fileOpen = fileOpen;
            this.folderOpen = folderOpen;
            this.dataBase = dataBase;

            SingleShortCutInformation = new ObservableCollection<SingleShortCutInformation>(dataBase.SingleShortCutInformation.OrderBy(x => x.Name));
            GroupShortCutsInformation = new ObservableCollection<GroupShortCutsInformation>(dataBase.GroupShortCutsInformation.OrderBy(x => x.Name));

            InitNotifyIcon();
        }

        #region Commands

        #region Run
        private ICommand _runCommand;
        public ICommand RunCommand
        {
            get
            {
                return _runCommand ?? (_runCommand = new CommandHandler(() => RunSelectedSingleShortCutInformation(), () => CanRun));
            }
        }
        public bool CanRun
        {
            get
            {
                return true;
            }
        }

        public void RunSelectedSingleShortCutInformation()
        {
            if (MainTabSelectedIndex == 0)
            {
                OpenLinks(
                    urls: SelectedSingleShortCutInformation
                            .Where(x => x.PathType == PathType.Url)
                            .Select(x => x.Path)
                            .ToArray(),
                    paths: SelectedSingleShortCutInformation
                            .Where(x => x.PathType == PathType.File)
                            .Select(x => x.Path)
                            .ToArray(),
                    folders: SelectedSingleShortCutInformation
                        .Where(x => x.PathType == PathType.Folder)
                        .Select(x => x.Path)
                        .ToArray()
                    );
            }
            else
            {
                OpenLinks(
                    urls: SelectedGroupShortCutsInformation.ShortCuts
                            .Where(x => x.PathType == PathType.Url)
                            .Select(x => x.Path)
                            .ToArray(),
                    paths: SelectedGroupShortCutsInformation.ShortCuts
                            .Where(x => x.PathType == PathType.File)
                            .Select(x => x.Path)
                            .ToArray(),
                    folders: SelectedGroupShortCutsInformation.ShortCuts
                        .Where(x => x.PathType == PathType.Folder)
                        .Select(x => x.Path)
                        .ToArray()
                    );
            }
        }
        #endregion Run

        #region AddGroup
        private ICommand _addGroupCommand;
        public ICommand AddGroupCommand
        {
            get
            {
                return _addGroupCommand ?? (_addGroupCommand = new CommandHandler(() => AddGroup(), () => CanAddGroup));
            }
        }
        public bool CanAddGroup
        {
            get
            {
                return true;
            }
        }

        public void AddGroup()
        {
            string enteredText = null;

            InputTextDialog inputDialog = new InputTextDialog("Enter name:");
            if (inputDialog.ShowDialog() == true)
            {
                enteredText = inputDialog.ResultText;
            }

            if (string.IsNullOrEmpty(enteredText))
            {
                return;
            }

            if (GroupShortCutsInformation.Any(x => x.Name == enteredText))
            {
                MessageBox.Show("Group with this name already exists!", System.Windows.Application.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var dataRow = new GroupShortCutsInformation
            {
                Id = Guid.NewGuid(),
                Name = enteredText
            };

            GroupShortCutsInformation.AddSorted(dataRow, x => x.Name);

            dataBase.AddGroup(dataRow);
        }
        #endregion AddGroup

        #region AddSingle
        private ICommand _addSingleCommand;
        public ICommand AddSingleCommand
        {
            get
            {
                return _addSingleCommand ?? (_addSingleCommand = new CommandHandler(() => AddSingle(), () => CanAddSingle));
            }
        }
        public bool CanAddSingle
        {
            get
            {
                return true;
            }
        }

        public void AddSingle()
        {
            string name = string.Empty;
            string path = string.Empty;

            while (true)
            {
                var addSingleForm = new AddSingleForm();

                if (!string.IsNullOrEmpty(name) || !string.IsNullOrEmpty(path))
                {
                    addSingleForm.SetFormResult(name, path);
                }

                if (addSingleForm.ShowDialog() == true)
                {
                    if (addSingleForm.ValidateInput(out name, out path, out PathType pathType))
                    {
                        var dataRow = new SingleShortCutInformation
                        {
                            Id = Guid.NewGuid(),
                            Name = name,
                            Path = path,
                            PathType = pathType
                        };

                        SingleShortCutInformation.AddSorted(dataRow, x => x.Name);

                        dataBase.AddSingle(dataRow);
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
        }

        #endregion AddSingle

        #region DeleteSingle
        private ICommand _deleteSingleCommand;
        public ICommand DeleteSingleCommand
        {
            get
            {
                return _deleteSingleCommand ?? (_deleteSingleCommand = new CommandHandler(() => DeleteSingle(), () => CanDeleteSingle));
            }
        }
        public bool CanDeleteSingle
        {
            get
            {
                return true;
            }
        }

        public void DeleteSingle()
        {
            foreach (var info in SelectedSingleShortCutInformation.ToList())
            {
                SingleShortCutInformation.Remove(info);
                dataBase.RemoveSingle(info);
            }
        }
        #endregion DeleteSingle

        #region DeleteGroup
        private ICommand _deleteGroupCommand;
        public ICommand DeleteGroupCommand
        {
            get
            {
                return _deleteGroupCommand ?? (_deleteGroupCommand = new CommandHandler(() => DeleteGroup(), () => CanDeleteGroup));
            }
        }
        public bool CanDeleteGroup
        {
            get
            {
                return true;
            }
        }

        public void DeleteGroup()
        {
            dataBase.RemoveGroup(SelectedGroupShortCutsInformation);
            GroupShortCutsInformation.Remove(SelectedGroupShortCutsInformation);
        }
        #endregion DeleteGroup

        #region AssignToGroup
        private ICommand _assignToGroupCommand;
        public ICommand AssignToGroupCommand
        {
            get
            {
                return _assignToGroupCommand ?? (_assignToGroupCommand = new CommandHandler(() => AssignToGroup(), () => CanAssignToGroup));
            }
        }
        public bool CanAssignToGroup
        {
            get
            {
                return true;
            }
        }

        public void AssignToGroup()
        {
            ComboBoxDialog assignToGroupForm = new ComboBoxDialog("Select group", GroupShortCutsInformation, "Name");
            if (assignToGroupForm.ShowDialog() == true)
            {
                var pickedGroup = assignToGroupForm.ResultItem as GroupShortCutsInformation;

                if (pickedGroup is null) return;

                foreach (var newItem in SelectedSingleShortCutInformation)
                {
                    if (!pickedGroup.ShortCuts.Any(existingItem => existingItem.Id == newItem.Id))
                    {
                        pickedGroup.ShortCuts.Add(newItem);
                    }
                }

                foreach (var singleShortCut in SelectedSingleShortCutInformation)
                {
                    dataBase.AddToGroup(pickedGroup, singleShortCut);
                }
            }
        }
        #endregion AssignToGroup

        #region RemoveFromGroup
        private ICommand _removeFromGroupCommand;
        public ICommand RemoveFromGroupCommand
        {
            get
            {
                return _removeFromGroupCommand ?? (_removeFromGroupCommand = new CommandHandler(() => RemoveFromGroup(), () => CanRemoveFromGroup));
            }
        }
        public bool CanRemoveFromGroup
        {
            get
            {
                return true;
            }
        }

        public void RemoveFromGroup()
        {
            foreach (var ooo in SelectedGroupShortCutsInformation.ShortCuts.Where(x => x.IsSelected).ToList())
            {
                SelectedGroupShortCutsInformation.ShortCuts.Remove(ooo);
                dataBase.RemoveFromGroup(SelectedGroupShortCutsInformation, ooo);
            }
        }
        #endregion RemoveFromGroup

        #endregion Commands

        public void OpenLinks(string[] urls, string[] paths, string[] folders)
        {
            urlOpen?.OpenUrlsInDefaultBrowser(urls: urls);

            fileOpen?.OpenFiles(paths: paths);

            folderOpen?.OpenFolders(folders: folders);
        }

        #region NotifyIcon
        public void InitNotifyIcon()
        {
            var NotifyIcon = new NotifyIcon()
            {
                Icon = new Icon("C:\\GIT\\ShortCutsManager\\ShortCuts Manager\\ShortCuts Manager\\Resources\\icon.ico"),
                Visible = true,
                Text = "ShortCuts Manager",
            };

            NotifyIcon.Click += NotifyIcon_Click;

            ContextMenu contextMenu = new ContextMenu();

            var contextMenuStrip = new ContextMenuStrip();

            var singleMenuItem = new ToolStripMenuItem
            {
                Text = "Single"
            };

            singleMenuItem.DropDownOpening += SingleMenuItem_DropDownOpening;
            singleMenuItem.DropDownItems.AddRange(new ToolStripMenuItem[] { new ToolStripMenuItem() { } });

            var groupsMenuItem = new ToolStripMenuItem
            {
                Text = "Groups"
            };

            contextMenuStrip.Items.AddRange(new ToolStripItem[]
            {
                singleMenuItem,
                groupsMenuItem
            });

            NotifyIcon.ContextMenuStrip = contextMenuStrip;
        }

        private void SingleMenuItem_DropDownOpening(object? sender, EventArgs e)
        {
            var senderToolStripMenuItem = sender as ToolStripMenuItem;

            senderToolStripMenuItem.DropDownItems.Clear();

            senderToolStripMenuItem.DropDownItems.AddRange(new ToolStripMenuItem[]
            {
                new ToolStripMenuItem()
                {
                    Text = "subItem"
                }
            });
        }

        private void NotifyIcon_Click(object? sender, EventArgs e)
        {
            if ((e as System.Windows.Forms.MouseEventArgs).Button == MouseButtons.Right) return;

            System.Windows.Application.Current.MainWindow.Show();
            System.Windows.Application.Current.MainWindow.WindowState = System.Windows.WindowState.Normal;
            System.Windows.Application.Current.MainWindow.Activate();
        }
        #endregion NotifyIcon
    }
}