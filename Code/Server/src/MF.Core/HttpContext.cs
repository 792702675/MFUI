using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MF
{
    public static class HttpContext
    {
        public static IServiceProvider ServiceProvider;

        public static Microsoft.AspNetCore.Http.HttpContext Current
        {
            get
            {
                object factory = ServiceProvider.GetService(typeof(Microsoft.AspNetCore.Http.IHttpContextAccessor));
                Microsoft.AspNetCore.Http.HttpContext context = ((Microsoft.AspNetCore.Http.HttpContextAccessor)factory).HttpContext;
                return context;
            }
        }

        public static string MapPath(this Microsoft.AspNetCore.Http.HttpContext context, string path)
        {
            var _hostingEnvironment = (IWebHostEnvironment)ServiceProvider.GetService(typeof(IWebHostEnvironment));
            return Path.Combine(_hostingEnvironment.ContentRootPath, path.TrimStart('~').TrimStart('\\').TrimStart('/'));
        }
        public static string MapWebPath(this Microsoft.AspNetCore.Http.HttpContext context, string path)
        {
            var _hostingEnvironment = (IWebHostEnvironment)ServiceProvider.GetService(typeof(IWebHostEnvironment));
            return Path.Combine(_hostingEnvironment.WebRootPath, path.TrimStart('~').TrimStart('\\').TrimStart('/'));
        }

    }
}
