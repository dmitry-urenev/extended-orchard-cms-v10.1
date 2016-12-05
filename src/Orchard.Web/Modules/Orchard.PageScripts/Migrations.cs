using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using System.Linq;

namespace Orchard.PageScripts
{
    public class Migrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder
                .CreateTable("PageScriptsInfo",
                             table => table
                                .Column<int>("Id", column => column.PrimaryKey().Identity())
                                .Column<string>("TopHeaderScript", c => c.Unlimited())
                                .Column<string>("BottomHeaderScript", c => c.Unlimited())
                                .Column<string>("TopBodyScript", c => c.Unlimited())
                                .Column<string>("BottomBodyScript", c => c.Unlimited()));

            return 1;
        }
    }
}