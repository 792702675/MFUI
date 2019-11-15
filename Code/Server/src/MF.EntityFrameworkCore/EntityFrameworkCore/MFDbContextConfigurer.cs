using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MF.EntityFrameworkCore
{
    public static class MFDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<MFDbContext> builder, string connectionString)
        {
            //var loggerFactory = new LoggerFactory();
            //loggerFactory.AddProvider(new EFLoggerProvider());
            //builder.UseLoggerFactory(loggerFactory);

            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<MFDbContext> builder, DbConnection connection)
        {
            //var loggerFactory = new LoggerFactory();
            //loggerFactory.AddProvider(new EFLoggerProvider());
            //builder.UseLoggerFactory(loggerFactory);

            builder.UseSqlServer(connection);
        }
    }
}
