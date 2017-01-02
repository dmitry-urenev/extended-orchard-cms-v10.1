using Orchard.ImageGallery.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.ImageGallery.Services;
using System.Web.Routing;
using System.Linq;
using Orchard.Core.Common.Models;

namespace Orchard.ImageGallery.Handlers
{
    public class GalleryImagePartHandler : ContentHandler
    {
        private readonly IImageGalleryService _galleryService;

        public GalleryImagePartHandler(IImageGalleryService galleryService)
        {
            _galleryService = galleryService;

            OnRemoved<GalleryPart>(
                (context, gallery) =>
                {
                    foreach (var i in gallery.Images)
                    {
                        context.ContentManager.Remove(i.ContentItem);
                    }
                });

            OnPublished<GalleryImagePart>((context, img) =>
            {
                var gallery = img.GalleryPart;
                if (img.IsCover.GetValueOrDefault() && !string.Equals(img.PublicUrl, gallery.AlbumCoverUrl))
                    _galleryService.SetAlbumCover(gallery, img);
            });

            OnRemoved<GalleryImagePart>((context, img) =>
            {
                var gallery = img.GalleryPart;
                if (img.IsCover.GetValueOrDefault())
                    _galleryService.SetAlbumCover(gallery, null);
            });
        }
    }
}