using System;
using System.IO;

namespace MF.Wopi
{
    enum RequestType
    {
        None,

        CheckFileInfo,
        PutRelativeFile,

        Lock,
        Unlock,
        RefreshLock,
        UnlockAndRelock,

        ExecuteCobaltRequest,

        DeleteFile,
        ReadSecureStore,
        GetRestrictedLink,
        RevokeRestrictedLink,

        CheckFolderInfo,

        GetFile,
        PutFile,

        EnumerateChildren,
    }

    static class WopiHeaders
    {
        public const string RequestType = "X-WOPI-Override";
        public const string ItemVersion = "X-WOPI-ItemVersion";

        public const string Lock = "X-WOPI-Lock";
        public const string OldLock = "X-WOPI-OldLock";
        public const string LockFailureReason = "X-WOPI-LockFailureReason";
        public const string LockedByOtherInterface = "X-WOPI-LockedByOtherInterface";

        public const string SuggestedTarget = "X-WOPI-SuggestedTarget";
        public const string RelativeTarget = "X-WOPI-RelativeTarget";
        public const string OverwriteRelativeTarget = "X-WOPI-OverwriteRelativeTarget";
    }

    class WopiRequest
    {
        public RequestType Type { get; set; }

        public string AccessToken { get; set; }

        public string Id { get; set; }

        public string FullPath
        {
            get { return Path.Combine(LocalStoragePath, Id); }
        }
        internal static string LocalStoragePath
        {
            get
            {
                var lsp = "";// Config["LocalStoragePath"].ToString();
                if (string.IsNullOrWhiteSpace(lsp))
                {
                    lsp = AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/").TrimEnd('/') + "/upload/";
                }
                return lsp;
            }
        }
    }
}
