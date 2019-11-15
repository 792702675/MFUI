using Abp.Configuration;
using Abp.Runtime.Caching;
using Abp.Web.Models;
using Newtonsoft.Json;
using MF.Captcha;
using MF.Configuration;
using MF.DragVerifications.Dto;
using MF.Geetests.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MF.Geetests
{
    public class GeetestAppService : MFAppServiceBase, IGeetestAppService
    {
        private readonly ICacheManager _cacheManager;
        public GeetestAppService(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }
        private void SetCache(VerifcationCache verifcationCache)
        {
            var hasClientToken = HttpContext.Current.Request.Cookies.TryGetValue("ClientToken", out string requestCookie);
            if (!hasClientToken)
            {
                requestCookie = Guid.NewGuid().ToString("N");
                HttpContext.Current.Response.Cookies.Append("ClientToken", requestCookie);
            }
            if (verifcationCache != null)
            {
                _cacheManager.GetCache("ClientToken").Set(requestCookie, verifcationCache);
            }
            else
            {
                _cacheManager.GetCache("ClientToken").Remove(requestCookie);
            }
        }
        private VerifcationCache GetCache()
        {
            var hasClientToken = HttpContext.Current.Request.Cookies.TryGetValue("ClientToken", out string requestCookie);
            if (!hasClientToken)
            {
                throw new Abp.UI.UserFriendlyException("错误的请求");
            }
            var verifcationCache = _cacheManager.GetCache("ClientToken").GetOrDefault<string, VerifcationCache>(requestCookie);
            if (verifcationCache?.VerifcationType != VerifcationType.Drag)
            {
                throw new Abp.UI.UserFriendlyException("错误的请求");
            }
            return verifcationCache;
        }

        public string GetCaptcha()
        {
            var verifcationCache = new VerifcationCache()
            {
                VerifcationType = VerifcationType.GeetestNow
            };
            SetCache(verifcationCache);

            GeetestLib geetest = GetGeetestLib();
            Byte gtServerStatus = geetest.preProcess();
            return geetest.getResponseStr();
        }


        public CheckCodeOutput Check(GeetestCheckInput input)
        {
            GeetestLib geetest = GetGeetestLib();
            int result = geetest.enhencedValidateRequest(input.Challenge, input.Validate, input.Seccode);
            if (result == 1)
            {
                var verifcationCache = new VerifcationCache()
                {
                    VerifcationType = VerifcationType.Geetest,
                    Code = Guid.NewGuid().ToString()
                };
                SetCache(verifcationCache);
                return new CheckCodeOutput()
                {
                    Success = true,
                    Token = verifcationCache.Code
                };
            }
            else
            {
                return new CheckCodeOutput() { Success = false };
            }
        }

        public GeetestCheckOutput APPGetCaptcha()
        {
            return JsonConvert.DeserializeObject<GeetestCheckOutput>(GetCaptcha());
        }

        public string APPCheck(GeetestAppCheckInput input)
        {
            var result = Check(new GeetestCheckInput()
            {
                Challenge = input.geetest_challenge,
                Seccode = input.geetest_seccode,
                Validate = input.geetest_validate,
            });
            if (result.Success)
            {
                return "success";
            }
            else
            {
                throw new Abp.UI.UserFriendlyException("invalid");
            }
        }
        private GeetestLib GetGeetestLib()
        {
            return new GeetestLib(SettingManager.GetSettingValue(AppSettingNames.Captcha.Geetest.PublicKey), SettingManager.GetSettingValue(AppSettingNames.Captcha.Geetest.PrivateKey));
        }
    }
}
