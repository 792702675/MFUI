using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using MF.Authorization;
using MF.Authorization.Users;
using DataExporting;
using DataExporting.Dto;
using Abp.Application.Services;
using Abp.Domain.Entities;
using MF.CommonDto;
using System;
using System.Linq.Expressions;
using MF.Commons;
using Microsoft.EntityFrameworkCore;

namespace MF
{
    public class AsyncMFCrudAppService<TEntity, TEntityDto, TGetAllInput, TCreateInput, TUpdateInput> :
        AsyncMFCrudAppService<
            TEntity,
            TEntityDto,
            int,
            TGetAllInput,
            TCreateInput,
            TUpdateInput>
        where TEntity : class, IEntity<int>
        where TEntityDto : IEntityDto<int>
        where TUpdateInput : IEntityDto<int>
    {

        public AsyncMFCrudAppService(IRepository<TEntity, int> repository) : base(repository)
        {
        }
    }
    public class AsyncMFCrudAppService<TEntity, TEntityDto, TPrimaryKey, TGetAllInput, TCreateInput, TUpdateInput> :
        AsyncCrudAppService<
            TEntity,
            TEntityDto,
            TPrimaryKey,
            TGetAllInput,
            TCreateInput,
            TUpdateInput>
        where TEntity : class, IEntity<TPrimaryKey>
        where TEntityDto : IEntityDto<TPrimaryKey>
        where TUpdateInput : IEntityDto<TPrimaryKey>
    {

        //public IWeChatSession WeChatSession { get; set; }
        public IAppFolders AppFolders { get; set; }
        //public IAppSession AppSession { get; set; }


        private string _managePermissionName;
        public string ManagePermissionName
        {
            get { return _managePermissionName; }
            set
            {
                _managePermissionName = value;
                CreatePermissionName = _managePermissionName;
                UpdatePermissionName = _managePermissionName;
                DeletePermissionName = _managePermissionName;
                //GetAllPermissionName = _managePermissionName;
            }
        }

        public AsyncMFCrudAppService(
            IRepository<TEntity, TPrimaryKey> repository
        ) : base(repository)
        {
        }

        protected IQueryable<TEntity> ApplyFilter(IQueryable<TEntity> q, object input)
        {
            if (input is IFilteredResultRequest _input)
            {
                if (!string.IsNullOrWhiteSpace(_input.Filter))
                {
                    q = q.Filter(_input.Filter);
                }
            }
            return q;
        }

        protected override IQueryable<TEntity> CreateFilteredQuery(TGetAllInput input)
        {
            var q = base.CreateFilteredQuery(input);

            q = ApplyFilter(q, input);
            return q;
        }
        protected virtual void CreateProcessingEntity(TEntity entity, TCreateInput input)
        {

        }
        protected virtual void UpdateProcessingEntity(TEntity entity, TUpdateInput input)
        {

        }
        protected virtual void DeleteProcessingEntity(TEntity entity)
        {

        }


        public override async Task<TEntityDto> CreateAsync(TCreateInput input)
        {
            CheckCreatePermission();
            var _data = MapToEntity(input); 
            CreateProcessingEntity(_data, input);
            await Repository.InsertAsync(_data);
            await UnitOfWorkManager.Current.SaveChangesAsync();

            return MapToEntityDto(_data); 
        }

        public override async Task<TEntityDto> UpdateAsync(TUpdateInput input)
        {
            CheckUpdatePermission();

            var entity = await GetEntityByIdAsync(input.Id);

            MapToEntity(input, entity);
            UpdateProcessingEntity(entity, input);
            await CurrentUnitOfWork.SaveChangesAsync();

            return MapToEntityDto(entity);
        }
        private void CheckDelete(TEntity entity)
        {
            if (entity is ICanDelete canDeleteObj)
            {
                if (canDeleteObj is ICheckDelete checkDeleteObj)
                {
                    checkDeleteObj.CheckDelete();
                }
                else if (!canDeleteObj.CanDelete())
                {
                    throw new Abp.UI.UserFriendlyException("存在引用关系，不能删除。");
                }
            }
        }
        public override async Task DeleteAsync(EntityDto<TPrimaryKey> input)
        {
            CheckDeletePermission();
            var obj = await Repository.FirstOrDefaultAsync(input.Id);
            if (obj == null) { return; }
            CheckDelete(obj);
            DeleteProcessingEntity(obj);
            await Repository.DeleteAsync(obj);
        }
        public virtual async Task DeleteBatch(ArrayDto<TPrimaryKey> input)
        {
            CheckDeletePermission();
            var objs = await Repository.GetAllListAsync(x => input.Value.Contains(x.Id));
            foreach (var item in objs)
            {
                await DeleteAsync(new EntityDto<TPrimaryKey>( item.Id));
            }
        }

        /// <summary>
        /// 获取下拉列表
        /// </summary>
        protected async Task<List<NameValueDto<T>>> GetDropDownList<T>(Expression<Func<TEntity, NameValueDto<T>>> selector, Expression<Func<TEntity, bool>> predicate = null, string sort = null)
        {
            return await GetDropDownList(selector, x => x.WhereIf(predicate != null, predicate), sort);
        }
        /// <summary>
        /// 获取下拉列表
        /// </summary>
        protected async Task<List<T>> GetDropDownList<T>(Expression<Func<TEntity, T>> selector, Func<IQueryable<TEntity>, IQueryable<TEntity>> filter, string sort = null)
        {
            var q = Repository.GetAll();
            if (!string.IsNullOrEmpty(sort))
            {
                q = q.OrderBy(sort);
            }
            if (filter != null)
            {
                q = filter(q);
            }
            return await q
                .Select(selector)
                .ToListAsync();
        }

        protected async Task<FileDto> ExportToExcel(FilteredInputDto input, Func<IQueryable<TEntity>, IQueryable<TEntity>> where)
        {
            var q = Repository.GetAll();
            if (where != null)
            {
                q = where(q);
            }
            q = ApplyFilter(q, input);
            
            var data = ObjectMapper.Map<List<TEntityDto>>(await q.ToListAsync()); 
            FileDto fileinfo = new ExcelExporter().ExportToFile(data, AppFolders.TempFileDownloadFolder);
            return fileinfo;
        }
        protected virtual async Task<FileDto> ExportToExcel(FilteredInputDto input)
        {
            return await ExportToExcel(input, null);
        }

        public override async Task<TEntityDto> GetAsync(EntityDto<TPrimaryKey> input)
        {
            var q = CreateFilteredQuery(default(TGetAllInput));
            var data = await q.FirstOrDefaultAsync(x => (object)x.Id == (object)input.Id);
            return MapToEntityDto(data);
        }

    }
}