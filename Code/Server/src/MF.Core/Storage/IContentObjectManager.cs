using System;
using System.Threading.Tasks;

namespace MF.Storage
{
    public interface IBinaryObjectManager
    {
        Task<BinaryObject> GetOrNullAsync(Guid id);

        Task<BinaryObject> SaveAsync(BinaryObject file);

        Task DeleteAsync(Guid id);

        Task<Guid> SaveBase64Async(string base64);
    }
}