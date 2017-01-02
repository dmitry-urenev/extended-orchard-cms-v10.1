using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Orchard.ImageGallery.Services;
using Orchard.ImageGallery.ViewModels;
using Orchard;
using Orchard.Core.Contents.Controllers;
using Orchard.Localization;
using Orchard.UI.Notify;
using Orchard.ImageGallery.Models;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.MediaLibrary.Services;
using System.Text.RegularExpressions;

namespace Orchard.ImageGallery.Controllers
{
    public class AdminController : Controller, IUpdateModel
    {
        private readonly IContentManager _contentManager;
        private readonly ITransactionManager _transactionManager;
        private readonly IImageGalleryService _imageGalleryService;
        private readonly IMediaLibraryService _mediaService;

        public AdminController(IOrchardServices services,
            IContentManager contentManager,
            ITransactionManager transactionManager,
            IMediaLibraryService mediaService,
            IImageGalleryService imageGalleryService)
        {
            _contentManager = contentManager;
            _transactionManager = transactionManager;
            _mediaService = mediaService;
            _imageGalleryService = imageGalleryService;

            Services = services;
            T = NullLocalizer.Instance;
        }

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        [HttpGet]
        public ViewResult List()
        {
            var galleries = _imageGalleryService.GetGalleries();
            return View(galleries);
        }

        [HttpGet]
        public ActionResult Create()
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageImageGallery, T("Couldn't create image gallery")))
                return new HttpUnauthorizedResult();

            GalleryPart gallery = _contentManager.New<GalleryPart>("Gallery");

            if (gallery == null)
                return HttpNotFound();
            
            var model = _contentManager.BuildEditor(gallery);
            return View(model);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult CreatePOST()
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageImageGallery, T("Couldn't create image gallery")))
                return new HttpUnauthorizedResult();

            GalleryPart gallery = _contentManager.New<GalleryPart>("Gallery");

            _contentManager.Create(gallery, VersionOptions.Draft);
            var model = _contentManager.UpdateEditor(gallery, this);

            if (!ModelState.IsValid)
            {
                gallery.ContentItem.Record.Id = 0;
                _transactionManager.Cancel();
                return View(model);
            }

            try
            {
                _imageGalleryService.CreateFolder(gallery);
            }
            catch (Exception exception)
            {
                _transactionManager.Cancel();
                Services.Notifier.Error(T("Creating image gallery failed: {0}", exception.Message));
                return View(model);
            }

            _contentManager.Publish(gallery.ContentItem);

            Services.Notifier.Information(T("Image gallery created"));

            return RedirectToAction("List");
        }

        [HttpGet]
        public ActionResult Images(int galleryId)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageImageGallery, T("Cannot edit image gallery")))
                return new HttpUnauthorizedResult();

            var gallery = _imageGalleryService.GetGallery(galleryId);
            if (gallery == null)
                return HttpNotFound();

            GalleryViewModel model = new GalleryViewModel(gallery);
            var mediaFiles = _imageGalleryService.GetMediaFiles(gallery);

            foreach (var imageModel in model)
            {
                var mf = mediaFiles.FirstOrDefault(f => f.Name == imageModel.Part.FileName);
                imageModel.File = mf;
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult EditProperties(int galleryId)
        {
            var gallery = _imageGalleryService.GetGallery(galleryId);
            if (gallery == null)
                return HttpNotFound();

            if (!Services.Authorizer.Authorize(Permissions.ManageImageGallery, T("Cannot edit image gallery")))
                return new HttpUnauthorizedResult();

            var galleryPart = gallery.ContentItem.As<GalleryPart>();
            var model = Services.ContentManager.BuildEditor(galleryPart);

            return View(model);
        }

        [HttpPost, ActionName("EditProperties")]
        [Orchard.Mvc.FormValueRequired("submit.Save")]
        public ActionResult EditPropertiesPOST(int galleryId)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageImageGallery, T("Cannot edit image gallery")))
                return new HttpUnauthorizedResult();

            var gallery = _imageGalleryService.GetGallery(galleryId);
            if (gallery == null)
                return RedirectToAction("List");

            var galleryPart = gallery.ContentItem.As<GalleryPart>();
            var model = _contentManager.UpdateEditor(galleryPart, this);

            if (!ModelState.IsValid)
            {
                _transactionManager.Cancel();
                return View(model);
            }
            _contentManager.Publish(galleryPart.ContentItem);

            Services.Notifier.Information(T("Image gallery updated"));

            return RedirectToAction("Images", new { galleryId = gallery.Id });
        }

        [HttpGet]
        public ActionResult AddImages(int galleryId)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageImageGallery, T("Cannot add images to image gallery")))
                return new HttpUnauthorizedResult();

            var gallery = _imageGalleryService.GetGallery(galleryId);
            if (gallery == null)
                return HttpNotFound();

            return View(new ImageAddViewModel
            {
                AllowedFiles = _imageGalleryService.AllowedFileFormats,
                GalleryId = galleryId
            });
        }

        [HttpPost]
        public ActionResult AddImages(ImageAddViewModel viewModel)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageImageGallery, T("Couldn't upload media file")))
            {
                return new HttpUnauthorizedResult();
            }
            viewModel.AllowedFiles = _imageGalleryService.AllowedFileFormats;

            if (!ModelState.IsValid)
                return View(viewModel);

            var gallery = _imageGalleryService.GetGallery(viewModel.GalleryId);
            if (gallery == null)
                return RedirectToAction("List");

            try
            {
                if (viewModel.ImageFiles == null || viewModel.ImageFiles.Count() == 0 || viewModel.ImageFiles.First() == null)
                {
                    ModelState.AddModelError("File", T("Select a file to upload").ToString());
                    return View(viewModel);
                }

                if (viewModel.ImageFiles.Any(file => !_imageGalleryService.IsFileAllowed(file)))
                {
                    ModelState.AddModelError("File", T("That file type is not allowed.").ToString());
                    return View(viewModel);
                }

                foreach (var file in viewModel.ImageFiles)
                {
                    _imageGalleryService.AddImage(gallery, file);
                }
            }
            catch (Exception exception)
            {
                Services.Notifier.Error(T("Adding image failed: {0}", exception.Message));
                return View(viewModel);
            }

            return RedirectToAction("Images", new { galleryId = viewModel.GalleryId });
        }

        [HttpPost]
        [Orchard.Mvc.FormValueRequired("submit.Delete")]
        [ActionName("EditProperties")]
        public ActionResult Delete(int galleryId)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageImageGallery, T("Cannot delete image gallery")))
                return new HttpUnauthorizedResult();

            var gallery = _imageGalleryService.GetGallery(galleryId);
            if (gallery == null)
                return RedirectToAction("List");

            try
            {
                _imageGalleryService.DeleteGallery(gallery);
                Services.Notifier.Information(T(string.Format("Image gallery \"{0}\" was deleted", gallery.Title)));
            }
            catch (Exception exception)
            {
                Services.Notifier.Error(T("Deleting image gallery failed: {0}", exception.Message));
            }

            return RedirectToAction("List");
        }

        [HttpPost]
        public ActionResult DeleteImage(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageImageGallery, T("Cannot delete images")))
                return new HttpUnauthorizedResult();

            var image = _imageGalleryService.GetImage(id);
            if (image == null)
            {
                Services.Notifier.Error(T("Image not found"));
                return RedirectToAction("List");
            }
            var gallery = image.GalleryPart;
            try
            {
                _imageGalleryService.DeleteImage(image);

                Services.Notifier.Information(T("Image was successfully deleted"));
            }
            catch (Exception e)
            {
                Services.Notifier.Error(T(e.Message));
            }

            return RedirectToAction("Images", new { galleryId = gallery.Id });
        }

        [ActionName("Images"), HttpPost]
        [Orchard.Mvc.FormValueRequired("submit.DeleteSelectedImages")]
        public ActionResult DeleteSelectedImages(string galleryId, IEnumerable<int> ids)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageImageGallery, T("Cannot delete images")))
                return new HttpUnauthorizedResult();

            try
            {
                foreach (var id in ids)
                {
                    var image = _imageGalleryService.GetImage(id);
                    if (image != null)
                    {
                        _imageGalleryService.DeleteImage(image);
                    }
                }
                Services.Notifier.Information(T("Images were successfully deleted"));
            }
            catch (Exception exception)
            {
                Services.Notifier.Error(T("Deleting image failed: {0}", exception.Message));
            }

            return RedirectToAction("Images", new { galleryId });
        }

        public JsonResult ReorderImages(int galleryId, IEnumerable<int> images)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageImageGallery, T("Cannot modify gallery")))
                return Json(new HttpUnauthorizedResult());

            _imageGalleryService.ReorderImages(galleryId, images);

            return new JsonResult();
        }

        public JsonResult ReorderGalleries(IEnumerable<int> galleries)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageImageGallery, T("Cannot modify galleries")))
                return Json(new HttpUnauthorizedResult());

            _imageGalleryService.ReorderGalleries(galleries);

            return new JsonResult();
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties)
        {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage)
        {
            ModelState.AddModelError(key, errorMessage.ToString());
        }
    }
}