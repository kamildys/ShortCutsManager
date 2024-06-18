using ShortCuts_Manager.Dialogs;
using ShortCuts_Manager.ExtensionMethods;
using ShortCuts_Manager.Helpers;
using ShortCuts_Manager.Helpers.Enums;
using ShortCuts_Manager.Interfaces;
using ShortCuts_Manager.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using MessageBox = System.Windows.MessageBox;
using Application = System.Windows.Application;

namespace ShortCuts_Manager
{
    public class MainWindowViewModel
    {
        private AppNotifyIcon AppNotifyIcon { get; set; } = new AppNotifyIcon();

        private readonly IUrlOpen? urlOpen;
        private readonly IFileOpen? fileOpen;
        private readonly IFolderOpen? folderOpen;
        private readonly IDataBase? dataBase;

        public int MainTabSelectedIndex { get; set; }

        public ObservableCollection<SingleShortCutInformation> SingleShortCutInformation { get; set; } = new ObservableCollection<SingleShortCutInformation>();
        public List<SingleShortCutInformation> SelectedSingleShortCutInformation => SingleShortCutInformation.Where(x => x.IsSelected).ToList();

        public ObservableCollection<GroupShortCutsInformation> GroupShortCutsInformation { get; set; } = new ObservableCollection<GroupShortCutsInformation>();
        public GroupShortCutsInformation? SelectedGroupShortCutsInformation { get; set; }

        public string FilterText { get; set; }

        public MainWindowViewModel(IUrlOpen urlOpen, IFileOpen fileOpen, IFolderOpen folderOpen, IDataBase dataBase)
        {
            this.urlOpen = urlOpen;
            this.fileOpen = fileOpen;
            this.folderOpen = folderOpen;
            this.dataBase = dataBase;

            SingleShortCutInformation = new ObservableCollection<SingleShortCutInformation>(dataBase.SingleShortCutInformation.OrderBy(x => x.Name));
            GroupShortCutsInformation = new ObservableCollection<GroupShortCutsInformation>(dataBase.GroupShortCutsInformation.OrderBy(x => x.Name));

            AppNotifyIcon.InitNotifyIcon();
            FilterChanged();
        }

        #region Commands

        #region Run
        private ICommand _runCommand;
        public ICommand RunCommand => _runCommand ??= new CommandHandler((param) => RunSelectedSingleShortCutInformation(), () => true);

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

        #region RunSpecific
        private ICommand _runSpecificCommand;
        public ICommand RunSpecificCommand => _runSpecificCommand ??= new CommandHandler((param) => RunSpecific(param), () => true);

        public void RunSpecific(object info)
        {
            if (info is SingleShortCutInformation infoSingleShortCutInformation)
            {
                if (infoSingleShortCutInformation.PathType == PathType.Url)
                {
                    urlOpen?.OpenUrlsInDefaultBrowser(urls: [infoSingleShortCutInformation.Path]);
                }
                else if (infoSingleShortCutInformation.PathType == PathType.File)
                {
                    fileOpen?.OpenFiles(paths: [infoSingleShortCutInformation.Path]);
                }
                else if (infoSingleShortCutInformation.PathType == PathType.Folder)
                {
                    folderOpen?.OpenFolders(folders: [infoSingleShortCutInformation.Path]);
                }
                return;
            }

            if (info is GroupShortCutsInformation infoGroupShortCutsInformation)
            {
                OpenLinks(
                urls: infoGroupShortCutsInformation.ShortCuts
                        .Where(x => x.PathType == PathType.Url)
                        .Select(x => x.Path)
                        .ToArray(),
                paths: infoGroupShortCutsInformation.ShortCuts
                        .Where(x => x.PathType == PathType.File)
                        .Select(x => x.Path)
                        .ToArray(),
                folders: infoGroupShortCutsInformation.ShortCuts
                        .Where(x => x.PathType == PathType.Folder)
                        .Select(x => x.Path)
                        .ToArray()
                );

                return;
            }
        }
        #endregion RunSpecific

        #region AddGroup
        private ICommand _addGroupCommand;
        public ICommand AddGroupCommand => _addGroupCommand ??= new CommandHandler((param) => AddGroup(), () => true);

        public void AddGroup()
        {
            string? enteredText = null;

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
                MessageBox.Show("Group with this name already exists!", Application.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Error);
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
        public ICommand AddSingleCommand => _addSingleCommand ??= new CommandHandler((param) => OpenAddWindow(), () => true);

        public void OpenAddWindow(string name = null, string path = null)
        {
            AddSingleForm addSingleForm;

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(path))
            {
                addSingleForm = new AddSingleForm();
            }
            else
            {
                addSingleForm = new AddSingleForm(name, path);
            }

            addSingleForm.ShowDialog();

            if (addSingleForm.DialogResult == false) return;

            var dataRow = new SingleShortCutInformation
            {
                Id = Guid.NewGuid(),
                Name = addSingleForm.NameInput,
                Path = addSingleForm.PathInput,
                PathType = addSingleForm.PathType
            };

            SingleShortCutInformation.AddSorted(dataRow, x => x.Name);
            dataBase.AddSingle(dataRow);
        }

        #endregion AddSingle

        #region DeleteSingle
        private ICommand _deleteSingleCommand;
        public ICommand DeleteSingleCommand => _deleteSingleCommand ??= new CommandHandler((param) => DeleteSingle(), () => true);

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
        public ICommand DeleteGroupCommand => _deleteGroupCommand ??= new CommandHandler((param) => DeleteGroup(), () => true);

        public void DeleteGroup()
        {
            dataBase.RemoveGroup(SelectedGroupShortCutsInformation);
            GroupShortCutsInformation.Remove(SelectedGroupShortCutsInformation);
        }
        #endregion DeleteGroup

        #region AssignToGroup
        private ICommand _assignToGroupCommand;
        public ICommand AssignToGroupCommand => _assignToGroupCommand ??= new CommandHandler((param) => AssignToGroup(), () => true);

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
        public ICommand RemoveFromGroupCommand => _removeFromGroupCommand ??= new CommandHandler((param) => RemoveFromGroup(), () => true);

        public void RemoveFromGroup()
        {
            foreach (var ooo in SelectedGroupShortCutsInformation.ShortCuts.Where(x => x.IsSelected).ToList())
            {
                SelectedGroupShortCutsInformation.ShortCuts.Remove(ooo);
                dataBase.RemoveFromGroup(SelectedGroupShortCutsInformation, ooo);
            }
        }
        #endregion RemoveFromGroup

        #region FilterChanged
        private ICommand _filterChangedCommand;
        public ICommand FilterChangedCommand => _filterChangedCommand ??= new CommandHandler((param) => FilterChanged(), () => true);

        public void FilterChanged()
        {
            if (string.IsNullOrEmpty(FilterText))
            {
                foreach (var el in SingleShortCutInformation.ToList())
                {
                    el.IsVisible = true;
                }

                foreach (var el in GroupShortCutsInformation.ToList())
                {
                    el.IsVisible = true;
                }
            }
            else
            {
                foreach (var el in SingleShortCutInformation.ToList())
                {
                    el.IsVisible = el.Name.Contains(FilterText, StringComparison.OrdinalIgnoreCase);
                }

                foreach (var el in GroupShortCutsInformation.ToList())
                {
                    el.IsVisible = el.Name.Contains(FilterText, StringComparison.OrdinalIgnoreCase);
                }
            }
        }
        #endregion FilterChanged

        #endregion Commands

        public void OpenLinks(string[] urls, string[] paths, string[] folders)
        {
            urlOpen?.OpenUrlsInDefaultBrowser(urls: urls);
            fileOpen?.OpenFiles(paths: paths);
            folderOpen?.OpenFolders(folders: folders);
        }
    }
}