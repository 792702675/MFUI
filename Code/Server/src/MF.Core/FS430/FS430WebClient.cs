using Abp;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Extensions;
using Abp.Web.Models;
using MF.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace MF.FS430
{
    public class FS430WebClient : MFDomainServiceBase
    {
        public static TimeSpan DefaultTimeout { get; set; }

        public string BaseUrl { get; set; }

        public TimeSpan Timeout { get; set; }

        public Collection<Cookie> Cookies { get; private set; }

        public ICollection<NameValue> RequestHeaders { get; private set; }

        public ICollection<NameValue> ResponseHeaders { get; private set; }

        ISettingManager _settingManager { get; set; }
        static FS430WebClient()
        {
            DefaultTimeout = TimeSpan.FromSeconds(90);
        }

        public FS430WebClient(ISettingManager settingManager)
        {
            _settingManager = settingManager;

            Timeout = DefaultTimeout;
            Cookies = new Collection<Cookie>();
            RequestHeaders = new List<NameValue>();
            RequestHeaders.Add(new NameValue("Token", _settingManager.GetSettingValue(AppSettingNames.OSS.FS430.AccessKey)));
            ResponseHeaders = new List<NameValue>();
        }

        public virtual async Task PostAsync(string url, int? timeout = null)
        {
            await PostAsync<object>(url, timeout);
        }

        public virtual async Task PostAsync(string url, object input, int? timeout = null)
        {
            await PostAsync<object>(url, input, timeout);
        }

        public virtual async Task<TResult> PostAsync<TResult>(string url, int? timeout = null)
            where TResult : class
        {
            return await PostAsync<TResult>(url, null, timeout);
        }

        public virtual async Task<TResult> PostAsync<TResult>(string url, object input, int? timeout = null)
            where TResult : class
        {
            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler { CookieContainer = cookieContainer })
            {
                using (var client = new HttpClient(handler))
                {
                    client.Timeout = timeout.HasValue ? TimeSpan.FromMilliseconds(timeout.Value) : Timeout;

                    if (!BaseUrl.IsNullOrEmpty())
                    {
                        client.BaseAddress = new Uri(BaseUrl);
                    }

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    foreach (var header in RequestHeaders)
                    {
                        client.DefaultRequestHeaders.Add(header.Name, header.Value);
                    }

                    using (var requestContent = new StringContent(Object2JsonString(input), Encoding.UTF8, "application/json"))
                    {
                        foreach (var cookie in Cookies)
                        {
                            if (!BaseUrl.IsNullOrEmpty())
                            {
                                cookieContainer.Add(new Uri(BaseUrl), cookie);
                            }
                            else
                            {
                                cookieContainer.Add(cookie);
                            }
                        }

                        using (var response = await client.PostAsync(url, requestContent))
                        {
                            SetResponseHeaders(response);

                            if (!response.IsSuccessStatusCode)
                            {
                                throw new AbpException("Could not made request to " + url + "! StatusCode: " + response.StatusCode + ", ReasonPhrase: " + response.ReasonPhrase);
                            }

                            var ajaxResponse = JsonString2Object<AjaxResponse<TResult>>(await response.Content.ReadAsStringAsync());
                            if (!ajaxResponse.Success)
                            {
                                throw new Abp.UI.UserFriendlyException(ajaxResponse.Error.Code, ajaxResponse.Error.Message, ajaxResponse.Error.Details);
                            }

                            return ajaxResponse.Result;
                        }
                    }
                }
            }
        }
        public virtual async Task UploadAsync(string url, Stream stream, int? timeout = null)
        {
            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler { CookieContainer = cookieContainer })
            {
                using (var client = new HttpClient(handler))
                {
                    client.Timeout = timeout.HasValue ? TimeSpan.FromMilliseconds(timeout.Value) : Timeout;

                    if (!BaseUrl.IsNullOrEmpty())
                    {
                        client.BaseAddress = new Uri(BaseUrl);
                    }

                    //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                    foreach (var header in RequestHeaders)
                    {
                        client.DefaultRequestHeaders.Add(header.Name, header.Value);
                    }
                    using (var form = new MultipartFormDataContent())
                    {

                        using (var fileContent = new ByteArrayContent(File.ReadAllBytes("D:\\123.txt")))
                        {
                            form.Add(fileContent);

                            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = Path.GetFileName(url), DispositionType = DispositionTypeNames.Attachment, Name = "fileData" };

                            form.Headers.Remove("Content-Type");

                            string text2 = "---------------------" + DateTime.Now.Ticks.ToString("x", NumberFormatInfo.InvariantInfo);

                            form.Headers.Add("Content-Type", "multipart/form-data; boundary=" + text2);



                            using (var response = await client.PostAsync(url, form))
                            {
                                SetResponseHeaders(response);

                                if (!response.IsSuccessStatusCode)
                                {
                                    throw new AbpException("Could not made request to " + url + "! StatusCode: " + response.StatusCode + ", ReasonPhrase: " + response.ReasonPhrase);
                                }
                            }
                        }


                    }
                }
            }
        }


        private void SetResponseHeaders(HttpResponseMessage response)
        {
            ResponseHeaders.Clear();
            foreach (var header in response.Headers)
            {
                foreach (var headerValue in header.Value)
                {
                    ResponseHeaders.Add(new NameValue(header.Key, headerValue));
                }
            }
        }

        private static string Object2JsonString(object obj)
        {
            if (obj == null)
            {
                return "";
            }

            return JsonConvert.SerializeObject(obj,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
        }

        private static TObj JsonString2Object<TObj>(string str)
        {
            return JsonConvert.DeserializeObject<TObj>(str,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
        }
    }
}
