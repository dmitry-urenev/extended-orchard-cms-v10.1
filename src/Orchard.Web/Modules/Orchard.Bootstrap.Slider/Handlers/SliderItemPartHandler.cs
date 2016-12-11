using System.Linq;
using System.Web.Routing;

using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Core.Common.Models;
using Orchard.Bootstrap.Slider.Models;
using Orchard.Bootstrap.Slider.Services;

namespace Orchard.Bootstrap.Slider.Handlers
{
    
    public class SliderItemPartHandler : ContentHandler
    {
        private readonly ISliderService _sliderService;

        public SliderItemPartHandler(ISliderService sliderService, RequestContext requestContext)
        {
            _sliderService = sliderService;

            OnGetDisplayShape<SliderItemPart>(SetModelProperties);
            OnGetEditorShape<SliderItemPart>(SetModelProperties);
            OnUpdateEditorShape<SliderItemPart>(SetModelProperties);

            OnRemoved<SliderPart>(
                (context, b) =>
                _sliderService.List(context.ContentItem.As<SliderPart>()).ToList().ForEach(
                    sliderItem => context.ContentManager.Remove(sliderItem.ContentItem)));
        }

        private static void SetModelProperties(BuildShapeContext context, SliderItemPart sliderItem)
        {
            context.Shape.Slider = sliderItem.SliderPart;
        }

        protected override void GetItemMetadata(GetContentItemMetadataContext context)
        {
            var sliderItem = context.ContentItem.As<SliderItemPart>();

            if (sliderItem == null)
                return;

            context.Metadata.CreateRouteValues = new RouteValueDictionary {
                {"Area", "Orchard.Bootstrap.Slider"},
                {"Controller", "SliderItemAdmin"},
                {"Action", "Create"},
                {"sliderId", sliderItem.SliderPart.Id}
            };
            context.Metadata.EditorRouteValues = new RouteValueDictionary {
                {"Area", "Orchard.Bootstrap.Slider"},
                {"Controller", "SliderItemAdmin"},
                {"Action", "Edit"},
                {"itemId", context.ContentItem.Id},
                {"sliderId", sliderItem.SliderPart.Id}
            };
            context.Metadata.RemoveRouteValues = new RouteValueDictionary {
                {"Area", "Orchard.Bootstrap.Slider"},
                {"Controller", "SliderItemAdmin"},
                {"Action", "Delete"},
                {"itemId", context.ContentItem.Id},
                {"sliderId", sliderItem.SliderPart.Id}
            };
        }
    }
}