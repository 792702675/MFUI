using System.ComponentModel.DataAnnotations;

namespace MF.OrganizationUnits.Dto
{
    /// <summary>
    /// �û�������֯����
    /// </summary>
    public class UserToOrganizationUnitInput
    {
        /// <summary>
        /// �û�ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// ��֯����ID
        /// </summary>
        [Range(1, long.MaxValue)]
        public long OrganizationUnitId { get; set; }
    }
}