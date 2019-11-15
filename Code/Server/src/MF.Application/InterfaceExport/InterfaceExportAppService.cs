using System.Web;
using Abp.Configuration;
using Newtonsoft.Json;
using MF.Configuration;

namespace MF.InterfaceExport
{
    /// <summary>
    /// 接口导出实现
    /// </summary>
    public class InterfaceExportAppService : MFAppServiceBase, IInterfaceExportAppService
    {
        private const string SwaggerUrl = "http://generator.swagger.io/api/gen/clients/typescript-fetch";
        private const string DownloadUrl = "http://generator.swagger.io/api/gen/download/";
        /// <inheritdoc />
        public string GetReactDownloadUrl()
        {
            var data = HttpRequester.Request(GetSwaggerApiUrl(), new HttpRequester.RequestOptions());
            var postData = $"{{\"spec\":{data}}}";
            var download = HttpRequester.Request(SwaggerUrl, 
                new HttpRequester.RequestOptions
                {
                    Method = HttpRequester.HttpMethod.Post,
                }, 
                postData);
            return DownloadUrl + JsonConvert.DeserializeObject<dynamic>(download).code;
        }

        private string GetSwaggerApiUrl()
        {
            return $"{HttpContext.Current.Request.Scheme}://{HttpContext.Current.Request.Host}/swagger/v1/swagger.json";
        }
    }
}
