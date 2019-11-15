using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using MF.Configuration;
using MF.Web;

namespace MF.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class MFDbContextFactory : IDesignTimeDbContextFactory<MFDbContext>
    {
        public MFDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<MFDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            MFDbContextConfigurer.Configure(builder, configuration.GetConnectionString(MFConsts.ConnectionStringName));

            return new MFDbContext(builder.Options);
        }
    }
}
