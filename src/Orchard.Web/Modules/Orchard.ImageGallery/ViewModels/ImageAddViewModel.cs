using System.Collections.Generic;
using System.Web;

namespace Orchard.ImageGallery.ViewModels {
    public class ImageAddViewModel {

        public int GalleryId { get; set; }

        public IEnumerable<HttpPostedFileBase> ImageFiles { get; set; }

        public IEnumerable<string> AllowedFiles { get; set; }
    }
}