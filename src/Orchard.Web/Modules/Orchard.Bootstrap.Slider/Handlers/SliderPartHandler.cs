using System.Web.Routing;

using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Bootstrap.Slider.Models;

namespace Orchard.Bootstrap.Slider.Handlers
{
    
    public class SliderPartHandler : ContentHandler {

        public SliderPartHandler()
        {
            OnGetDisplayShape<SliderPart>((context, slider) =>
            {
                //context.Shape.Description = slider.Description;
                //context.Shape.PostCount = slider.PostCount;
            });
        }

        protected override void GetItemMetadata(GetContentItemMetadataContext context) {
            var slider = context.ContentItem.As<SliderPart>();

            if (slider == null)
                return;

            context.Metadata.CreateRouteValues = new RouteValueDictionary {
                {"Area", "Orchard.Bootstrap.Slider"},
                {"Controller", "SliderAdmin"},
                {"Action", "Create"}
            };
            context.Metadata.EditorRouteValues = new RouteValueDictionary {
                {"Area", "Orchard.Bootstrap.Slider"},
                {"Controller", "SliderAdmin"},
                {"Action", "Edit"},
                {"sliderId", context.ContentItem.Id}
            };
            context.Metadata.RemoveRouteValues = new RouteValueDictionary {
                {"Area", "Orchard.Bootstrap.Slider"},
                {"Controller", "SliderAdmin"},
                {"Action", "Remove"},
                {"sliderId", context.ContentItem.Id}
            };
            context.Metadata.AdminRouteValues = new RouteValueDictionary {
                {"Area", "Orchard.Bootstrap.Slider"},
                {"Controller", "SliderAdmin"},
                {"Action", "Items"},
                {"sliderId", context.ContentItem.Id}
            };
        }
    }
}