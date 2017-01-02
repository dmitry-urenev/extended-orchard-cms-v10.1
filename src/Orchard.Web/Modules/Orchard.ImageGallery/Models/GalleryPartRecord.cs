using Orchard.ContentManagement.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.ImageGallery.Models
{
    public class GalleryPartRecord : ContentPartRecord
    {
        public GalleryPartRecord()
        {
            IsEnabled = true;
            ThumbnailHeight = 200;
            ThumbnailWidth = 250;
            KeepAspectRatio = true;
        }

        public virtual string FolderName { get; set; }

        public virtual bool IsEnabled { get; set; }

        public virtual int ThumbnailWidth { get; set; }

        public virtual int ThumbnailHeight { get; set; }

        public virtual bool KeepAspectRatio { get; set; }

        public virtual int OrderIdx { get; set; }

        public virtual string AlbumCoverUrl { get; set; }
    }
}
