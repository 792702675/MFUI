using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Uow;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MF.OSS
{
    public class OssCreateFolderJob : AsyncBackgroundJob<CreateFolderDto>, ITransientDependency
    {
        private readonly OSSManage _oSSManage;
        public OssCreateFolderJob(OSSManage oSSManage)
        {
            _oSSManage = oSSManage;
        }

        [UnitOfWork]
        protected override async Task ExecuteAsync(CreateFolderDto data)
        {
            await _oSSManage.CreateFolder(data.BucketName, data.Key, data.Tags, data.IsHidden);
        }

    }
    public class CreateFolderDto
    {
        public string BucketName { get; set; }
        public string Key { get; set; }
        public string[] Tags { get; set; }
        public bool IsHidden { get; set; }
    }
}
