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
    public class GalleryPartDriver : ContentPartDriver<GalleryPart>
    {
        private readonly IImageGalleryService _imageGalleryService;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IThumbnailService _thumbnailService;

        public GalleryPartDriver(IImageGalleryService imageGalleryService, IThumbnailService thumbnailService, IWorkContextAccessor workContextAccessor)
        {
            _thumbnailService = thumbnailService;
            _workContextAccessor = workContextAccessor;
            _imageGalleryService = imageGalleryService;
        }

        protected override DriverResult Display(GalleryPart part, string displayType, dynamic shapeHelper)
        {
            if (string.Equals(displayType, "SummaryAdmin", StringComparison.OrdinalIgnoreCase) &&
                string.Equals(displayType, "Summary", StringComparison.OrdinalIgnoreCase))
            {
                // Image gallery returns nothing if in Summary Admin
                return null;
            }

            return ContentShape("Parts_ImageGallery",
                                () => shapeHelper.DisplayTemplate(
                                    TemplateName: "Parts/ImageGallery",
                                    Model: part,
                                    Prefix: Prefix));
        }

        //GET
        protected override DriverResult Editor(GalleryPart part, dynamic shapeHelper)
        {
            var results = new List<DriverResult> {
                ContentShape("Parts_GalleryPart_Fields",
                                () => shapeHelper.EditorTemplate(
                                    TemplateName: "Parts.GalleryPart.Fields",
                                    Model: part,
                                    Prefix: Prefix))
            };

            if (part.Id > 0)
                results.Add(ContentShape("Gallery_DeleteButton",
                    deleteButton => deleteButton));

            return Combined(results.ToArray());
        }

        //POST
        protected override DriverResult Editor(GalleryPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }
    }
}