using Newtonsoft.Json;
using ShortCuts_Manager.Interfaces;
using ShortCuts_Manager.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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
        private ShortCutInformations shortCutInformations { get; set; }

        public FileXMLDataBase()
        {
            try
            {
                string json = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DB.txt"));
                var infos = JsonConvert.DeserializeObject<ShortCutInformations>(json);

                this.SingleShortCutInformation = infos.SingleShortCutInformation;
                this.GroupShortCutsInformation = infos.GroupShortCutsInformation;
            }
            catch (Exception ex)
            {
            }
        }

        public List<SingleShortCutInformation> SingleShortCutInformation { get; private set; } = new List<SingleShortCutInformation>();
        public List<GroupShortCutsInformation> GroupShortCutsInformation { get; private set; } = new List<GroupShortCutsInformation>();

        public void AddGroup(GroupShortCutsInformation item)
        {
            GroupShortCutsInformation.Add(item);
            SaveData();
        }

        public void AddSingle(SingleShortCutInformation item)
        {
            SingleShortCutInformation.Add(item);
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
    }
}
