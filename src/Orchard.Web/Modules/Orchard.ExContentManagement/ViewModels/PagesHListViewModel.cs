using Orchard.ContentManagement;
using Orchard.Core.Contents.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.ExContentManagement.ViewModels
{
    // Hierarchical list of pages
    public class PagesHListViewModel : ListContentsViewModel
    {       
        public string SelectedCulture { get; set; }

        public IEnumerable<string> AvailableCultures { get; set; }

        public new IEnumerable<HEntry> Entries { get; set; }

        #region Nested type: Hierarchical Entry

        public class HEntry : Entry
        {
            public HEntry()
            {
                ChildEntries = Enumerable.Empty<HEntry>();
            }           

            public IEnumerable<HEntry> ChildEntries { get; set; }
        }

        #endregion
    }
}