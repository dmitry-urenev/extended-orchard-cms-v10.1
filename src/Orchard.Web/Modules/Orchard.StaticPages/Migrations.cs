using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using System.Linq;

namespace Orchard.StaticPages {
    public class Migrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder
                .CreateTable("RouteRecord",
                             table => table
                                          .Column<int>("Id", column => column.PrimaryKey().Identity())
                                          .Column<string>("Path", c => c.WithLength(2048))
                                          .Column<int>("Action_id")
                                          .Column<string>("RouteValues", c => c.Unlimited()))
                .CreateTable("StaticPagePartRecord",
                             table => table
                                 .ContentPartRecord()
                                 .Column<int>("RouteId")
                                 .Column<string>("StaticPageUrl", c => c.WithLength(2048))
                                 .Column<bool>("IsAction")
                             );              
              
            ContentDefinitionManager.AlterTypeDefinition("StaticPage",
                cfg => cfg
                .WithPart("TitlePart")
                .WithPart("CommonPart")                
                .WithPart("AutoroutePart", builder => builder
                    .WithSetting("AutorouteSettings.AllowCustomPattern", "true")
                    .WithSetting("AutorouteSettings.AutomaticAdjustmentOnEdit", "false")
                    .WithSetting("AutorouteSettings.PatternDefinitions", "[{Name:'Title', Pattern: '{Content.Slug}', Description: 'my-page'}]")
                    .WithSetting("AutorouteSettings.DefaultPatternIndex", "0"))
                .WithPart("StaticPagePart")                
                .Creatable());
            return 1;
        }
    }
}