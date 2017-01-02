using System.Data;
using Orchard.ImageGallery.Models;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;

namespace Orchard.ImageGallery {
    public class Migrations : DataMigrationImpl {
        public int Create()
        {
            SchemaBuilder.CreateTable("GalleryPartRecord", table => table
                .ContentPartRecord()
                .Column<string>("FolderName")
                .Column<bool>("IsEnabled")
                .Column<int>("ThumbnailWidth")
                .Column<int>("ThumbnailHeight")
                .Column<bool>("KeepAspectRatio")
                .Column<string>("AlbumCoverUrl")
                .Column<int>("OrderIdx")
            );

            ContentDefinitionManager.AlterPartDefinition("GalleryPart", cfg => cfg
                    .WithDescription("Turns content types into a image gallery")
            );

            ContentDefinitionManager.AlterTypeDefinition("Gallery", cfg => cfg
                    .WithPart("GalleryPart")
                    .WithPart("CommonPart")
                    .WithPart("TitlePart")
            );

            ContentDefinitionManager.AlterPartDefinition("GalleryImagePart", cfg => cfg
                    .WithField("Order", f => f.OfType("NumericField").WithDisplayName("Order"))
                    .WithField("IsCover", f => f.OfType("BooleanField").WithDisplayName("Is gallery cover"))
            );

            ContentDefinitionManager.AlterTypeDefinition("GalleryImage", td => td
                    .DisplayedAs("Gallery Image")
                    .WithSetting("MediaFileNameEditorSettings.ShowFileNameEditor", "True")
                    .AsImage()
                    .WithIdentity()
                    .WithPart("GalleryImagePart")
            );

            return 1;
        }
    }
}