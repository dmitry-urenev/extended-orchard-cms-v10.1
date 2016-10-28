using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;
using Orchard.Localization.Models;
using System.Linq;

namespace Orchard.Localization
{
    public class Migrations : DataMigrationImpl
    {

        public int Create()
        {
            SchemaBuilder.CreateTable("LocalizationPartRecord",
                table => table
                    .ContentPartRecord()
                    .Column<int>("CultureId")
                    .Column<int>("MasterContentItemId")
                );

            ContentDefinitionManager.AlterPartDefinition("LocalizationPart", builder => builder.Attachable());

            return 1;
        }

        public int UpdateFrom1()
        {
            ContentDefinitionManager.AlterPartDefinition("LocalizationPart", builder => builder
                .WithDescription("Provides the user interface to localize content items."));

            return 2;
        }

        public int UpdateFrom2()
        {
            if (!ContentDefinitionManager.GetTypeDefinition("Page")
                .Parts.Any(p => p.PartDefinition.Name == "LocalizationPart"))
            {
                ContentDefinitionManager.AlterTypeDefinition("Page", builder => builder
                    .WithPart("LocalizationPart"));
            }

            ContentDefinitionManager.AlterTypeDefinition("MenuItem", builder => builder
                .WithPart("LocalizationPart"));

            ContentDefinitionManager.AlterTypeDefinition("ContentMenuItem", builder => builder
                .WithPart("LocalizationPart"));

            // Culture Switcher
            SchemaBuilder.CreateTable("CultureSwitcherPartRecord", table => table
              .ContentPartRecord()
              .Column<int>("DisplayMode")
              .Column<int>("DisplayType")
              .Column<string>("OrderedCultures")
           );

            ContentDefinitionManager.AlterPartDefinition(typeof(CultureSwitcherPart).Name, cfg => cfg.Attachable());

            ContentDefinitionManager.AlterTypeDefinition("CultureSwitcherWidget", cfg => cfg
                .WithPart("CultureSwitcherPart")
                .WithPart("WidgetPart")
                .WithPart("CommonPart")
                .WithSetting("Stereotype", "Widget")); 
           
            return 3;
        }
    }

    [OrchardFeature("Orchard.Localization.Transliteration")]
    public class TransliterationMigrations : DataMigrationImpl
    {

        public int Create()
        {
            SchemaBuilder.CreateTable("TransliterationSpecificationRecord",
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("CultureFrom")
                    .Column<string>("CultureTo")
                    .Column<string>("Rules", c => c.Unlimited())
                );

            return 1;
        }
    }
}