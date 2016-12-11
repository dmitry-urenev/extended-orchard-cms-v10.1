using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using System.Data;

namespace Orchard.Bootstrap.Slider
{
    public class Migrations : DataMigrationImpl
    {

        public int Create()
        {
            ContentDefinitionManager.AlterPartDefinition("SliderPart", builder => builder
                .WithDescription("Turns content types into a Slider."));

            ContentDefinitionManager.AlterTypeDefinition("Slider",
                cfg => cfg
                    .WithPart("SliderPart")
                    .WithPart("CommonPart")
                    .WithPart("TitlePart")
                );

            ContentDefinitionManager.AlterPartDefinition("SliderItemPart", builder => builder
                .WithDescription("Turns content types into a SliderItem."));

            ContentDefinitionManager.AlterTypeDefinition("SliderItem",
                cfg => cfg
                    .WithPart("SliderItemPart")
                    .WithPart("CommonPart")
                    .WithPart("TitlePart")
                    .WithPart("BodyPart")
                    .Draftable()
                );

            ContentDefinitionManager.AlterPartDefinition("SliderItemPart", builder => builder
                   .WithField("Picture", cfg => cfg.OfType("MediaLibraryPickerField")
                   .WithSetting("MediaLibraryPickerFieldSettings.DisplayedContentTypes", "Image")
                   .WithSetting("MediaLibraryPickerFieldSettings.Multiple", "False"))
           );

            return 1;
        }

        public int UpdateFrom1()
        {
            ContentDefinitionManager.AlterPartDefinition("SliderWidgetPart", builder => builder
            .WithDescription("Turns content types into a SliderWidget."));

            ContentDefinitionManager.AlterTypeDefinition("SliderWidget", cfg => cfg
               .WithPart("SliderWidgetPart")
               .WithPart("WidgetPart")
               .WithPart("CommonPart")
               .WithSetting("Stereotype", "Widget"));

            return 2;
        }

        public int UpdateFrom2()
        {
            ContentDefinitionManager.AlterTypeDefinition("SliderItem", cfg => cfg
                .WithPart("LocalizationPart"));

            return 3;
        }

    }
}