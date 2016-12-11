
using Orchard.Bootstrap.Slider.Models;
using Orchard.Bootstrap.Slider.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Core.Feeds;
using System.Collections.Generic;

namespace Orchard.Bootstrap.Slider.Drivers
{
    
    public class SliderItemPartDriver : ContentPartDriver<SliderItemPart>
    {
        protected override DriverResult Display(SliderItemPart part, string displayType, dynamic shapeHelper)
        {
            return Combined(
              ContentShape("Parts_Sliders_SliderItem_SummaryAdmin",
                  () => shapeHelper.Parts_Sliders_SliderItem_SummaryAdmin()));
        }


        protected override DriverResult Editor(SliderItemPart sliderItemPart, dynamic shapeHelper)
        {
            var results = new List<DriverResult> {
                ContentShape("Parts_Sliders_SliderItem_Fields",
                             () => shapeHelper.EditorTemplate(TemplateName: "Parts.Sliders.SliderItem.Fields", Model: sliderItemPart, Prefix: Prefix))
            };

            //if (sliderItemPart.Id > 0)
            //    results.Add(ContentShape("Blog_DeleteButton",
            //        deleteButton => deleteButton));

            return Combined(results.ToArray());
        }

        protected override DriverResult Editor(SliderItemPart sliderItemPart, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(sliderItemPart, Prefix, null, null);
            return Editor(sliderItemPart, shapeHelper);
        }
    }
}