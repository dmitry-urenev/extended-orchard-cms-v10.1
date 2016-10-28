using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.Records;

namespace Orchard.Localization.Models
{
    public class CultureSwitcherPartRecord : ContentPartRecord
    {
        public virtual int DisplayMode { get; set; }

        public virtual int DisplayType { get; set; }

        public virtual string OrderedCultures { get; set; }
    }
}