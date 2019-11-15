using Abp.Dependency;
using Abp.Runtime.Caching;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;

namespace MF.QRLogin
{
    public class QRLoginHub : Hub, ITransientDependency
    {
        private readonly ICacheManager _cacheManager;
        public QRLoginHub(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public QRCodeInfo GetToken()
        {
            var info = new QRCodeInfo()
            {
                ConnectionId = Context.ConnectionId
            };
            _cacheManager.GetCache("QRLoginHub").Set(Context.ConnectionId, info);
            return info;
        }
    }
}
