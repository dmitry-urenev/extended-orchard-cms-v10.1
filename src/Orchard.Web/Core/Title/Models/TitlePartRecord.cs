using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement.Records;

namespace Orchard.Core.Title.Models {
    public class TitlePartRecord : ContentPartVersionRecord
    {
        public TitlePartRecord()
        {
            RenderTitle = true;
        }

        [StringLength(1024)]
        public virtual string Title { get; set; }

        public virtual bool RenderTitle { get; set; }
    }
}
