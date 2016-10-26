using System.Collections.Generic;
using Orchard.ContentManagement;

namespace Orchard.Localization.ViewModels {
    public class EditLocalizationViewModel  {
        public string SelectedCulture { get; set; }
        public IEnumerable<string> AvailableCultures { get; set; }
        public string SiteCulture { get; set; }
        public IContent ContentItem { get; set; }
        public IContent MasterContentItem { get; set; }
        public ContentLocalizationsViewModel ContentLocalizations { get; set; }
    }
}