using Orchard.Bootstrap.Slider.Models;
using System.Collections.Generic;

namespace Orchard.Bootstrap.Slider.ViewModels
{
    public class SliderWidgetViewModel
    {
        public SliderWidgetPart Part { get; set; }
        public IEnumerable<SliderPart> Sliders { get; set; }

        //public int SelectedSlider { get; set; }
    }
}