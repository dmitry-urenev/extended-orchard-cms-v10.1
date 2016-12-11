using System;
using System.Collections.Generic;

using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Bootstrap.Slider.Models;
using Orchard.Bootstrap.Slider.ViewModels;
using Orchard.Bootstrap.Slider.Services;
using Orchard.Widgets.Models;

namespace Orchard.Bootstrap.Slider.Drivers
{
    
    public class SliderWidgetPartDriver : ContentPartDriver<SliderWidgetPart>
    {
        private ISliderService _sliderService;

        public SliderWidgetPartDriver(ISliderService sliderService)
        {
            _sliderService = sliderService;
        }

        protected override string Prefix
        {
            get { return "SliderWidgetPart"; }
        }

        protected override DriverResult Display(SliderWidgetPart part, string displayType, dynamic shapeHelper)
        {
            if (part.Slider == null)
                return null;

            var items = _sliderService.List(part.Slider, 0, part.Count, VersionOptions.Published);

            return Combined(
                ContentShape("Parts_Sliders_SliderWidget",
                () => shapeHelper.Parts_Sliders_SliderWidget(Slides: items)));
        }

        protected override DriverResult Editor(SliderWidgetPart widgetPart, dynamic shapeHelper)
        {
            var viewModel = new SliderWidgetViewModel()
            {
                Part = widgetPart,
                Sliders = _sliderService.Get()
            };

            var results = new List<DriverResult> {
                ContentShape("Parts_Sliders_SliderWidget_Fields",
                             () => shapeHelper.EditorTemplate(TemplateName: "Parts.Sliders.SliderWidget.Fields", Model: viewModel, Prefix: Prefix))
            };

            return Combined(results.ToArray());
        }

        protected override DriverResult Editor(SliderWidgetPart widgetPart, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(widgetPart, Prefix + ".Part", null, null);
            return Editor(widgetPart, shapeHelper);
        }
    }
}