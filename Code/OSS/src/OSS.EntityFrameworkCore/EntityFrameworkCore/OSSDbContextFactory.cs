using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using OSS.Configuration;
using OSS.Web;

namespace OSS.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class OSSDbContextFactory : IDesignTimeDbContextFactory<OSSDbContext>
    {
        public OSSDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<OSSDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            OSSDbContextConfigurer.Configure(builder, configuration.GetConnectionString(OSSConsts.ConnectionStringName));

            return new OSSDbContext(builder.Options);
        }
    }
}
