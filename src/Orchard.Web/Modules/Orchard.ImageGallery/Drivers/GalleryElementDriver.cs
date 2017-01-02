using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.Layouts.Framework.Display;
using Orchard.Layouts.Framework.Drivers;
using Orchard.ImageGallery.Elements;
using Orchard.ImageGallery.Services;
using Orchard.ImageGallery.ViewModels;

namespace Orchard.ImageGallery.Drivers
{
    public class GalleryElementDriver : ElementDriver<GalleryElement>
    {
        private readonly IImageGalleryService _imageGalleryService;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IThumbnailService _thumbnailService;

        public GalleryElementDriver(IImageGalleryService imageGalleryService, IThumbnailService thumbnailService, IWorkContextAccessor workContextAccessor)
        {
            _imageGalleryService = imageGalleryService;
            _thumbnailService = thumbnailService;
            _workContextAccessor = workContextAccessor;
        }

        protected override EditorResult OnBuildEditor(GalleryElement element, ElementEditorContext context)
        {
            var viewModel = new GalleryElementEditorViewModel
            {
                AvailableGalleries = _imageGalleryService.GetGalleries()
            };

            var editor = context.ShapeFactory.EditorTemplate(TemplateName: "GalleryElementEditor", Model: viewModel);

            if (context.Updater != null)
            {
                context.Updater.TryUpdateModel(viewModel, context.Prefix, null, null);
                element.GalleryId = viewModel.SelectedGallery;
            }

            return Editor(context, editor);
        }

        protected override void OnDisplaying(GalleryElement element, ElementDisplayingContext context)
        {
            var gallery = element.GalleryId.HasValue ? _imageGalleryService.GetGallery(element.GalleryId ?? 0) : null;

            context.ElementShape.ImageGallery = gallery;
            context.ElementShape.IsGalleryAvalable = gallery != null && gallery.IsEnabled; // TODO: Add gallerty.IsEnabled check
        }
    }
}
