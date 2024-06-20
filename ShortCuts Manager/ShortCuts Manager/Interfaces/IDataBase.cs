using ShortCuts_Manager.Models;

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

        void Import(string filename);

        void Export(string filename, List<SingleShortCutInformation> singleShortCutInformation, List<GroupShortCutsInformation> groupShortCutsInformation);

    }
}
