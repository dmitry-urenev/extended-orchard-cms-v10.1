using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Orchard.ImageGallery.ViewModels;
using Orchard.ContentManagement;
using System.ComponentModel.DataAnnotations;
using Orchard.Core.Title.Models;
using Orchard.Fields.Fields;
using System;
using Orchard.Security;
using Orchard.ContentManagement.Aspects;
using Orchard.ContentManagement.Utilities;

namespace Orchard.ImageGallery.Models
{
    public class GalleryPart : ContentPart<GalleryPartRecord>
    {
        public string Title
        {
            get { return this.As<TitlePart>().Title; }
            set { this.As<TitlePart>().Title = value; }
        }

        public string FolderName
        {
            get { return Record.FolderName; }
            set { Record.FolderName = value; }
        }

        public bool IsEnabled
        {
            get { return Record.IsEnabled; }
            set { Record.IsEnabled = value; }
        }

        public bool KeepAspectRatio
        {
            get { return Record.KeepAspectRatio; }
            set { Record.KeepAspectRatio = value; }
        }

        public int ThumbnailWidth
        {
            get { return Record.ThumbnailWidth; }
            set { Record.ThumbnailWidth = value; }
        }

        public int ThumbnailHeight
        {
            get { return Record.ThumbnailHeight; }
            set { Record.ThumbnailHeight = value; }
        }

        public int Order
        {
            get { return Record.OrderIdx; }
            set { Record.OrderIdx = value; }
        }

        public DateTime? ModifiedUtc
        {
            get { return this.As<ICommonPart>().ModifiedUtc; }
        }

        public DateTime? CreatedUtc
        {
            get { return this.As<ICommonPart>().CreatedUtc; }
        }

        public IUser Creator
        {
            get { return this.As<ICommonPart>().Owner; }
            set { this.As<ICommonPart>().Owner = value; }
        }

        public int ImagesCount { get; set; }

        internal LazyField<IEnumerable<GalleryImagePart>> ImagesField = new LazyField<IEnumerable<GalleryImagePart>>();

        public IEnumerable<GalleryImagePart> Images
        {
            get { return ImagesField.Value; }
        }

        public string AlbumCoverUrl
        {
            get { return Record.AlbumCoverUrl; }
            set { Record.AlbumCoverUrl = value; }
        }
    }
}