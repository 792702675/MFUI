using Abp.AutoMapper;

namespace MF.Authorization.Permissions.Dto
{
    /// <summary>
    /// Ȩ��
    /// </summary>
    [AutoMapFrom(typeof(Abp.Authorization.Permission))]
    public class FlatPermissionDto
    {
        /// <summary>
        /// �ϼ�����
        /// </summary>
        public string ParentName { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ��ʾ����
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsGrantedByDefault { get; set; }
    }
}