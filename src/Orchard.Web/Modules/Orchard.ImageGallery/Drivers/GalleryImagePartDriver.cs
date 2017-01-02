using System.Linq;
using Orchard.ImageGallery.Models;
using Orchard.ImageGallery.Services;
using Orchard.ImageGallery.ViewModels;
using Orchard;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement;
using Orchard.UI.Resources;
using System.Web.Mvc;
using System;
using System.Collections.Generic;

namespace Orchard.ImageGallery.Drivers
{
    public class GalleryImagePartDriver : ContentPartDriver<GalleryImagePart>
    {
        private readonly IImageGalleryService _imageGalleryService;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IThumbnailService _thumbnailService;

        public GalleryImagePartDriver(IImageGalleryService imageGalleryService, IThumbnailService thumbnailService, IWorkContextAccessor workContextAccessor)
        {
            _thumbnailService = thumbnailService;
            _workContextAccessor = workContextAccessor;
            _imageGalleryService = imageGalleryService;
        }

        protected override DriverResult Display(GalleryImagePart part, string displayType, dynamic shapeHelper)
        {
            if (string.Equals(displayType, "SummaryAdmin", StringComparison.OrdinalIgnoreCase) &&
                string.Equals(displayType, "Summary", StringComparison.OrdinalIgnoreCase))
            {
                // Image gallery returns nothing if in Summary Admin
                return null;
            }

            return null;

            //return ContentShape("Parts_ImageGallery",
            //                    () => shapeHelper.DisplayTemplate(
            //                        TemplateName: "Parts/ImageGallery",
            //                        Model: part,
            //                        Prefix: Prefix));
        }

        //GET
        protected override DriverResult Editor(GalleryImagePart part, dynamic shapeHelper)
        {
            return ContentShape("GalleryImage_DeleteButton", deleteButton => deleteButton);
        }
    }
}