using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.Core.Common.Models;
using Orchard.Core.Title.Models;

namespace Orchard.Bootstrap.Slider.Models
{
    public class SliderItemPart : ContentPart
    {
        public string Title
        {
            get { return this.As<TitlePart>().Title; }
            set { this.As<TitlePart>().Title = value; }
        }

        public string Url
        {
            get { return this.Retrieve(x => x.Url); }
            set { this.Store(x => x.Url, value); }
        }

        public string Caption
        {
            get { return this.As<BodyPart>().Text; }
            set { this.As<BodyPart>().Text = value; }
        }

        public int Order
        {
            get { return this.Retrieve(x => x.Order); }
            set { this.Store(x => x.Order, value); }
        }

        public SliderPart SliderPart
        {
            get { return this.As<ICommonPart>().Container.As<SliderPart>(); }
            set { this.As<ICommonPart>().Container = value; }
        }
    }
}