using ShortCuts_Manager.Dialogs;
using ShortCuts_Manager.ExtensionMethods;
using ShortCuts_Manager.Helpers;
using ShortCuts_Manager.Helpers.Enums;
using ShortCuts_Manager.Interfaces;
using ShortCuts_Manager.Models;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace ShortCuts_Manager
{
    public class MainWindowViewModel
    {
        private IUrlOpen? urlOpen;
        private IFileOpen? fileOpen;
        private IDataBase? dataBase;

        public int MainTabSelectedIndex { get; set; }

        public ObservableCollection<SingleShortCutInformation> SingleShortCutInformation { get; set; } = new ObservableCollection<SingleShortCutInformation>();
        public List<SingleShortCutInformation> SelectedSingleShortCutInformation {
            get
            {
                return SingleShortCutInformation.Where(x => x.IsSelected).ToList();
            }
        }

        public ObservableCollection<GroupShortCutsInformation> GroupShortCutsInformation { get; set; } = new ObservableCollection<GroupShortCutsInformation>();
        public GroupShortCutsInformation? SelectedGroupShortCutsInformation { get; set; }

        public MainWindowViewModel(IUrlOpen urlOpen, IFileOpen fileOpen, IDataBase dataBase)
        {
            this.urlOpen = urlOpen;
            this.fileOpen = fileOpen;
            this.dataBase = dataBase;

            SingleShortCutInformation = new ObservableCollection<SingleShortCutInformation>(dataBase.SingleShortCutInformation.OrderBy(x => x.Name));
            GroupShortCutsInformation = new ObservableCollection<GroupShortCutsInformation>(dataBase.GroupShortCutsInformation.OrderBy(x => x.Name));
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

            if(GroupShortCutsInformation.Any(x => x.Name == enteredText))
            {
                MessageBox.Show("Group with this name already exists!", Application.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var dataRow = new GroupShortCutsInformation
            {
                Id = Guid.NewGuid(),
                Name = enteredText
            };

            //GroupShortCutsInformation.Add(dataRow);
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
            AddSingleForm addSingleForm = new AddSingleForm();
            if (addSingleForm.ShowDialog() == true)
            {
                addSingleForm.GetFormResult(out string name, out string path, out PathType pathType);

                var dataRow = new SingleShortCutInformation
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    Path = path,
                    PathType = pathType
                };

                //SingleShortCutInformation.Add(dataRow);
                SingleShortCutInformation.AddSorted(dataRow, x => x.Name);

                dataBase.AddSingle(dataRow);
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
            foreach(var info in SelectedSingleShortCutInformation.ToList())
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

                if(pickedGroup is null) return;

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
            foreach(var ooo in SelectedGroupShortCutsInformation.ShortCuts.Where(x => x.IsSelected).ToList())
            {
                SelectedGroupShortCutsInformation.ShortCuts.Remove(ooo);
                dataBase.RemoveFromGroup(SelectedGroupShortCutsInformation, ooo);
            }
        }
        #endregion RemoveFromGroup

        #endregion Commands

        public void OpenLinks(string[] urls, string[] paths)
        {
            urlOpen?.OpenUrlsInDefaultBrowser(urls: urls);

            fileOpen?.OpenFiles(paths: paths);
        }
    }
}