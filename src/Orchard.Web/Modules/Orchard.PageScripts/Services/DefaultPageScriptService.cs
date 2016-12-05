using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.PageScripts.Models;
using Orchard.Data;

namespace Orchard.PageScripts.Services
{
    public class DefaultPageScriptService : IPageScriptService
    {

        private readonly IRepository<PageScriptsInfo> _repo;

        public DefaultPageScriptService(IRepository<PageScriptsInfo> repo)
        {
            _repo = repo;
        }

        public PageScriptsInfo GetScripts()
        {
            var scripts = _repo.Table.FirstOrDefault();
            if (scripts == null)
            {
                scripts = new PageScriptsInfo();
                _repo.Create(scripts);
            }
            return scripts;
        }

        public void Update(PageScriptsInfo scripts)
        {
            _repo.Update(scripts);
        }
    }
}
