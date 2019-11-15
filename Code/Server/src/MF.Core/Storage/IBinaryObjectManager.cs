using System;
using System.Threading.Tasks;

namespace MF.Storage
{
    public interface IContentObjectManager
    {
        Task<ContentObject> GetOrNullAsync(Guid id);

        Task<ContentObject> SaveAsync(ContentObject file);

        Task DeleteAsync(Guid id);
        Task UpdateContent(Guid id, string content, string html, Guid? group);
    }
}