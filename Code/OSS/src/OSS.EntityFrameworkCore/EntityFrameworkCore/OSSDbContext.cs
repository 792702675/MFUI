using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using OSS.Authorization.Roles;
using OSS.Authorization.Users;
using OSS.MultiTenancy;

namespace OSS.EntityFrameworkCore
{
    public class OSSDbContext : AbpZeroDbContext<Tenant, Role, User, OSSDbContext>
    {
        /* Define a DbSet for each entity of the application */
        
        public OSSDbContext(DbContextOptions<OSSDbContext> options)
            : base(options)
        {
        }
    }
}
