using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.PageScripts.Models
{
    public class PageScriptsInfo
    {
        public virtual int Id { get; set; }

        public virtual string TopHeaderScript { get; set; }

        public virtual string BottomHeaderScript { get; set; }

        public virtual string TopBodyScript { get; set; }

        public virtual string BottomBodyScript { get; set; }
    }
}
