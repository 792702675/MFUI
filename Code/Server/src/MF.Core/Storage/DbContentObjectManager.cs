using System;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;

namespace MF.Storage
{
    public class DbContentObjectManager : MFDomainServiceBase, IContentObjectManager, ITransientDependency
    {
        private readonly IRepository<ContentObject, Guid> _contentObjectRepository;

        public DbContentObjectManager(IRepository<ContentObject, Guid> contentObjectRepository)
        {
            _contentObjectRepository = contentObjectRepository;
        }

        public Task<ContentObject> GetOrNullAsync(Guid id)
        {
            return _contentObjectRepository.FirstOrDefaultAsync(id);
        }

        public async Task<ContentObject> SaveAsync(ContentObject file)
        {
            var data = await _contentObjectRepository.InsertAsync(file);
            await UnitOfWorkManager.Current.SaveChangesAsync();
            return data;
        }

        public async Task DeleteAsync(Guid id)
        {
            var data = await GetOrNullAsync(id);
            data.RelatedId = null;
            await _contentObjectRepository.DeleteAsync(data);
        }
        public async Task UpdateContent(Guid id, string content, string html, Guid? group)
        {
            var data = await _contentObjectRepository.GetAsync(id);
            data.Content = content;
            data.HtmlContent = html;
            data.Group = group;
            data.IsUpdate = true;
            data.LastUpdateTime = DateTime.Now;

            await UnitOfWorkManager.Current.SaveChangesAsync();
        }
    }
}