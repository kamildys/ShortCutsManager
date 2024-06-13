using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShortCuts_Manager.Interfaces
{
    public interface IUrlOpen
    {
        public abstract void OpenUrlsInDefaultBrowser(string[] urls);
    }
}
