using System;
using System.Drawing;
using System.Web;
using Abp.Configuration;
using Abp.Domain.Services;
using Abp.Runtime.Caching;
using Abp.UI;
using Newtonsoft.Json;
using MF.Configuration;
using MF.Geetests;
namespace MF.Captcha
{
    public class CaptchaManager : MFDomainServiceBase, ICaptchaManager
    {
        private readonly ICacheManager _cacheManager;

        public CaptchaManager(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        /// <inheritdoc />
        public void CheckCaptcha(string inputCaptcha)
        {
            var hasClientToken = HttpContext.Current.Request.Cookies.TryGetValue("ClientToken", out string requestCookie);
            if (!hasClientToken)
            {
                throw new UserFriendlyException("您的操作有误，请刷新重试");
            }
            var verifcationCache = _cacheManager.GetCache("ClientToken").GetOrDefault<string, VerifcationCache>("");
            switch (verifcationCache?.VerifcationType)
            {
                case VerifcationType.Image:
                    if (string.IsNullOrEmpty(inputCaptcha))
                    {
                        throw new UserFriendlyException("请输入验证码");
                    }
                    if (string.IsNullOrEmpty(verifcationCache?.Code))
                    {
                        throw new UserFriendlyException("验证码已过期，请刷新重试");
                    }
                    if (inputCaptcha.ToLower().Trim() != verifcationCache?.Code?.ToLower())
                    {
                        throw new UserFriendlyException("验证码输入错误");
                    }
                    _cacheManager.GetCache("ClientToken").Set("", "");
                    break;
                case VerifcationType.Drag:
                case VerifcationType.Geetest:
                    if (string.IsNullOrEmpty(inputCaptcha)
                        || string.IsNullOrEmpty(verifcationCache?.Code)
                        || inputCaptcha.ToLower().Trim() != verifcationCache?.Code?.ToLower())
                    {
                        throw new UserFriendlyException("验证码验证失败");
                    }
                    break;
                case VerifcationType.GeetestNow:
                    GeetestLib geetest = GetGeetestLib();
                    GeetestCheck input = JsonConvert.DeserializeObject<GeetestCheck>(inputCaptcha);
                    int result = geetest.enhencedValidateRequest(input.Challenge, input.Validate, input.Seccode);
                    if (result != 1)
                    {
                        throw new UserFriendlyException("验证失败");
                    }
                    break;
                default:
                    break;
            }
        }
        private GeetestLib GetGeetestLib()
        {
            return new GeetestLib(SettingManager.GetSettingValue(AppSettingNames.Captcha.Geetest.PublicKey), SettingManager.GetSettingValue(AppSettingNames.Captcha.Geetest.PrivateKey));
        }


        /// <inheritdoc />
        public Image GetCaptchaImage(int width = 80, int height = 40)
        {
            var buider = new ValidateImageBuilderOne { Height = height, Width = width };
            var content = CodeMaker.MakeCode();

            var hasClientToken= HttpContext.Current.Request.Cookies.TryGetValue("ClientToken", out string requestCookie);
            var clientToken = "";
            if (!hasClientToken)
            {
                clientToken = Guid.NewGuid().ToString("N");
                HttpContext.Current.Response.Cookies.Append("ClientToken", clientToken);
            }
            else
            {
                clientToken = requestCookie;
            }
            VerifcationCache verifcationCache = new VerifcationCache()
            {
                Code = content
            };
            _cacheManager.GetCache("ClientToken").Set(clientToken, verifcationCache);

            return buider.CreateImage(content);
        }
    }
}