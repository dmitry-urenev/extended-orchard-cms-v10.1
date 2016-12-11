using System;
using System.Collections.Generic;
using System.Data;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;

namespace Orchard.SocialLinks
{
    public class Migrations : DataMigrationImpl {

        public int Create()
        {
            // Creating our database table for SocialLink
            SchemaBuilder.CreateTable("FollowMePartRecord", table => table
                .ContentPartRecord()
                .Column("TwitterUrl", DbType.String)
                .Column("FacebookUrl", DbType.String)
                .Column("GoogleUrl", DbType.String)
                .Column("LinkedInUrl", DbType.String)
                .Column("GitHubUrl", DbType.String)
                .Column("YouTubeUrl", DbType.String)
                .Column("XingUrl", DbType.String)
            );

            ContentDefinitionManager.AlterTypeDefinition("FollowMeWidget", cfg => cfg
                .WithPart("FollowMePart")
                .WithPart("WidgetPart")
                .WithPart("CommonPart")
                .WithSetting("Stereotype", "Widget")
                .WithSetting("Description", "Displays icons and links to your profile pages on popular social websites.")
                );

            ContentDefinitionManager.AlterTypeDefinition("SocialShareWidget", cfg => cfg
                .WithPart("SocialSharePart")
                .WithPart("WidgetPart")
                .WithPart("CommonPart")
                .WithSetting("Stereotype", "Widget")
                .WithSetting("Description", "Displays icons and links to share current content page on popular social websites.")
                );

            return 1;
        }
    }
}