using System.Data;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using Orchard.SEO.Services;

namespace Orchard.SEO
{
    public class Migrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable("MetaRecord", table => table
                .ContentPartRecord()
                .Column<string>("Keywords")
                .Column<string>("Title")
                .Column<string>("Description")
                .Column<string>("Robots")
            );

            ContentDefinitionManager.AlterPartDefinition(
                "MetaPart", cfg => cfg
                .WithDescription("Provides meta tags: title, description, keywords, robots")    
                .Attachable());

            SchemaBuilder.CreateTable("RobotsFileRecord",
                table => table
                    .Column<int>("Id", col => col.PrimaryKey().Identity())
                    .Column<string>("FileContent", col => col.Unlimited()
                    .WithDefault(@"User-agent: *
Allow: /"))
                );

            SchemaBuilder.CreateTable("SitemapFileRecord",
                table => table
                    .Column<int>("Id", col => col.PrimaryKey().Identity())
                    .Column<string>("FileContent", col => col.Unlimited().WithDefault(SitemapService.DefaultFileText))
                );

            return 1;
        }
    }
}