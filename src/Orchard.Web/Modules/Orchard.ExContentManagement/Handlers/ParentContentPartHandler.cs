using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.ExContentManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.ExContentManagement.Handlers
{
    public class ParentContentPartHandler : ContentHandler
    {
        public ParentContentPartHandler(
            IRepository<ParentContentPartRecord> parentPartRepository)
        {
            Filters.Add(StorageFilter.For(parentPartRepository));

            OnPublishing<ParentContentPart>((context, part) => UpdateProperties(part));
        }

        private void UpdateProperties(ParentContentPart part)
        {
            part.ParentId = part.ParentContent != null ? part.ParentContent.Id : 0;
        }
    }
}