using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Orchard.SEO.Models;

namespace Orchard.SEO.Handlers
{
    [OrchardFeature("Orchard.SEO")]
    public class MetaHandler : ContentHandler
    {
        public MetaHandler(IRepository<MetaRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}