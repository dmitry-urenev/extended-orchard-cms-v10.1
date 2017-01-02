using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

using ICSharpCode.SharpZipLib.Zip;

using Orchard.ImageGallery.Models;

using Orchard;
using Orchard.Data;
using Orchard.FileSystems.Media;
using Orchard.Localization;
using Orchard.MediaLibrary.Models;
using Orchard.MediaLibrary.Services;
using Orchard.Validation;
using Orchard.UI.Notify;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
using Orchard.Core.Common.Models;

namespace Orchard.ImageGallery.Services
{
	public class ImageGalleryService : IImageGalleryService
	{
		private const string ImageGalleriesMediaFolder = "ImageGalleries";

		private readonly IContentManager _contentManager;
        private readonly IMediaLibraryService _mediaService;
		private readonly IThumbnailService _thumbnailService;
		private readonly IRepository<GalleryPartRecord> _imageGalleryPartRepository;
		private readonly IOrchardServices _services;
		private readonly IStorageProvider _storageProvider;

		private readonly IList<string> _imageFileFormats = new[] { "BMP", "GIF", "EXIF", "JPG", "PNG", "TIFF" };
		private readonly IList<string> _fileFormats = new[] { "BMP", "GIF", "EXIF", "JPG", "PNG", "TIFF", "ZIP" };

        public ImageGalleryService(IContentManager contentManager,
                                   IMediaLibraryService mediaService,
                                   IThumbnailService thumbnailService,
                                   IRepository<GalleryPartRecord> imageGalleryPartRepository, IOrchardServices services,
                                   IStorageProvider storageProvider)
        {
            _contentManager = contentManager;
            _storageProvider = storageProvider;
            _services = services;
            _imageGalleryPartRepository = imageGalleryPartRepository;
            _mediaService = mediaService;
            _thumbnailService = thumbnailService;

            if (_mediaService.GetMediaFolders(string.Empty).All(o => o.Name != ImageGalleriesMediaFolder))
            {
                _mediaService.CreateFolder(string.Empty, ImageGalleriesMediaFolder);
            }
        }

		public IEnumerable<string> AllowedFileFormats
		{
			get { return _fileFormats; }
		}

        public void DeleteGallery(GalleryPart gallery)
        {
            _contentManager.Remove(gallery.ContentItem);
            _mediaService.DeleteFolder(GetMediaPath(gallery.FolderName));
        }

        public void DeleteImage(GalleryImagePart image)
        {
            _contentManager.Remove(image.ContentItem);
        }

        public IEnumerable<GalleryPart> GetGalleries()
        {
            var galleries = _contentManager
                .Query<GalleryPart>(VersionOptions.Latest, "Gallery")
                .Join<GalleryPartRecord>()
                .List()
                .OrderBy(g => g.Order)
                .ThenByDescending(g => g.CreatedUtc)
                .ToList();

            foreach (var g in galleries)
            {
                g.ImagesCount = GetImagesCount(g.Id);
            }

            return galleries;
        }

        public GalleryPart GetGallery(int id)
        {
            var g = _contentManager.Get<GalleryPart>(id, VersionOptions.Latest);
            g.ImagesCount = GetImagesCount(id);
            return g;
        }

        public GalleryImagePart GetImage(int id)
        {
            return _contentManager.Get<GalleryImagePart>(id, VersionOptions.Latest);
        }
        
        private int GetImagesCount(int galleryId)
        {
            return _contentManager.Query("GalleryImage")
                      .Join<CommonPartRecord>()
                      .Where(cr => cr.Container.Id == galleryId)
                      .Count();
        }

        public IEnumerable<GalleryImagePart> GetImages(GalleryPart gallery)
        {
            return GetImages(gallery.Id);
        }

        public IEnumerable<GalleryImagePart> GetImages(int galleryId)
        {
            var images = _contentManager.Query("GalleryImage")
              .Join<CommonPartRecord>()
              .Where(cr => cr.Container.Id == galleryId)
              .List()
              .Select(i => i.As<GalleryImagePart>())
              .OrderBy(i => i.Order)
              .ToList();

            return images;
        }

        public IEnumerable<MediaFile> GetMediaFiles(GalleryPart gallery)
        {
            string path = GetMediaPath(gallery.FolderName);
            return _mediaService.GetMediaFiles(path);
        }

        public void AddImage(GalleryPart gallery, HttpPostedFileBase imageFile)
		{
			AddImage(gallery, imageFile.FileName, imageFile.InputStream);
		}

		public void AddImage(GalleryPart gallery, string fileName, Stream imageFile)
		{
            if (!IsFileAllowed(fileName, true))
            {
                throw new InvalidOperationException(string.Format("{0} is not a valid file.", fileName));
            }

            // Zip file processing is different from Media module since we want the folders structure to be flattened
            if (IsZipFile(Path.GetExtension(fileName)))
            {
                UnzipMediaFileArchive(gallery, imageFile);
            }
            else
            {
                string galleryFullPath = GetMediaPath(gallery.FolderName);
                var media = _mediaService.ImportMedia(imageFile, galleryFullPath, fileName, "GalleryImage");
                if (media != null && media.Has<GalleryImagePart>())
                {
                    var img = media.As<GalleryImagePart>();
                    img.GalleryPart = gallery;

                    _contentManager.Create(media);

                    img.Order = gallery.ImagesCount++;
                }
            }
        }

		private string GetMediaPath(string imageGalleryName)
		{
			return _storageProvider.Combine(ImageGalleriesMediaFolder, imageGalleryName);
		}

		private string GetName(string mediaPath)
		{
			return mediaPath.Split(new[] { '\\', '/' }).Last();
		}
        
		public string GetPublicUrl(string path)
		{
			return _mediaService.GetMediaPublicUrl(Path.GetDirectoryName(path), Path.GetFileName(path));
		}

		public bool IsFileAllowed(string fileName, bool allowZip) {
			return (IsImageFile(fileName) || (allowZip &&
			                                  IsZipFile(Path.GetExtension(fileName))));
		}

		public bool IsFileAllowed(HttpPostedFileBase postedFile)
		{
			if (postedFile == null)
			{
				return false;
			}

			return IsFileAllowed(postedFile.FileName, true);
		}

		private bool IsImageFile(string fileName)
		{
			string extension = Path.GetExtension(fileName);
			if (extension == null)
				return false;
			extension = extension.TrimStart('.');

			return _imageFileFormats.Any(o => extension.Equals(o, StringComparison.OrdinalIgnoreCase));
		}

		// TODO: Submit a path to Media module to make this method public?
		/// <summary>
		/// Determines if a file is a Zip Archive based on its extension.
		/// </summary>
		/// <param name="extension">The extension of the file to analyze.</param>
		/// <returns>True if the file is a Zip archive; false otherwise.</returns>
		private static bool IsZipFile(string extension)
		{
			return string.Equals(extension.TrimStart('.'), "zip", StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Unzips a media archive file flattening the folder structure.
		/// </summary>
		/// <param name="imageGallery">Image gallery name.</param>
		/// <param name="zipStream">The archive file stream.</param>
		protected void UnzipMediaFileArchive(GalleryPart gallery, Stream zipStream)
		{
			Argument.ThrowIfNull(zipStream, "zipStream");

			using (ZipInputStream fileInflater = new ZipInputStream(zipStream))
			{
				ZipEntry entry;

				while ((entry = fileInflater.GetNextEntry()) != null)
				{
					if (!entry.IsDirectory && !string.IsNullOrEmpty(entry.Name))
					{
						// Handle Mac OS X meta files
						if (entry.Name.StartsWith("__MACOSX", StringComparison.OrdinalIgnoreCase)) continue;
						// Skip disallowed files
						if (IsFileAllowed(entry.Name, false))
						{
							string fileName = Path.GetFileName(entry.Name);

							try
							{
								AddImage(gallery, fileName, fileInflater);
							}
							catch (ArgumentException argumentException)
							{
								if (argumentException.Message.Contains(fileName))
								{
									_services.Notifier.Warning(new LocalizedString(string.Format("File \"{0}\" skipped since it already exists.", fileName)));
								}
								else
								{
									throw;
								}
							}
						}
					}
				}
			}
		}

        public void CreateFolder(GalleryPart gallery)
        {
            string folderName = Slugify(string.IsNullOrWhiteSpace(gallery.FolderName) ?
                string.IsNullOrWhiteSpace(gallery.Title) ?
                ("Gallery " + gallery.Id) :
                gallery.Title :
                gallery.FolderName);

            if (!string.Equals(gallery.FolderName, folderName, StringComparison.OrdinalIgnoreCase))
                gallery.FolderName = folderName;

            _mediaService.CreateFolder(ImageGalleriesMediaFolder, folderName);
        }

        private string RemoveAccent(string txt)
        {
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }

        private string Slugify(string phrase)
        {
            string str = RemoveAccent(phrase).ToLower();
            str = System.Text.RegularExpressions.Regex.Replace(str, @"[^a-z0-9\s-]", ""); // Remove all non valid chars          
            str = System.Text.RegularExpressions.Regex.Replace(str, @"\s+", " ").Trim(); // convert multiple spaces into one space  
            str = System.Text.RegularExpressions.Regex.Replace(str, @"\s", "-"); // //Replace spaces by dashes
            return str;
        }


        public void SetAlbumCover(GalleryPart gallery, GalleryImagePart img)
        {
            if (img != null)
            {
                if (!img.IsCover.GetValueOrDefault())
                    img.IsCover = true;

                gallery.Images.Where(i => i.IsCover.GetValueOrDefault() && i.Id != img.Id)
                    .ToList()
                    .ForEach(i => i.IsCover = false);

                gallery.AlbumCoverUrl = img.PublicUrl;
            }
            else
            {
                gallery.AlbumCoverUrl = null;
            }
        }

        public void ReorderImages(int galleryId, IEnumerable<int> images)
        {
            var gallery = GetGallery(galleryId);

            int position = 0;

            foreach (int id in images)
            {
                var image = gallery.Images.FirstOrDefault(i => i.Id == id);
                if (image != null)
                {
                    image.Order = position++;
                }
                _contentManager.Publish(image.ContentItem);
            }
        }

        public void ReorderGalleries(IEnumerable<int> ids)
        {
            var galleries = GetGalleries();

            int position = 0;

            foreach (int id in ids)
            {
                var g = galleries.FirstOrDefault(i => i.Id == id);
                if (g != null)
                {
                    g.Order = position++;
                }
            }
        }
    }
}