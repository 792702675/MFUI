using System.ComponentModel.DataAnnotations;

namespace MF.OrganizationUnits.Dto
{
    /// <summary>
    /// �û�������֯����
    /// </summary>
    public class UsersToOrganizationUnitInput
    {
        /// <summary>
        /// �û�ID
        /// </summary>
        public string UserIdListStr { get; set; }

        /// <summary>
        /// ��֯����ID
        /// </summary>
        [Range(1, long.MaxValue)]
        public long OrganizationUnitId { get; set; }
    }
}