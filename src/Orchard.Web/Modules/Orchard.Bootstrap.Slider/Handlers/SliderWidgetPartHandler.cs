using System.Linq;
using System.Web.Routing;

using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Core.Common.Models;
using Orchard.Bootstrap.Slider.Models;
using Orchard.Bootstrap.Slider.Services;

namespace Orchard.Bootstrap.Slider.Handlers
{
    
    public class SliderWidgetPartHandler : ContentHandler
    {
        private readonly ISliderService _sliderService;

        public SliderWidgetPartHandler(ISliderService sliderService, RequestContext requestContext)
        {
            _sliderService = sliderService;

            OnLoaded<SliderWidgetPart>((c, p) => SetModelProperties(p));
        }

        private void SetModelProperties(SliderWidgetPart widget)
        {
            if (widget.SliderId.HasValue)
            {
                widget.Slider = _sliderService.Get(widget.SliderId.GetValueOrDefault(), VersionOptions.Latest);
            }
        }
    }
}