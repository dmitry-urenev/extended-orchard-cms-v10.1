using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;

namespace Orchard.ExContentManagement
{
    public class Migrations : DataMigrationImpl
    {
        public int Create()
        {      
            SchemaBuilder     
              .CreateTable("ParentContentPartRecord",
                             table => table
                                 .ContentPartRecord()
                                 .Column<int>("ParentId"));

            return 1;
        }

        public int UpdateFrom1()
        {       
            ContentDefinitionManager.AlterPartDefinition("ParentContentPart", builder => builder
                .WithDescription("Provides parent-to-child relation between content items."));

            ContentDefinitionManager.AlterTypeDefinition("Page", cfg => cfg
                .WithPart("ParentContentPart"));         
          
            ContentDefinitionManager.AlterTypeDefinition("Page",
                   cfg => cfg                       
                       .WithPart("AutoroutePart", builder => builder
                           .WithSetting("AutorouteSettings.AllowCustomPattern", "true")
                           .WithSetting("AutorouteSettings.AutomaticAdjustmentOnEdit", "false")
                           .WithSetting("AutorouteSettings.PatternDefinitions", "[{Name:'Parent Page and Title', Pattern: '{Content.Container.Path}/{Content.Slug}', Description: 'my-parent/my-page-url'}]")
                           .WithSetting("AutorouteSettings.DefaultPatternIndex", "0"))                       
                   );                               

            return 2;
        }

        public int UpdateFrom2()
        {
            ContentDefinitionManager.AlterPartDefinition("Menu", b => b
                .WithField("Order", cfg => cfg.OfType("NumericField").WithDisplayName("Order")));

            ContentDefinitionManager.AlterTypeDefinition("Menu",
               cfg => cfg
                   .WithPart("Menu"));

            return 3;
        }
    }
}
