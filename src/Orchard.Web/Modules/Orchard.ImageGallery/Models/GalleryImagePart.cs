using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.Fields.Fields;
using Orchard.MediaLibrary.Models;
using Orchard.Security;
using System;

namespace Orchard.ImageGallery.Models
{
    public class GalleryImagePart : ContentPart
    {
        public GalleryPart GalleryPart
        {
            get { return this.As<ICommonPart>().Container.As<GalleryPart>(); }
            set { this.As<ICommonPart>().Container = value; }
        }

        public string Title
        {
            get { return this.As<MediaPart>().Title; }
        }

        public string FileName
        {
            get { return this.As<MediaPart>().FileName; }
        }

        public string PublicUrl
        {
            get { return this.As<MediaPart>().MediaUrl; }
        }

        public string AlternateText
        {
            get { return this.As<MediaPart>().AlternateText; }
        }

        public string Caption
        {
            get { return this.As<MediaPart>().Caption; }
        }

        public int Width
        {
            get { return this.As<ImagePart>().Width; }
        }

        public int Height
        {
            get { return this.As<ImagePart>().Height; }
        }

        public int? Order
        {
            get
            {
                var field = this.Get(typeof(NumericField), "Order") as NumericField;
                if (field != null)
                {
                    return Convert.ToInt32(field.Value ?? 0);
                }
                return default(int?);
            }
            set
            {
                var field = this.Get(typeof(NumericField), "Order") as NumericField;
                if (field != null)
                {
                    field.Value = value;
                }
            }
        }

        public bool? IsCover
        {
            get
            {
                var field = this.Get(typeof(BooleanField), "IsCover") as BooleanField;
                return field != null ? field.Value : default(bool?);
            }
            set
            {
                var field = this.Get(typeof(BooleanField), "IsCover") as BooleanField;
                if (field != null)
                {
                    field.Value = value;
                }
            }
        }

        public DateTime? ModifiedUtc
        {
            get { return this.As<ICommonPart>().ModifiedUtc; }
        }

        public IUser Creator
        {
            get { return this.As<ICommonPart>().Owner; }
            set { this.As<ICommonPart>().Owner = value; }
        }
    }
}