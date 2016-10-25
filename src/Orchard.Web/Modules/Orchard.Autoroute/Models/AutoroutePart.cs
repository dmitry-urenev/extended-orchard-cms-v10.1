using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;

namespace Orchard.Autoroute.Models {
    public class AutoroutePart : ContentPart<AutoroutePartRecord>, IAliasAspect {

        public string CustomPattern {
            get { return Retrieve(x => x.CustomPattern); }
            set { Store(x => x.CustomPattern, value); }
        }
        public bool UseCustomPattern {
            get { return Retrieve(x => x.UseCustomPattern); }
            set { Store(x => x.UseCustomPattern, value); }
        }
        public bool UseCulturePattern {
            get { return Retrieve(x => x.UseCulturePattern); }
            set { Store(x => x.UseCulturePattern, value); }
        }
        public string DisplayAlias
        {
            get
            {
                return Retrieve(x => x.DisplayAlias);
            }
            set
            {
                var oldValue = Retrieve(x => x.DisplayAlias);
                Store(x => x.DisplayAlias, value);
                OnDisplayAliasChanged(oldValue, value);
            }
        }

        public string LocalAlias
        {
            get
            {
                var _localAlias = Retrieve(x => x.LocalAlias);
                return _localAlias;
            }
            set
            {
                var oldValue = Retrieve(x => x.LocalAlias);
                Store(x => x.LocalAlias, value);
                OnLocalAliasChanged(oldValue, value);
            }
        }

        public bool IsHomePage
        {
            get { return Retrieve(x => x.IsHomePage); }
            set { Store(x => x.IsHomePage, value); }
        }

        private void OnLocalAliasChanged(string oldValue, string newValue)
        {

        }

        private void OnDisplayAliasChanged(string oldValue, string newValue)
        {

        }

        //public bool PromoteToHomePage
        //{
        //    get { return this.Retrieve(x => x.PromoteToHomePage); }
        //    set { this.Store(x => x.PromoteToHomePage, value); }
        //}

        public string Path
        {
            get { return DisplayAlias; }
        }
    }
}
