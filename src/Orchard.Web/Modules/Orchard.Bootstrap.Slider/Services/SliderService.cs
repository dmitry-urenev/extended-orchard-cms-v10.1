using Orchard.Bootstrap.Slider.Models;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Core.Title.Models;
using Orchard.Localization.Models;
using Orchard.Localization.Services;
using Orchard.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Orchard.Bootstrap.Slider.Services
{
    public class SliderService : ISliderService
    {
        private IContentManager _contentManager;
        private ICultureManager _cultureManager;
        private IHttpContextAccessor _contextAccessor;

        public SliderService(IContentManager contentManager,
            ICultureManager cultureManager,
            IHttpContextAccessor contextAccessor)
        {
            _contentManager = contentManager;
            _cultureManager = cultureManager;
            _contextAccessor = contextAccessor;
        }
        
        public SliderPart Get(int id, VersionOptions versionOptions)
        {
            return _contentManager.Get(id, versionOptions).As<SliderPart>();
        }
        
        public IEnumerable<SliderPart> Get()
        {
            return Get(VersionOptions.Published);
        }

        public IEnumerable<SliderPart> Get(VersionOptions versionOptions)
        {
            return _contentManager.Query<SliderPart>(versionOptions, "Slider")
                .Join<TitlePartRecord>()
                .OrderBy(br => br.Title)
                .List();
        }


        /// <summary>
        /// Returns slider items for specified slider in current Culture
        /// </summary>
        /// <param name="sliderPart"></param>
        /// <param name="skip"></param>
        /// <param name="count"></param>
        /// <param name="versionOptions"></param>
        /// <returns></returns>
        public IEnumerable<SliderItemPart> List(SliderPart sliderPart, int skip, int count, VersionOptions versionOptions)
        {
            string lang = _cultureManager.GetCurrentCulture(_contextAccessor.Current());
            return List(sliderPart, lang, skip, count, versionOptions);
        }


        /// <summary>
        /// Returns slider items for specified slider and specified Culture
        /// </summary>
        /// <param name="sliderPart"></param>
        /// <param name="lang"></param>
        /// <param name="skip"></param>
        /// <param name="count"></param>
        /// <param name="versionOptions"></param>
        /// <returns></returns>
        public IEnumerable<SliderItemPart> List(SliderPart sliderPart, string lang, int skip, int count, VersionOptions versionOptions)
        {
            return GetSliderQuery(sliderPart, versionOptions)
                    .List()
                    .Where(r => !r.Has<LocalizationPart>() || r.As<LocalizationPart>().Culture == null ||
                         string.Equals(r.As<LocalizationPart>().Culture.Culture, lang, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(ci => ci.As<SliderItemPart>().Order)
                    .Skip(skip)
                    .Take(count)
                    .Select(ci => ci.As<SliderItemPart>())
                    .ToList();
        }

        /// <summary>
        /// Returns all slider items for specified slider.
        /// </summary>
        /// <param name="sliderPart"></param>
        /// <returns></returns>
        public IEnumerable<SliderItemPart> List(SliderPart sliderPart)
        {
            return GetSliderQuery(sliderPart, VersionOptions.Published)
                    .List()
                    .Select(ci => ci.As<SliderItemPart>());
        }

        private IContentQuery<ContentItem, CommonPartRecord> GetSliderQuery(SliderPart slider, VersionOptions versionOptions)
        {
            return
                _contentManager.Query(versionOptions, "SliderItem")
                .Join<CommonPartRecord>()
                    .Where<CommonPartRecord>(cr => cr.Container.Id == slider.Id)
                    .WithQueryHints(new QueryHints().ExpandParts<SliderItemPart, LocalizationPart>())
                ;
        }

        public int ItemsCount(SliderPart sliderPart, string lang, VersionOptions versionOptions)
        {
            return _contentManager.Query(versionOptions, "SliderItem")
                   .Join<CommonPartRecord>()
                   .Where(cr => cr.Container.Id == sliderPart.Id)
                   .WithQueryHints(new QueryHints().ExpandParts<LocalizationPart>())
                   .List()
                   .Where(r => !r.Has<LocalizationPart>() || r.As<LocalizationPart>().Culture == null ||
                         string.Equals(r.As<LocalizationPart>().Culture.Culture, lang, StringComparison.OrdinalIgnoreCase))
                   .Count();
        }

        public int ItemsCount(SliderPart sliderPart, VersionOptions versionOptions)
        {
            string lang = _cultureManager.GetCurrentCulture(_contextAccessor.Current());
            return ItemsCount(sliderPart, lang, versionOptions);
        }
       
    }
}