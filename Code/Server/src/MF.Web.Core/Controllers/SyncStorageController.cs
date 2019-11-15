using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Abp.Auditing;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.IO.Extensions;
using Abp.UI;
using DataExporting.Net.MimeTypes;
using MF.Controllers;
using MF.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace MF.Web.Controllers
{
    public class SyncStorageController : MFControllerBase
    {
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly IRepository<BinaryObject, Guid> _syncBinaryObjectRepository;
        private readonly IRepository<BinaryObject, Guid> _binaryObjectRepository;

        public SyncStorageController(
            IBinaryObjectManager binaryObjectManager,
            IRepository<BinaryObject, Guid> syncBinaryObjectRepository,
            IRepository<BinaryObject, Guid> binaryObjectRepository
            )
        {
            _binaryObjectManager = binaryObjectManager;
            _syncBinaryObjectRepository = syncBinaryObjectRepository;
            _binaryObjectRepository = binaryObjectRepository;
        }

        public async Task<string> Copy()
        {
            var data = _syncBinaryObjectRepository.GetAll().ToList();
            var totalCount = data.Count;
            var realCount = 0;
            foreach (var item in data)
            {
                if (!await _binaryObjectRepository.GetAll().AnyAsync(x => x.Id == item.Id))
                {
                    await _binaryObjectRepository.InsertAsync(new BinaryObject
                    {
                        Id = item.Id,
                        Bytes = item.Bytes,
                        ContentType = item.ContentType,
                        TenantId = item.TenantId
                    });
                    realCount++;
                }
            }
            return $"{realCount}/{totalCount}";
        }

        /// <summary>
        /// ��ȡ�洢���ļ�
        /// </summary>
        [DisableAuditing]
        public async Task<ActionResult> GetFileById(string id = "")
        {
            var headerValue = Request.Headers["If-Modified-Since"];
            if (!StringValues.IsNullOrEmpty(headerValue))
            {
                return new StatusCodeResult(304);
            }

            Response.Headers.Add("Last-Modified", new DateTime(2018, 1, 1).ToUniversalTime().ToString("R"));

            if (id.IsNullOrEmpty())
            {
                return null;
            }
            var file = await _binaryObjectManager.GetOrNullAsync(Guid.Parse(id));
            return File(file?.Bytes, file?.ContentType);
        }


        /// <summary>
        /// ��ȡ�洢���ļ�
        /// </summary>
        [DisableAuditing]
        public async Task<FileResult> GetFileThumbById(string id = "")
        {
            if (id.IsNullOrEmpty())
            {
                return null;
            }
            var file = await _binaryObjectManager.GetOrNullAsync(Guid.Parse(id));
            if (file.Bytes == null || file.Bytes.Length <= 0)
            {
                return null;
            }
            using (var ims = new MemoryStream(file.Bytes))
            using (var oms = new MemoryStream())

            {
                ims.Seek(0, SeekOrigin.Begin);
                var image = Image.FromStream(ims, true, true);
                image.GetThumbnailImage(40, 40, null, IntPtr.Zero).Save(oms, ImageFormat.Png);
                oms.Seek(0, SeekOrigin.Begin);
                return File(oms.ToArray(), file?.ContentType);
            }
        }

        /// <summary>
        /// �ϴ��ļ�
        /// </summary>
        private async Task<Guid> Upload(string contentType)
        {
            //Check input
            if (Request.Form.Files.Count <= 0 || Request.Form.Files[0] == null)
            {
                throw new UserFriendlyException(L("ProfilePicture_Change_Error"));
            }
            var file = Request.Form.Files[0];
            return await Save(file.OpenReadStream().GetAllBytes(), contentType ?? MimeTypeNames.ImageJpeg);
        }

        /// <summary>
        /// �ϴ�Base64ͼƬ
        /// </summary>
        public async Task<Guid> UploadBase64([FromBody]string data)
        {
            //Check input
            if (string.IsNullOrEmpty(data))
            {
                throw new UserFriendlyException("û�����ݡ�");
            }
            return await _binaryObjectManager.SaveBase64Async(data);
        }
        private async Task<Guid> Save(byte[] data, string contentType)
        {
            var storedFile = new BinaryObject(AbpSession.TenantId ?? 0, data, contentType);
            await _binaryObjectManager.SaveAsync(storedFile);
            return storedFile.Id;
        }

        /// <summary>
        /// �ϴ�Jpeg�ļ�
        /// </summary>
        public virtual async Task<Guid> UploadImageJpeg()
        {
            return await Upload(MimeTypeNames.ImageJpeg);
        }
        /// <summary>
        /// �ϴ�Svg�ļ�
        /// </summary>
        public virtual async Task<Guid> UploadSvg()
        {
            return await Upload(MimeTypeNames.ImageSvgXml);
        }

        /// <summary>
        /// ɾ���ļ�
        /// </summary>
        public async Task Delete(string id)
        {
            if (!id.IsNullOrEmpty())
            {
                await _binaryObjectManager.DeleteAsync(Guid.Parse(id));
            }
        }

    }
}
