using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShortCuts_Manager.Models
{
    public class GroupShortCutsInformation
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ObservableCollection<SingleShortCutInformation> ShortCuts { get; set; } = [];
        public bool IsSelected { get; set; }
    }
}
