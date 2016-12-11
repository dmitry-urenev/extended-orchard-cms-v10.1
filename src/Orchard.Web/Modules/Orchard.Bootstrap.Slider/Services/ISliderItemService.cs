using System;
using System.Collections.Generic;
using Orchard.Bootstrap.Slider.Models;
using Orchard.ContentManagement;

namespace Orchard.Bootstrap.Slider.Services
{
    public interface ISliderItemService : IDependency
    {
        SliderItemPart Get(int id);
        SliderItemPart Get(int id, VersionOptions versionOptions);
        IEnumerable<SliderItemPart> Get(SliderPart sliderPart);
        IEnumerable<SliderItemPart> Get(SliderPart sliderPart, VersionOptions versionOptions);
        IEnumerable<SliderItemPart> Get(SliderPart sliderPart, int skip, int count);
        IEnumerable<SliderItemPart> Get(SliderPart sliderPart, int skip, int count, VersionOptions versionOptions);
        int ItemsCount(SliderPart sliderPart);
        int ItemsCount(SliderPart sliderPart, VersionOptions versionOptions);
        void Delete(SliderItemPart sliderItemPart);
        void Publish(SliderItemPart sliderItemPart);
        void Publish(SliderItemPart sliderItemPart, DateTime scheduledPublishUtc);
        void Unpublish(SliderItemPart sliderItemPart);
        DateTime? GetScheduledPublishUtc(SliderItemPart sliderItemPart);
    }
}