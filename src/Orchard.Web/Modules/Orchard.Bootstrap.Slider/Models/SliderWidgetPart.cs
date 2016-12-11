using Orchard.ContentManagement;
using System.ComponentModel.DataAnnotations;

namespace Orchard.Bootstrap.Slider.Models
{
    public class SliderWidgetPart : ContentPart
    {
        public int Count
        {
            get
            {
                var c = this.Retrieve(x => x.Count);
                if (c == 0)
                    return 3;

                return c;
            }
            set { this.Store(x => x.Count, value); }
        }

        public int Interval
        {
            get
            {
                var interval = this.Retrieve(x => x.Interval);
                if (interval == 0)
                    return 5000;

                return interval;
            }
            set { this.Store(x => x.Interval, value); }
        }

        public AnimationType AnimationType
        {
            get { return this.Retrieve(x => x.AnimationType); }
            set { this.Store(x => x.AnimationType, value); }
        }

        [Required]
        public int? SliderId
        {
            get { return this.Retrieve(x => x.SliderId); }
            set { this.Store(x => x.SliderId, value); }
        }

        public SliderPart Slider { get; set; }
    }

    public enum AnimationType { Slide, Fade }
}