using ShortCuts_Manager.Helpers.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
