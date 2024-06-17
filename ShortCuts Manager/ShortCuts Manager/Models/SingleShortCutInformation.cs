using ShortCuts_Manager.Helpers.Enums;

namespace ShortCuts_Manager.Models
{
    public class SingleShortCutInformation
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public PathType PathType { get; set; }

        public bool IsSelected { get; set; }
    }
}
