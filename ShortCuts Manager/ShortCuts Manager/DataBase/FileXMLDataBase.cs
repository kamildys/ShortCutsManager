using Newtonsoft.Json;
using ShortCuts_Manager.Interfaces;
using ShortCuts_Manager.Models;
using System.IO;
using System.Linq;
using System.Windows;

namespace ShortCuts_Manager.DataBase
{
    #region DTO
    public class ShortCutInformations
    {
        public List<SingleShortCutInformation> SingleShortCutInformation { get; set; }
        public List<GroupShortCutsInformation> GroupShortCutsInformation { get; set; }
    }
    #endregion DTO

    public class FileXMLDataBase : IDataBase
    {
        public FileXMLDataBase()
        {
            try
            {
                string json = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DB.txt"));
                var infos = JsonConvert.DeserializeObject<ShortCutInformations>(json);

                this.SingleShortCutInformation = infos.SingleShortCutInformation;
                this.GroupShortCutsInformation = infos.GroupShortCutsInformation;
            }
            catch (Exception)
            {
            }
        }

        public List<SingleShortCutInformation> SingleShortCutInformation { get; private set; } = new List<SingleShortCutInformation>();
        public List<GroupShortCutsInformation> GroupShortCutsInformation { get; private set; } = new List<GroupShortCutsInformation>();

        public void AddGroup(GroupShortCutsInformation item)
        {
            if (!GroupShortCutsInformation.Any(x => x.Name != item.Name))
            {
                GroupShortCutsInformation.Add(item);
            }

            SaveData();
        }

        public void AddSingle(SingleShortCutInformation item)
        {
            if (!SingleShortCutInformation.Any(x => x.Name != item.Name))
            {
                SingleShortCutInformation.Add(item);
            }

            SaveData();
        }

        public void RemoveGroup(GroupShortCutsInformation item)
        {
            GroupShortCutsInformation.Remove(item);
            SaveData();
        }

        public void RemoveSingle(SingleShortCutInformation item)
        {
            SingleShortCutInformation.Remove(item);
            SaveData();
        }

        public void AddToGroup(GroupShortCutsInformation group, SingleShortCutInformation item)
        {
            if (!group.ShortCuts.Any(x => x.Id == item.Id))
            {
                group.ShortCuts.Add(item);
            }

            SaveData();
        }

        public void RemoveFromGroup(GroupShortCutsInformation group, SingleShortCutInformation item)
        {
            if (group.ShortCuts.Any(x => x.Id == item.Id))
            {
                group.ShortCuts.Remove(item);
            }

            SaveData();
        }

        private void SaveData()
        {
            string json = JsonConvert.SerializeObject(
                new ShortCutInformations() { 
                    GroupShortCutsInformation = GroupShortCutsInformation, 
                    SingleShortCutInformation = SingleShortCutInformation
                }, 
                Formatting.Indented);

            File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DB.txt"), json);
        }

        public void Import(string filename)
        {
            try
            {
                string json = File.ReadAllText(filename);
                var infos = JsonConvert.DeserializeObject<ShortCutInformations>(json);

                List<SingleShortCutInformation> singleShortCutInformation_error = 
                    infos.SingleShortCutInformation
                    .Where( y => 
                        SingleShortCutInformation
                        .Select(x => x.Name)
                        .Contains(y.Name)
                    ).ToList();
                
                List<GroupShortCutsInformation> groupShortCutsInformation_error =
                    infos.GroupShortCutsInformation
                    .Where(y =>
                        GroupShortCutsInformation
                        .Select(x => x.Name)
                        .Contains(y.Name)
                    ).ToList();

                if(singleShortCutInformation_error.Count > 0)
                {
                    System.Windows.MessageBox.Show(
                        string.Join("\n", singleShortCutInformation_error.Select(x => string.Format("{0} - Name already exists", x.Name))),
                        "Import error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }

                if (groupShortCutsInformation_error.Count > 0)
                {
                    System.Windows.MessageBox.Show(
                        string.Join("\n", groupShortCutsInformation_error.Select(x => string.Format("{0} - Name already exists", x.Name))),
                        "Import error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }

                foreach (var single in infos.SingleShortCutInformation)
                {
                    AddSingle(single);
                }

                foreach (var group in infos.GroupShortCutsInformation)
                {
                    AddGroup(group);
                }
            } 
            catch (Exception) 
            { }
        }

        public void Export(
            string filename,
            List<SingleShortCutInformation> singleShortCutInformation = null,
            List<GroupShortCutsInformation> groupShortCutsInformation = null)
        {
            //All
            if (singleShortCutInformation == null && groupShortCutsInformation == null)
            {
                string json = JsonConvert.SerializeObject(
                    new ShortCutInformations()
                    {
                        GroupShortCutsInformation = GroupShortCutsInformation,
                        SingleShortCutInformation = SingleShortCutInformation
                    },
                    Formatting.Indented);

                File.WriteAllText(filename, json);
            }
            //Selected
            else
            {
                string json = JsonConvert.SerializeObject(
                    new ShortCutInformations()
                    {
                        GroupShortCutsInformation = groupShortCutsInformation,
                        SingleShortCutInformation = singleShortCutInformation
                    },
                    Formatting.Indented);

                File.WriteAllText(filename, json);
            }
        }
    }
}
