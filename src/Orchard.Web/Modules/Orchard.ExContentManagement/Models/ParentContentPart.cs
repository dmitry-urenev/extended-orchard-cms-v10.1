using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;

namespace Orchard.ExContentManagement.Models
{
    public class ParentContentPart : ContentPart<ParentContentPartRecord>
    {
        public IContent ParentContent
        {
            get { return this.As<ICommonPart>().Container; }
            set { this.As<ICommonPart>().Container = value; }
        }

        public int ParentId
        {
            get { return Record.ParentId; }
            set { Record.ParentId = value; }
        }
    }
}