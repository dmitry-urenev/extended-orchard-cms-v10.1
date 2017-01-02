using Orchard.Layouts.Framework.Elements;
using Orchard.Layouts.Helpers;
using Orchard.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.ImageGallery.Elements
{
    public class GalleryElement : Element
    {
        public override string Category
        {
            get { return "Media"; }
        }

        public override LocalizedString DisplayText
        {
            get { return T("Image Gallery"); }
        }

        public override string ToolboxIcon
        {
            get { return "\uf1c5"; }
        }

        public int? GalleryId
        {
            get { return this.Retrieve(x => x.GalleryId); }
            set { this.Store(x => x.GalleryId, value); }
        }
    }
}
