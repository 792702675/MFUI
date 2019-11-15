using System;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;

namespace MF.Storage
{
    public class DbBinaryObjectManager : MFDomainServiceBase,IBinaryObjectManager, ITransientDependency
    {
        private readonly IRepository<BinaryObject, Guid> _binaryObjectRepository;

        public DbBinaryObjectManager(IRepository<BinaryObject, Guid> binaryObjectRepository)
        {
            _binaryObjectRepository = binaryObjectRepository;
        }

        public Task<BinaryObject> GetOrNullAsync(Guid id)
        {
            return _binaryObjectRepository.FirstOrDefaultAsync(id);
        }

        public async Task<BinaryObject> SaveAsync(BinaryObject file)
        {
            var data = await _binaryObjectRepository.InsertAsync(file);
            await UnitOfWorkManager.Current.SaveChangesAsync();
            return data;
        }

        public Task DeleteAsync(Guid id)
        {
            return _binaryObjectRepository.DeleteAsync(id);
        }
        public async Task<Guid> SaveBase64Async(string base64)
        {
            byte[] bytes = Convert.FromBase64String(base64);
            var storedFile = new BinaryObject(AbpSession.TenantId ?? 0, bytes, "image/jpeg");
            await SaveAsync(storedFile);
            return storedFile.Id;
        }
    }
}