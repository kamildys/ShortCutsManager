using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShortCuts_Manager.Interfaces
{
    public interface IFileOpen
    {
        public abstract void OpenFiles(string[] paths);
    }
}
