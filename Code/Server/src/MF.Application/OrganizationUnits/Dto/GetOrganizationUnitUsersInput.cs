using System;
using System.ComponentModel.DataAnnotations;
using Abp.Runtime.Validation;
using MF.CommonDto;

namespace MF.OrganizationUnits.Dto
{
    /// <summary>
    /// ��ȡ��֯����Ա��Ϣ����
    /// </summary>
    public class GetOrganizationUnitUsersInput : PagedAndSortedInputDto, IShouldNormalize
    {
        /// <summary>
        /// ����ID
        /// </summary>
        [Range(1, long.MaxValue)]
        public long Id { get; set; }

        /// <summary>
        /// �Ƿ�ݹ�����¼���������
        /// </summary>
        public bool IsRecursiveSearch { get; set; }

        /// <summary>
        /// �û����ƹ�������
        /// </summary>
        public string NameFilter { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "user.Name, user.Surname";
            }
            else if (Sorting.Contains("userName"))
            {
                Sorting = Sorting.Replace("userName", "user.userName");
            }
            else if (Sorting.Contains("addedTime"))
            {
                Sorting = Sorting.Replace("addedTime", "uou.creationTime");
            }
        }
    }
}