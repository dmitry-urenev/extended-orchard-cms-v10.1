using System.Collections.Generic;
using System.Web;
using Orchard.ImageGallery.Models;
using Orchard;
using Orchard.MediaLibrary.Models;

namespace Orchard.ImageGallery.Services
{
    public interface IImageGalleryService : IDependency
    {
        IEnumerable<GalleryPart> GetGalleries();

        GalleryPart GetGallery(int galleryId);

        IEnumerable<GalleryImagePart> GetImages(GalleryPart part);

        void AddImage(GalleryPart gallery, HttpPostedFileBase imageFile);

        void DeleteGallery(GalleryPart gallery);

        GalleryImagePart GetImage(int id);
        void DeleteImage(GalleryImagePart image);

        void SetAlbumCover(GalleryPart gallery, GalleryImagePart img);

        void ReorderImages(int galleryId, IEnumerable<int> images);

        void ReorderGalleries(IEnumerable<int> galleries);

        #region utility methods

        IEnumerable<string> AllowedFileFormats { get; }

        string GetPublicUrl(string path);

        bool IsFileAllowed(HttpPostedFileBase file);

        void CreateFolder(GalleryPart gallery);

        IEnumerable<MediaFile> GetMediaFiles(GalleryPart gallery);

        #endregion
    }
}