using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;

namespace Orchard.ExContentManagement.ViewModels
{
    public class ParentContentPartEditViewModel
    {
        public int ParentId { get; set; }

        public ContentItem ParentContent { get; set; }

        public string ParentContentTitle { get; set; }

        public string ContentTypeName { get; set; }
    }
}