using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Abp;
using Abp.Auditing;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.IO.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using Abp.Web.Models;
using MF.Authorization.Users;
//using MF.Friendships;
using MF.IO;
using MF.Storage;
using DataExporting.Net.MimeTypes;
using Microsoft.AspNetCore.Mvc;
using Abp.AspNetCore.Mvc.Authorization;

namespace MF.Controllers
{
    /// <summary>  /// </summary>
    public class ProfileController : MFControllerBase
    {
        private readonly UserManager _userManager;
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly IAppFolders _appFolders;
        //private readonly IFriendshipManager _friendshipManager;

        /// <summary>  /// </summary>
        public ProfileController(
            UserManager userManager,
            IBinaryObjectManager binaryObjectManager,
            IAppFolders appFolders
            //,
            //IFriendshipManager friendshipManager
            )
        {
            _userManager = userManager;
            _binaryObjectManager = binaryObjectManager;
            _appFolders = appFolders;
            //_friendshipManager = friendshipManager;
        }

        /// <summary>  /// </summary>
        [DisableAuditing]
        public async Task<FileResult> GetProfilePicture()
        {
            var user = await _userManager.GetUserByIdAsync(AbpSession.GetUserId());
            if (user.ProfilePictureId == null)
            {
                return GetDefaultProfilePicture();
            }

            return await GetProfilePictureById(user.ProfilePictureId.Value);
        }

        /// <summary>  /// </summary>
        [DisableAuditing]
        public async Task<FileResult> GetProfilePictureById(string id = "")
        {
            if (id.IsNullOrEmpty())
            {
                return GetDefaultProfilePicture();
            }

            return await GetProfilePictureById(Guid.Parse(id));
        }


        //TODO: DELETE not used action!!!

        /// <summary>  /// </summary>
        [UnitOfWork]
        [AbpMvcAuthorize]
        public virtual async Task<JsonResult> ChangeProfilePicture()
        {
            try
            {
                //Check input
                if (Request.Form.Files.Count <= 0 || Request.Form.Files[0] == null)
                {
                    throw new UserFriendlyException(L("ProfilePicture_Change_Error"));
                }

                var file = Request.Form.Files[0];

                if (file.Length > 30720) //30KB.
                {
                    throw new UserFriendlyException(L("ProfilePicture_Warn_SizeLimit", "30KB"));
                }

                //Get user
                var user = await _userManager.GetUserByIdAsync(AbpSession.GetUserId());

                //Delete old picture
                if (user.ProfilePictureId.HasValue)
                {
                    await _binaryObjectManager.DeleteAsync(user.ProfilePictureId.Value);
                }

                //Save new picture
                var storedFile = new BinaryObject(AbpSession.TenantId, file.OpenReadStream().GetAllBytes());
                await _binaryObjectManager.SaveAsync(storedFile);

                //Update new picture on the user
                user.ProfilePictureId = storedFile.Id;

                //Return success
                return Json(new AjaxResponse());
            }
            catch (UserFriendlyException ex)
            {
                //Return error message
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        /// <summary>  /// </summary>
        [AbpMvcAuthorize]
        public JsonResult UploadProfilePicture()
        {
            try
            {
                //Check input
                if (Request.Form.Files.Count <= 0 || Request.Form.Files[0] == null)
                {
                    throw new UserFriendlyException(L("ProfilePicture_Change_Error"));
                }

                var file = Request.Form.Files[0];

                if (file.Length > 1048576) //1MB.
                {
                    throw new UserFriendlyException(L("ProfilePicture_Warn_SizeLimit"));
                }

                //Check file type & format
                var fileImage = Image.FromStream(file.OpenReadStream());
                var acceptedFormats = new List<ImageFormat>
                {
                    ImageFormat.Jpeg, ImageFormat.Png, ImageFormat.Gif
                };

                if (!acceptedFormats.Contains(fileImage.RawFormat))
                {
                    throw new ApplicationException("Uploaded file is not an accepted image file !");
                }

                //Delete old temp profile pictures
                AppFileHelper.DeleteFilesInFolderIfExists(_appFolders.TempFileDownloadFolder, "userProfileImage_" + AbpSession.GetUserId());

                //Save new picture
                var fileInfo = new FileInfo(file.FileName);
                var tempFileName = "userProfileImage_" + AbpSession.GetUserId() + fileInfo.Extension;
                var tempFilePath = Path.Combine(_appFolders.TempFileDownloadFolder, tempFileName);
                using (var fs = System.IO.File.OpenWrite(tempFilePath))
                {
                    file.CopyTo(fs);
                }

                using (var bmpImage = new Bitmap(tempFilePath))
                {
                    return Json(new AjaxResponse(new { fileName = tempFileName, width = bmpImage.Width, height = bmpImage.Height }));
                }
            }
            catch (UserFriendlyException ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        private FileResult GetDefaultProfilePicture()
        {
            return File("/Common/Images/default-profile-picture.png", MimeTypeNames.ImagePng);
        }

        private async Task<FileResult> GetProfilePictureById(Guid profilePictureId)
        {
            var file = await _binaryObjectManager.GetOrNullAsync(profilePictureId);
            if (file == null)
            {
                return GetDefaultProfilePicture();
            }

            return File(file.Bytes, MimeTypeNames.ImageJpeg);
        }

        /// <summary> 
        /// ΪͼƬ��������ͼ 
        /// </summary> 
        /// <param name="image">ԭͼƬ</param> 
        /// <param name="width">����ͼ��</param> 
        /// <param name="height">����ͼ��</param> 
        /// <returns></returns> 
        private System.Drawing.Image GetHvtThumbnail(System.Drawing.Image image, int width, int height)
        {
            Bitmap m_hovertreeBmp = new Bitmap(width, height);
            //��Bitmap����һ��System.Drawing.Graphics 
            Graphics m_HvtGr = Graphics.FromImage(m_hovertreeBmp);
            //����  
            m_HvtGr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //�������Ҳ��ɸ����� 
            m_HvtGr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            //����������High 
            m_HvtGr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            //��ԭʼͼ����Ƴ����������ÿ�ߵ���Сͼ 
            Rectangle rectDestination = new Rectangle(0, 0, width, height);

            int m_width, m_height;
            if (image.Width * height > image.Height * width)
            {
                m_height = image.Height;
                m_width = (image.Height * width) / height;
            }
            else
            {
                m_width = image.Width;
                m_height = (image.Width * height) / width;
            }

            m_HvtGr.DrawImage(image, rectDestination, 0, 0, m_width, m_height, GraphicsUnit.Pixel);

            return m_hovertreeBmp;
        }
    }
}