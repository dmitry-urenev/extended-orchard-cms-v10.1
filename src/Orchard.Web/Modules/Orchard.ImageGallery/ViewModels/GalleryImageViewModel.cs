using System.Collections.Generic;
using Orchard.ImageGallery.Models;
using Orchard.MediaLibrary.Models;

namespace Orchard.ImageGallery.ViewModels
{
    public class GalleryImageViewModel
    {
        public GalleryImageViewModel(GalleryImagePart part)
        {
            Part = part;
        }

        public GalleryImagePart Part { get; set; }

        public MediaFile File { get; set; }
    }
}