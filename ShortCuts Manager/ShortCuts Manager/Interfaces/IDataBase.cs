using ShortCuts_Manager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShortCuts_Manager.Interfaces
{
    public interface IDataBase
    {
        List<SingleShortCutInformation> SingleShortCutInformation { get; }
        List<GroupShortCutsInformation> GroupShortCutsInformation { get; }

        void AddSingle(SingleShortCutInformation item);
        void RemoveSingle(SingleShortCutInformation item);

        void AddGroup(GroupShortCutsInformation item);
        void RemoveGroup(GroupShortCutsInformation item);

        void AddToGroup(GroupShortCutsInformation group, SingleShortCutInformation item);

        void RemoveFromGroup(GroupShortCutsInformation group, SingleShortCutInformation item);
    }
}
