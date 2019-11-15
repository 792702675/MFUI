using Abp.Domain.Services;
using System.Collections.Generic;

namespace MF.JPush
{
    public interface IGetuiServer : IDomainService
    {
        void Push(string msg, params string[] tagList);
    }
}