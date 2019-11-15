using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace OSS.EntityFrameworkCore
{
    public static class OSSDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<OSSDbContext> builder, string connectionString)
        {
            builder.UseSqlite(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<OSSDbContext> builder, DbConnection connection)
        {
            builder.UseSqlite(connection);
        }
    }
}
