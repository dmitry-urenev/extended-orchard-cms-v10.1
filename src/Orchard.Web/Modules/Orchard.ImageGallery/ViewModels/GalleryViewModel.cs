using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Orchard.ImageGallery.Models;

namespace Orchard.ImageGallery.ViewModels
{
    public class GalleryViewModel : IEnumerable<GalleryImageViewModel>
    {
        public GalleryViewModel(GalleryPart part)
        {
            Part = part;
            Images = part.Images.Select(i => new GalleryImageViewModel(i)).ToList();
        }

        public IEnumerable<GalleryImageViewModel> Images { get; set; }

        public GalleryPart Part { get; set; }



        public IEnumerator<GalleryImageViewModel> GetEnumerator()
        {
            return Images.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Images.GetEnumerator();
        }
    }
}