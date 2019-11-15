using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using MF.OSS;
using Abp.Dependency;

namespace MF.Wopi
{
    public class ContentProvider
    {
        private class LockInfo
        {
            public string Lock { get; set; }
            public DateTime DateCreated { get; set; }
            public bool Expired { get { return this.DateCreated.AddMinutes(30) < DateTime.UtcNow; } }
        }

        /// <summary>
        /// Simplified Lock info storage.
        /// A real lock implementation would use persised storage for locks.
        /// </summary>
        private static readonly Dictionary<string, LockInfo> Locks = new Dictionary<string, LockInfo>();


        //声明请求代理
        private readonly RequestDelegate _nextDelegate;
        private readonly OSSManage _ossManage;


        public ContentProvider(RequestDelegate nextDelegate, OSSManage ossManage)
        {
            _nextDelegate = nextDelegate;
            _ossManage = ossManage;
        }


        //拉截并接受所有请求
        public async Task Invoke(Microsoft.AspNetCore.Http.HttpContext context)
        {
            //判断是否为来自WOPI服务端的请求
            if (context.Request.Path.ToString().ToLower().IndexOf("files") >= 0)
            {
                WopiRequest requestData = ParseRequest(context.Request);

                switch (requestData.Type)
                {
                    //获取文件信息
                    case RequestType.CheckFileInfo:
                        await HandleCheckFileInfoRequest(context, requestData);
                        break;

                    //尝试解锁并重新锁定
                    case RequestType.UnlockAndRelock:
                        HandleUnlockAndRelockRequest(context, requestData);
                        break;

                    //获取文件
                    case RequestType.GetFile:
                        await HandleGetFileRequest(context, requestData);
                        break;

                    //写入文件
                    case RequestType.PutFile:
                        await HandlePutFileRequest(context, requestData);
                        break;

                    default:
                        ReturnServerError(context.Response);
                        break;
                }
            }
            else
            {
                await _nextDelegate.Invoke(context);
            }
        }

        private static OSSManage GetOSSManage()
        {
            return IocManager.Instance.Resolve<OSSManage>();
        }

        private static WopiRequest ParseRequest(HttpRequest request)
        {
            // Initilize wopi request data object with default values
            WopiRequest requestData = new WopiRequest()
            {
                Type = RequestType.None,
                AccessToken = request.Query["access_token"],
                Id = ""
            };

            // request.Url pattern:
            // http(s)://server/<...>/wopi/[files|folders]/<id>?access_token=<token>
            // or
            // https(s)://server/<...>/wopi/files/<id>/contents?access_token=<token>
            // or
            // https(s)://server/<...>/wopi/folders/<id>/children?access_token=<token>

            // Get request path, e.g. /<...>/wopi/files/<id>
            string requestPath = request.Path; //request.Url.AbsolutePath;
            // remove /<...>/wopi/
            string wopiPath = requestPath.Substring(WopiConfig.WopiPath.Length);

            if (wopiPath.StartsWith(WopiConfig.FilesRequestPath))
            {
                // A file-related request

                // remove /files/ from the beginning of wopiPath
                string rawId = wopiPath.Substring(WopiConfig.FilesRequestPath.Length);

                if (rawId.EndsWith(WopiConfig.ContentsRequestPath))
                {
                    // The rawId ends with /contents so this is a request to read/write the file contents

                    // Remove /contents from the end of rawId to get the actual file id
                    requestData.Id = rawId.Substring(0, rawId.Length - WopiConfig.ContentsRequestPath.Length);

                    if (request.Method == "GET")
                        requestData.Type = RequestType.GetFile;
                    if (request.Method == "POST")
                        requestData.Type = RequestType.PutFile;
                }
                else
                {
                    requestData.Id = rawId;

                    if (request.Method == "GET")
                    {
                        // a GET to the file is always a CheckFileInfo request
                        requestData.Type = RequestType.CheckFileInfo;
                    }
                    else if (request.Method == "POST")
                    {
                        // For a POST to the file we need to use the X-WOPI-Override header to determine the request type
                        string wopiOverride = request.Headers[WopiHeaders.RequestType];

                        switch (wopiOverride)
                        {
                            case "PUT_RELATIVE":
                                requestData.Type = RequestType.PutRelativeFile;
                                break;
                            case "LOCK":
                                // A lock could be either a lock or an unlock and relock, determined based on whether
                                // the request sends an OldLock header.
                                if (request.Headers[WopiHeaders.OldLock] != (string)null)
                                    requestData.Type = RequestType.UnlockAndRelock;
                                else
                                    requestData.Type = RequestType.Lock;
                                break;
                            case "UNLOCK":
                                requestData.Type = RequestType.Unlock;
                                break;
                            case "REFRESH_LOCK":
                                requestData.Type = RequestType.RefreshLock;
                                break;
                            case "COBALT":
                                requestData.Type = RequestType.ExecuteCobaltRequest;
                                break;
                            case "DELETE":
                                requestData.Type = RequestType.DeleteFile;
                                break;
                            case "READ_SECURE_STORE":
                                requestData.Type = RequestType.ReadSecureStore;
                                break;
                            case "GET_RESTRICTED_LINK":
                                requestData.Type = RequestType.GetRestrictedLink;
                                break;
                            case "REVOKE_RESTRICTED_LINK":
                                requestData.Type = RequestType.RevokeRestrictedLink;
                                break;
                        }
                    }
                }
            }
            else if (wopiPath.StartsWith(WopiConfig.FoldersRequestPath))
            {
                // A folder-related request.

                // remove /folders/ from the beginning of wopiPath
                string rawId = wopiPath.Substring(WopiConfig.FoldersRequestPath.Length);

                if (rawId.EndsWith(WopiConfig.ChildrenRequestPath))
                {
                    // rawId ends with /children, so it's an EnumerateChildren request.

                    // remove /children from the end of rawId
                    requestData.Id = rawId.Substring(0, rawId.Length - WopiConfig.ChildrenRequestPath.Length);
                    requestData.Type = RequestType.EnumerateChildren;
                }
                else
                {
                    // rawId doesn't end with /children, so it's a CheckFolderInfo.

                    requestData.Id = rawId;
                    requestData.Type = RequestType.CheckFolderInfo;
                }
            }
            else
            {
                // An unknown request.
                requestData.Type = RequestType.None;
            }
            return requestData;
        }



        /// <summary>
        /// 接受并处理获取文件信息的请求
        /// </summary>
        /// <remarks>
        /// </remarks>
        private async Task HandleCheckFileInfoRequest(Microsoft.AspNetCore.Http.HttpContext context, WopiRequest requestData)
        {
            //判断是否有合法token    
            if (!ValidateAccess(requestData, writeAccessRequired: false))
            {
                ReturnInvalidToken(context.Response);
                return;
            }
            //获取文件           
            //var fileinfo = await GetOSSManage().GetFileInfo(requestData.Id.ToInt());
            var fileinfo = new FileInfo(HttpContext.Current.MapPath("word.docx"));
            DateTime? lastModifiedTime = DateTime.Now;
            try
            {
                CheckFileInfoResponse responseData = new CheckFileInfoResponse()
                {
                    //获取文件名称
                    BaseFileName = Path.GetFileName(requestData.Id),
                    //Size = fileinfo.Size,
                    //Version = (fileinfo.LastModified ?? DateTime.Now).ToFileTimeUtc().ToString(),
                    Size = fileinfo.Length,
                    Version = (fileinfo.LastWriteTimeUtc).ToFileTimeUtc().ToString(),
                    SupportsLocks = true,
                    SupportsUpdate = true,
                    UserCanNotWriteRelative = true,

                    ReadOnly = false,
                    UserCanWrite = true
                };

                var jsonString = JsonConvert.SerializeObject(responseData);

                ReturnSuccess(context.Response);

                await context.Response.WriteAsync(jsonString);

            }
            catch (UnauthorizedAccessException )
            {
                ReturnFileUnknown(context.Response);
            }
        }


        private void HandleUnlockAndRelockRequest(Microsoft.AspNetCore.Http.HttpContext context, WopiRequest requestData)
        {
            if (!ValidateAccess(requestData, writeAccessRequired: true))
            {
                ReturnInvalidToken(context.Response);
                return;
            }

            if (!File.Exists(requestData.FullPath))
            {
                ReturnFileUnknown(context.Response);
                return;
            }

            string newLock = context.Request.Headers[WopiHeaders.Lock];
            string oldLock = context.Request.Headers[WopiHeaders.OldLock];

            lock (Locks)
            {
                if (TryGetLock(requestData.Id, out LockInfo existingLock))
                {
                    if (existingLock.Lock == oldLock)
                    {
                        // There is a valid lock on the file and the existing lock matches the provided one

                        // Replace the existing lock with the new one
                        Locks[requestData.Id] = new LockInfo() { DateCreated = DateTime.UtcNow, Lock = newLock };
                        context.Response.Headers[WopiHeaders.OldLock] = newLock;
                        ReturnSuccess(context.Response);
                    }
                    else
                    {
                        // The existing lock doesn't match the requested one.  Return a lock mismatch error
                        // along with the current lock
                        ReturnLockMismatch(context.Response, existingLock.Lock);
                    }
                }
                else
                {
                    // The requested lock does not exist.  That's also a lock mismatch error.
                    ReturnLockMismatch(context.Response, reason: "File not locked");
                }
            }
        }
        /// <summary>
        /// 接受并处理获取文件的请求
        /// </summary>
        /// <remarks>
        /// </remarks>
        private async Task HandleGetFileRequest(Microsoft.AspNetCore.Http.HttpContext context, WopiRequest requestData)
        {
            //判断是否有合法token    
            if (!ValidateAccess(requestData, writeAccessRequired: false))
            {
                ReturnInvalidToken(context.Response);
                return;
            }


            //获取文件             
            //var file = await _ossManage.GetFile(requestData.Id.ToInt());

            //if (null == file)
            //{
            //    ReturnFileUnknown(context.Response);
            //    return;
            //}

            //var stream = file.Content;

            var fileinfo = new FileInfo(HttpContext.Current.MapPath("word.docx"));
            var stream = fileinfo.OpenRead ();

            try
            {
                //int i = 0;
                //List<byte> bytes = new List<byte>();
                //do
                //{
                //    byte[] buffer = new byte[1024];
                //    i = stream.Read(buffer, 0, 1024);
                //    if (i > 0)
                //    {
                //        byte[] data = new byte[i];
                //        Array.Copy(buffer, data, i);
                //        bytes.AddRange(data);
                //    }
                //}
                //while (i > 0);


                ReturnSuccess(context.Response);
                context.Response.Headers.Add(WopiHeaders.ItemVersion, (fileinfo.LastWriteTimeUtc).ToFileTimeUtc().ToString());
                await stream.CopyToAsync( context.Response.Body);

            }
            catch (UnauthorizedAccessException)
            {
                ReturnFileUnknown(context.Response);
            }
            catch (FileNotFoundException )
            {
                ReturnFileUnknown(context.Response);
            }

        }

        /// <summary>
        /// 接受并处理写入文件的请求
        /// </summary>
        /// <remarks>
        /// </remarks>
        private async Task HandlePutFileRequest(Microsoft.AspNetCore.Http.HttpContext context, WopiRequest requestData)
        {
            await Task.FromResult(0);
            //判断是否有合法token    
            if (!ValidateAccess(requestData, writeAccessRequired: true))
            {
                ReturnInvalidToken(context.Response);
                return;
            }

            try
            {
                //写入文件			
                int result = 0;//await storage.UploadFile(requestData.FileId, context.Request.Body);
                if (result != 0)
                {
                    ReturnServerError(context.Response);
                    return;
                }

                ReturnSuccess(context.Response);
            }
            catch (UnauthorizedAccessException)
            {
                ReturnFileUnknown(context.Response);
            }
            catch (IOException )
            {
                ReturnServerError(context.Response);
            }
        }



        private static bool ValidateAccess(WopiRequest requestData, bool writeAccessRequired)
        {
            // TODO: Access token validation is not implemented in this sample.
            // For more details on access tokens, see the documentation
            // https://wopi.readthedocs.io/projects/wopirest/en/latest/concepts.html#term-access-token
            // "INVALID" is used by the WOPIValidator.
            return !string.IsNullOrWhiteSpace(requestData.AccessToken) && (requestData.AccessToken != "INVALID");
        }


        private static void ReturnSuccess(HttpResponse response)
        {
            ReturnStatus(response, 200, "Success");
        }

        private static void ReturnInvalidToken(HttpResponse response)
        {
            ReturnStatus(response, 401, "Invalid Token");
        }

        private static void ReturnFileUnknown(HttpResponse response)
        {
            ReturnStatus(response, 404, "File Unknown/User Unauthorized");
        }

        private static void ReturnLockMismatch(HttpResponse response, string existingLock = null, string reason = null)
        {
            response.Headers[WopiHeaders.Lock] = existingLock ?? string.Empty;
            if (!string.IsNullOrEmpty(reason))
            {
                response.Headers[WopiHeaders.LockFailureReason] = reason;
            }

            ReturnStatus(response, 409, "Lock mismatch/Locked by another interface");
        }

        private static void ReturnServerError(HttpResponse response)
        {
            ReturnStatus(response, 500, "Server Error");
        }

        private static void ReturnUnsupported(HttpResponse response)
        {
            ReturnStatus(response, 501, "Unsupported");
        }

        private static void ReturnStatus(HttpResponse response, int code, string description)
        {
            response.StatusCode = code;
            //response.StatusDescription = description;
        }

        private bool TryGetLock(string fileId, out LockInfo lockInfo)
        {
            // TODO: This lock implementation is not thread safe and not persisted and all in all just an example.
            if (Locks.TryGetValue(fileId, out lockInfo))
            {
                if (lockInfo.Expired)
                {
                    Locks.Remove(fileId);
                    return false;
                }
                return true;
            }

            return false;
        }

    }
}
