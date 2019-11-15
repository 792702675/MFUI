using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using MF.Authorization;
using MF.Authorization.Users;
using Abp.Application.Services;
using MF.Demos.Dto;
using MF.CommonDto;
using MF.PreviousAndNexts;
using MF.JPush;
using System;
using MF.Storage;
using MF.Images;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace MF.Demos
{
    public class DemoAppService : AsyncMFCrudAppService<Demo, GetListDemoDto, PagedSortedAndFilteredInputDto, CreateDemoDto, UpdateDemoDto>, IDemoAppService
    {
        public ImageManage ImageManage { get; set; }
        public IRepository<ContentObject, Guid> ContentObjectRepository { get; set; }
        public DemoAppService(
            IRepository<Demo, int> repository
            ) : base(repository)
        {
            DeletePermissionName = PermissionNames.Pages_DemoMangeDelete;
            CreatePermissionName = PermissionNames.Pages_DemoMangeCreate;
            UpdatePermissionName = PermissionNames.Pages_DemoMangeUpdate;
        }
        public override async Task<GetListDemoDto> GetAsync(EntityDto<int> input)
        {
            var data = await Repository.GetAsync(input.Id);
            var r = ObjectMapper.Map <GetDemoDto>(data);
            r.PreviousAndNext = new PreviousAndNext<Demo>(Repository, r.Id);
            return r;
        }

        public async Task CreateContentObject()
        {
            var guid = Guid.NewGuid();
            var obj = new ContentObject();
            obj.Id = guid;
            obj.Content = "123";
            await ContentObjectRepository.InsertAsync(obj);
        }

        public void ImageThumbnailTest(string url)
        {
            var s = ImageManage.GetThumbnail(url);
        }
        public ImageInfo GetImageInfoTest(string url)
        {
            return ImageManage.GetImageInfo(url);
        }
        public void ImageCropTest(string url)
        {
            ImageManage.Crop(url);
        }

        [Authorize(PermissionNames.Oss_Aliyun_Manage)]
        public async Task<List<GetDemoDto>> GetDemo()
        {
            return ObjectMapper.Map<List<GetDemoDto>>(await Repository.GetAll().ToListAsync());
        }
    }
}
