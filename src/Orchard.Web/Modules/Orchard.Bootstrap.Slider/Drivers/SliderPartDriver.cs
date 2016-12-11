using System;
using System.Collections.Generic;

using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Bootstrap.Slider.Models;

namespace Orchard.Bootstrap.Slider.Drivers
{
    
    public class SliderPartDriver : ContentPartDriver<SliderPart>
    {
        protected override string Prefix
        {
            get { return "SliderPart"; }
        }

        protected override DriverResult Display(SliderPart part, string displayType, dynamic shapeHelper)
        {
            return Combined(
                ContentShape("Parts_Sliders_Slider_Manage",
                    () => shapeHelper.Parts_Sliders_Slider_Manage()),

                ContentShape("Parts_Sliders_Slider_SummaryAdmin",
                    () => shapeHelper.Parts_Sliders_Slider_SummaryAdmin())
                );
        }

        protected override DriverResult Editor(SliderPart sliderPart, dynamic shapeHelper)
        {
            var results = new List<DriverResult> {
                ContentShape("Parts_Sliders_Slider_Fields",
                             () => shapeHelper.EditorTemplate(TemplateName: "Parts.Sliders.Slider.Fields", Model: sliderPart, Prefix: Prefix))
            };

            if (sliderPart.Id > 0)
                results.Add(ContentShape("Slider_DeleteButton",
                    deleteButton => deleteButton));

            return Combined(results.ToArray());
        }

        protected override DriverResult Editor(SliderPart sliderPart, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(sliderPart, Prefix, null, null);
            return Editor(sliderPart, shapeHelper);
        }
    }
}