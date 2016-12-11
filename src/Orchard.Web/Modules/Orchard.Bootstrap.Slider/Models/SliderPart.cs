using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using System.ComponentModel.DataAnnotations;

namespace Orchard.Bootstrap.Slider.Models
{
    public class SliderPart : ContentPart
    {
        public string Name
        {
            get { return this.As<ITitleAspect>().Title; }
        }
    }
}