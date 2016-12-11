using Orchard.PageScripts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.PageScripts.Services
{
    public interface IPageScriptService : IDependency
    {
        PageScriptsInfo GetScripts();
        void Update(PageScriptsInfo scripts);
    }
}
