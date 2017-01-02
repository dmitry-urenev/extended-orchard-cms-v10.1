using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;

namespace Orchard.Core.Title.Models {
    public class TitlePart : ContentPart<TitlePartRecord>, ITitleAspect {

        [Required]
        public string Title {
            get { return Retrieve(x => x.Title); }
            set { Store(x => x.Title, value); }
        }

        public bool RenderTitle
        {
            get { return Retrieve(x => x.RenderTitle); }
            set { Store(x => x.RenderTitle, value); }
        }

        // TODO: implement this as setting 
        public bool EnableRenderTitleSetting { get; set; }
    }
}