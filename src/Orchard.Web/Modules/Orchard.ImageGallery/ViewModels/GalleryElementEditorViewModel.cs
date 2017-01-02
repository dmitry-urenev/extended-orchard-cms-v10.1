using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.ImageGallery.ViewModels
{
    public class GalleryElementEditorViewModel
    {
        /// <summary>
        /// The selected gallery to be used.
        /// </summary>
        public int SelectedGallery { get; set; } // used on editor

        /// <summary>
        /// Galleries available to be selected.
        /// </summary>
        public IEnumerable<Models.GalleryPart> AvailableGalleries { get; set; } // used on editor

        /// <summary>
        /// Indicates if there is any image gallery available.
        /// </summary>
        public bool HasAvailableGalleries
        {
            get { return AvailableGalleries != null && AvailableGalleries.Any(); }
        }
    }
}
