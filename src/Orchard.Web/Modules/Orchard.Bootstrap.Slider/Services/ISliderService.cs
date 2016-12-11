using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.Bootstrap.Slider.Models;

namespace Orchard.Bootstrap.Slider.Services
{
    public interface ISliderService : IDependency
    {
        SliderPart Get(int id, VersionOptions versionOptions);

        IEnumerable<SliderPart> Get();

        IEnumerable<SliderPart> Get(VersionOptions versionOptions);

        IEnumerable<SliderItemPart> List(SliderPart sliderPart, int skip, int count, VersionOptions versionOptions);

        IEnumerable<SliderItemPart> List(SliderPart sliderPart, string lang, int skip, int count, VersionOptions versionOptions);

        IEnumerable<SliderItemPart> List(SliderPart sliderPart);

        int ItemsCount(SliderPart sliderPart, VersionOptions versionOptions);

        int ItemsCount(SliderPart sliderPart, string lang, VersionOptions versionOptions);

    }
}