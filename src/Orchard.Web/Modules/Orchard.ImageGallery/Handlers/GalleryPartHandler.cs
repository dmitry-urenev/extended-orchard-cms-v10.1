using Orchard.ImageGallery.Models;

using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.ImageGallery.Services;

namespace Orchard.ImageGallery.Handlers
{
    public class GalleryPartHandler : ContentHandler
    {
        private readonly IImageGalleryService _galleryService;

        public GalleryPartHandler(IRepository<GalleryPartRecord> repository, IImageGalleryService galleryService)
        {
            Filters.Add(StorageFilter.For(repository));

            _galleryService = galleryService;

            OnLoaded<GalleryPart>((context, part) =>
            {
                part.ImagesField.Loader(() => _galleryService.GetImages(part));
            });

            OnVersioned<GalleryPart>((context, part, newPart) =>
            {
                newPart.ImagesField = part.ImagesField;
            });
        }
    }
}