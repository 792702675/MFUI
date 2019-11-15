using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp;
using Abp.Domain.Entities;

namespace MF.Storage
{
    [Table("AppBinaryObjects")]
    public class BinaryObject : Entity<Guid>, IMayHaveTenant
    {
        public virtual int? TenantId { get; set; }

        [Required]
        public virtual byte[] Bytes { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        [StringLength(20)]
        public string ContentType { get; set; }

        public BinaryObject()
        {
            Id = SequentialGuidGenerator.Instance.Create();
        }

        public BinaryObject(int? tenantId, byte[] bytes)
            : this()
        {
            TenantId = tenantId;
            Bytes = bytes;
        }
        public BinaryObject(int? tenantId, byte[] bytes, string contentType = "")
            : this()
        {
            TenantId = tenantId;
            Bytes = bytes;
            ContentType = contentType;
        }

        public static string GetFileUrl(Guid? guid)
        {
            return guid.HasValue ? $"SyncStorage/GetFileById?id={guid}" : "";
        }
    }
}
