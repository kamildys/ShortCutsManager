using System.Collections.ObjectModel;

namespace ShortCuts_Manager.Models
{
    public class GroupShortCutsInformation
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ObservableCollection<SingleShortCutInformation> ShortCuts { get; set; } = [];
        public bool IsSelected { get; set; }
        public bool IsVisible { get; set; } = true;
    }
}
