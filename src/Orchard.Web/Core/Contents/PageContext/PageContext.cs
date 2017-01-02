using Orchard.Caching;
using Orchard.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.Core.Contents.PageContext
{
    public class PageContext
    {
        private readonly ISignals _signals;

        public PageContext(ISignals signals)
        {
            _signals = signals;
        }

        private ContentItem _contentItem;
        public ContentItem ContentItem
        {
            get
            {
                return _contentItem;
            }
            set
            {
                if (_contentItem != value)
                {
                    _contentItem = value;
                    OnContentItemChanged();
                }
            }
        }

        private void OnContentItemChanged()
        {
            _signals.Trigger(Localization.Signals.CurrentCultureChanged);
        }
    }
}