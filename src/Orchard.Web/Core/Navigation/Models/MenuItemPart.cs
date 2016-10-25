using Orchard.ContentManagement;

namespace Orchard.Core.Navigation.Models {
    public class MenuItemPart : ContentPart {
        public string Url
        {
            get { return this.Retrieve(x => x.Url); }
            set { this.Store(x => x.Url, value); }
        }

        public bool TargetBlank
        {
            get { return this.Retrieve(x => x.TargetBlank); }
            set { this.Store(x => x.TargetBlank, value); }
        }
    }
}
