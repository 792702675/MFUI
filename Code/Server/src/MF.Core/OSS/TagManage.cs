using Abp.Domain.Repositories;
using Abp.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MF.OSS
{
    public class TagManage : MFDomainServiceBase
    {
        private readonly IRepository<Tag, int> _repository;
        public TagManage(
            IRepository<Tag, int> repository
            )
        {
            _repository = repository;
        }

        public IEnumerable<int> GetTagId(string[] tagName)
        {
            foreach (var item in tagName.Where(x => !x.IsNullOrEmpty())) // 空标签不是好标签
            {
                var tag = _repository.FirstOrDefault(x => x.Name == item);
                if (tag == null)
                {
                    tag = new Tag { IsSystemTag = false, Name = item, };
                    _repository.InsertAndGetId(tag);
                }
                yield return tag.Id;
            }
        }

    }
}
