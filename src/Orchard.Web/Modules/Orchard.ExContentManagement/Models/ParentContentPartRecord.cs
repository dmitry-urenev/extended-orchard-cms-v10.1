using Orchard.ContentManagement.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.ExContentManagement.Models
{   
    public class ParentContentPartRecord : ContentPartRecord
    {
        public virtual int ParentId { get; set; }
    }
}