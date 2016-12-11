using System;
using System.Collections.Generic;
using System.Linq;

using Orchard.Bootstrap.Slider.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Common.Models;
using Orchard.Data;
using Orchard.Tasks.Scheduling;

namespace Orchard.Bootstrap.Slider.Services
{
    
    public class SliderItemService : ISliderItemService
    {
        private readonly IContentManager _contentManager;
        private readonly IPublishingTaskManager _publishingTaskManager;

        public SliderItemService(
            IContentManager contentManager,
            IPublishingTaskManager publishingTaskManager,
            IContentDefinitionManager contentDefinitionManager)
        {
            _contentManager = contentManager;
            _publishingTaskManager = publishingTaskManager;
        }

        public SliderItemPart Get(int id)
        {
            return Get(id, VersionOptions.Published);
        }

        public SliderItemPart Get(int id, VersionOptions versionOptions)
        {
            return _contentManager.Get<SliderItemPart>(id, versionOptions);
        }

        public IEnumerable<SliderItemPart> Get(SliderPart sliderPart)
        {
            return Get(sliderPart, VersionOptions.Published);
        }

        public IEnumerable<SliderItemPart> Get(SliderPart sliderPart, VersionOptions versionOptions)
        {
            return GetSliderItemQuery(sliderPart, versionOptions).List().Select(ci => ci.As<SliderItemPart>());
        }

        public IEnumerable<SliderItemPart> Get(SliderPart sliderPart, int skip, int count)
        {
            return Get(sliderPart, skip, count, VersionOptions.Published);
        }

        public IEnumerable<SliderItemPart> Get(SliderPart sliderPart, int skip, int count, VersionOptions versionOptions)
        {
            return GetSliderItemQuery(sliderPart, versionOptions)
                    .Slice(skip, count)
                    .ToList()
                    .Select(ci => ci.As<SliderItemPart>());
        }

        public int ItemsCount(SliderPart sliderPart)
        {
            return ItemsCount(sliderPart, VersionOptions.Published);
        }

        public int ItemsCount(SliderPart sliderPart, VersionOptions versionOptions)
        {
            return _contentManager.Query(versionOptions, "SliderItem")
                .Join<CommonPartRecord>().Where(
                    cr => cr.Container.Id == sliderPart.Id)
                .Count();
        }

        public void Delete(SliderItemPart sliderItemPart)
        {
            _publishingTaskManager.DeleteTasks(sliderItemPart.ContentItem);
            _contentManager.Remove(sliderItemPart.ContentItem);
        }

        public void Publish(SliderItemPart sliderItemPart)
        {
            _publishingTaskManager.DeleteTasks(sliderItemPart.ContentItem);
            _contentManager.Publish(sliderItemPart.ContentItem);
        }

        public void Publish(SliderItemPart sliderItemPart, DateTime scheduledPublishUtc)
        {
            _publishingTaskManager.Publish(sliderItemPart.ContentItem, scheduledPublishUtc);
        }

        public void Unpublish(SliderItemPart sliderItemPart)
        {
            _contentManager.Unpublish(sliderItemPart.ContentItem);
        }

        public DateTime? GetScheduledPublishUtc(SliderItemPart sliderItemPart)
        {
            var task = _publishingTaskManager.GetPublishTask(sliderItemPart.ContentItem);
            return (task == null ? null : task.ScheduledUtc);
        }

        private IContentQuery<ContentItem, CommonPartRecord> GetSliderItemQuery(SliderPart slider, VersionOptions versionOptions)
        {
            return
                _contentManager.Query(versionOptions, "SliderItem")
                .Join<CommonPartRecord>().Where(
                    cr => cr.Container.Id == slider.Id).OrderByDescending(cr => cr.CreatedUtc)
                    ;
        }
    }
}