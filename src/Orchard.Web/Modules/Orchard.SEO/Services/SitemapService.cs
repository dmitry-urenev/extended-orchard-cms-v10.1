using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.Caching;
using Orchard.Data;
using Orchard.Localization;
using Orchard.SEO.Models;
using Orchard.Environment.Extensions;

namespace Orchard.SEO.Services
{
    [OrchardFeature("Orchard.Sitemap")]
    public class SitemapService : ISitemapService
    {
        public const string DefaultFileText = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<urlset
      xmlns=""http://www.sitemaps.org/schemas/sitemap/0.9""
      xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
      xsi:schemaLocation=""http://www.sitemaps.org/schemas/sitemap/0.9
            http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd"">
<url>
  <loc>http://[your.site.com]/</loc>
  <priority>1.00</priority>
</url>
</urlset>";

        private readonly IRepository<SitemapFileRecord> _repository;
        private readonly ISignals _signals;

        public SitemapService(IRepository<SitemapFileRecord> repository, ISignals signals)
        {
            _repository = repository;
            _signals = signals;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public SitemapFileRecord Get()
        {
            var fileRecord = _repository.Table.FirstOrDefault();
            if (fileRecord == null)
            {
                fileRecord = new SitemapFileRecord()
                {
                    FileContent = DefaultFileText
                };
                _repository.Create(fileRecord);
            }
            return fileRecord;
        }

        public bool Save(string text)
        {
            var fileRecord = Get();
            fileRecord.FileContent = text;
            _signals.Trigger("SitemapFile.SettingsChanged");
            return true;
        }
    }
}