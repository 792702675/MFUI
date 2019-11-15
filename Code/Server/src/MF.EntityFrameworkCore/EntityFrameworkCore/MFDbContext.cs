using Microsoft.EntityFrameworkCore;
using MF.Authorization.Roles;
using MF.Authorization.Users;
using MF.MultiTenancy;
using MF.VerificationCodes;
using MF.Friendships;
using MF.Chat;
using MF.Authorization.ThirdParty;
using MF.WebFiles;
using MF.Demos;
using MF.Storage;
using MF.AppEditions;
using MF.AppStartPages;
using MF.OSS;
using MF.Buckets;
using MF.FS430;
using MF.SystemFunctions;
using Abp.Zero.EntityFrameworkCore;

namespace MF.EntityFrameworkCore
{
    public class MFDbContext : AbpZeroDbContext<Tenant, Role, User, MFDbContext>
    {

        /* Define a DbSet for each entity of the application */
        public virtual DbSet<Menus.Menu> Menus { get; set; }
        public virtual DbSet<VerificationCode> VerificationCodes { get; set; }

        public virtual DbSet<Friendship> Friendships { get; set; }

        public virtual DbSet<ChatMessage> ChatMessages { get; set; }

        public virtual DbSet<ThirdPartyUser> ThirdPartyUsers { get; set; }
        public virtual DbSet<WebFile> WebFile { get; set; }
        public virtual DbSet<AppEdition> AppEdition { get; set; }
        public virtual DbSet<AppStartPage> AppStartPage { get; set; }
        public virtual DbSet<Demo> Demo { get; set; }
        public virtual DbSet<FileSettingDemo> GetSetDemo { get; set; }
        public virtual DbSet<BinaryObject> BinaryObjects { get; set; }
        public virtual DbSet<ContentObject> ContentObject { get; set; }
        public virtual DbSet<OSSObject> OSSObject { get; set; }
        public virtual DbSet<Bucket430> Bucket { get; set; }
        public virtual DbSet<Tag> Tag { get; set; }
        public virtual DbSet<ObjectTag> ObjectTag { get; set; }
        public virtual DbSet<SysFun> SysFun { get; set; }
        public virtual DbSet<SysFunTag> SysFunTag { get; set; }
        public virtual DbSet<SaltPot> SaltPot { get; set; }

        public MFDbContext(DbContextOptions<MFDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OSSObject>().HasIndex(b => b.BucketName);
            modelBuilder.Entity<OSSObject>().HasIndex(b => b.Key);
            modelBuilder.Entity<OSSObject>().HasIndex(b => b.Name);
            modelBuilder.Entity<OSSObject>().HasIndex(b => b.ExtensionName);
            modelBuilder.Entity<OSSObject>().HasIndex(b => b.ETag);

            modelBuilder.Entity<Tag>().HasIndex(b => b.Name);


            modelBuilder.Entity<Bucket430>().HasIndex(b => b.Name);
            modelBuilder.Entity<SaltPot>().HasIndex(b => b.Key);



            base.OnModelCreating(modelBuilder);
        }
    }
}
