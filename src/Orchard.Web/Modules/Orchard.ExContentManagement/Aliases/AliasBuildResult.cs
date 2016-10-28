using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.ExContentManagement.Aliases
{
    public class AliasBuildResult
    {
        public string Alias { get; set; }

        public List<AliasSegment> Segments { get; set; }
    }
}