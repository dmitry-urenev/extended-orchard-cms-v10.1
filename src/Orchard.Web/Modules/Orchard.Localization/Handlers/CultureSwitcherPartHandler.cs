using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Localization.Models;

namespace Orchard.Localization.Handlers
{
    public class CultureSwitcherPartHandler : ContentHandler
    {
        public CultureSwitcherPartHandler(IRepository<CultureSwitcherPartRecord> switcherSettingsPartRepository)
        {
            Filters.Add(StorageFilter.For(switcherSettingsPartRepository));
        }
    }
}