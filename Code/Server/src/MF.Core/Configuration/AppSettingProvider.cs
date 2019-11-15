using System.Collections.Generic;
using System.Configuration;
using Abp.Configuration;
using Abp.Json;
using Abp.Zero.Configuration;
using MF.Security;

namespace MF.Configuration
{
    public class AppSettingProvider : SettingProvider
    {
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            context.Manager.GetSettingDefinition(AbpZeroSettingNames.UserManagement.TwoFactorLogin.IsEnabled).DefaultValue = false.ToString().ToLowerInvariant();
            return new[]
            {
                new SettingDefinition(AppSettingNames.UiTheme, "red", scopes: SettingScopes.Application | SettingScopes.Tenant | SettingScopes.User, isVisibleToClients: true),


                new SettingDefinition(AppSettingNames.UiTheme, "#108ee9", scopes: SettingScopes.Application | SettingScopes.Tenant | SettingScopes.User, isVisibleToClients: true),
                new SettingDefinition(AppSettingNames.Skin, "默认 ", scopes: SettingScopes.Application | SettingScopes.Tenant | SettingScopes.User, isVisibleToClients: true),
                new SettingDefinition(AppSettingNames.SiteUrl, "http://mf.kingyuer.cn:666/index.html", scopes: SettingScopes.Application, isVisibleToClients: true),

                new SettingDefinition(AppSettingNames.UserManagement.AllowSelfRegistration, ConfigurationManager.AppSettings[AppSettingNames.UserManagement.AllowSelfRegistration] ?? "true"),
                new SettingDefinition(AppSettingNames.UserManagement.IsNewRegisteredUserActiveByDefault, ConfigurationManager.AppSettings[AppSettingNames.UserManagement.IsNewRegisteredUserActiveByDefault] ?? "false"),
                new SettingDefinition(AppSettingNames.UserManagement.UseCaptchaOnRegistration, ConfigurationManager.AppSettings[AppSettingNames.UserManagement.UseCaptchaOnRegistration] ?? "true"),
                new SettingDefinition(AppSettingNames.UserManagement.IsPhoneNumberConfirmationRequiredForLogin, ConfigurationManager.AppSettings[AppSettingNames.UserManagement.IsPhoneNumberConfirmationRequiredForLogin] ?? "false"),


                new SettingDefinition(AppSettingNames.TenantManagement.AllowSelfRegistration,ConfigurationManager.AppSettings[AppSettingNames.TenantManagement.UseCaptchaOnRegistration] ?? "true"),
                new SettingDefinition(AppSettingNames.TenantManagement.IsNewRegisteredTenantActiveByDefault,ConfigurationManager.AppSettings[AppSettingNames.TenantManagement.IsNewRegisteredTenantActiveByDefault] ??"false"),
                new SettingDefinition(AppSettingNames.TenantManagement.UseCaptchaOnRegistration,ConfigurationManager.AppSettings[AppSettingNames.TenantManagement.UseCaptchaOnRegistration] ?? "true"),
                new SettingDefinition(AppSettingNames.TenantManagement.DefaultEdition,ConfigurationManager.AppSettings[AppSettingNames.TenantManagement.DefaultEdition] ?? ""),

                new SettingDefinition(AppSettingNames.Security.PasswordComplexity, PasswordComplexitySetting.DefaultPasswordComplexitySetting.ToJsonString()),


                new SettingDefinition(AppSettingNames.SMS.DefaultSender, ConfigurationManager.AppSettings[AppSettingNames.SMS.DefaultSender] ?? "QCSMSSenderManager"),
                new SettingDefinition(AppSettingNames.SMS.FreeSignName, ConfigurationManager.AppSettings[AppSettingNames.SMS.FreeSignName] ?? "青才科技"),
                new SettingDefinition(AppSettingNames.SMS.Ali.RegionEndpoint, ConfigurationManager.AppSettings[AppSettingNames.SMS.Ali.RegionEndpoint] ?? "http://1910484995838290.mns.cn-shenzhen.aliyuncs.com/"),
                new SettingDefinition(AppSettingNames.SMS.Ali.AccessKeyId, ConfigurationManager.AppSettings[AppSettingNames.SMS.Ali.AccessKeyId] ?? "LTAIJQTANmluFsyt"),
                new SettingDefinition(AppSettingNames.SMS.Ali.SecretAccessKey, ConfigurationManager.AppSettings[AppSettingNames.SMS.Ali.SecretAccessKey] ?? "5CXwsmA7aOQbK7S2cGwgwsViPXRSwU"),
                new SettingDefinition(AppSettingNames.SMS.Ali.TopicName, ConfigurationManager.AppSettings[AppSettingNames.SMS.Ali.TopicName] ?? "sms.topic-cn-shenzhen"),
                new SettingDefinition(AppSettingNames.SMS.Ali.TemplateCode, ConfigurationManager.AppSettings[AppSettingNames.SMS.Ali.TemplateCode] ?? "SMS_57610033"),

                new SettingDefinition(AppSettingNames.SMS.QC.Url, ConfigurationManager.AppSettings[AppSettingNames.SMS.QC.Url] ?? "http://sms.easyitcn.cn/sms"),
                new SettingDefinition(AppSettingNames.SMS.QC.Username, ConfigurationManager.AppSettings[AppSettingNames.SMS.QC.Username] ?? "zth"),
                new SettingDefinition(AppSettingNames.SMS.QC.Password, ConfigurationManager.AppSettings[AppSettingNames.SMS.QC.Password] ?? "123456"),


                new SettingDefinition(AppSettingNames.OAuth.WeixinOpen.IsEnabled, ConfigurationManager.AppSettings[AppSettingNames.OAuth.WeixinOpen.IsEnabled] ?? "false"),
                new SettingDefinition(AppSettingNames.OAuth.WeixinOpen.AppID, ConfigurationManager.AppSettings[AppSettingNames.OAuth.WeixinOpen.AppID] ?? ""),
                new SettingDefinition(AppSettingNames.OAuth.WeixinOpen.AppSecret, ConfigurationManager.AppSettings[AppSettingNames.OAuth.WeixinOpen.AppSecret] ?? ""),

                new SettingDefinition(AppSettingNames.OAuth.Alipay.IsEnabled, ConfigurationManager.AppSettings[AppSettingNames.OAuth.Alipay.IsEnabled] ?? "false"),
                new SettingDefinition(AppSettingNames.OAuth.Alipay.AppID, ConfigurationManager.AppSettings[AppSettingNames.OAuth.Alipay.AppID] ?? ""),
                new SettingDefinition(AppSettingNames.OAuth.Alipay.AppPrivateKey, ConfigurationManager.AppSettings[AppSettingNames.OAuth.Alipay.AppPrivateKey] ?? ""),
                new SettingDefinition(AppSettingNames.OAuth.Alipay.AlipayPublicKey, ConfigurationManager.AppSettings[AppSettingNames.OAuth.Alipay.AlipayPublicKey] ?? ""),

                new SettingDefinition(AppSettingNames.OAuth.QQ.IsEnabled, ConfigurationManager.AppSettings[AppSettingNames.OAuth.QQ.IsEnabled] ?? "false"),
                new SettingDefinition(AppSettingNames.OAuth.QQ.AppID, ConfigurationManager.AppSettings[AppSettingNames.OAuth.QQ.AppID] ?? ""),
                new SettingDefinition(AppSettingNames.OAuth.QQ.AppKey, ConfigurationManager.AppSettings[AppSettingNames.OAuth.QQ.AppKey] ?? ""),

                new SettingDefinition(AppSettingNames.OAuth.Weibo.IsEnabled, ConfigurationManager.AppSettings[AppSettingNames.OAuth.Weibo.IsEnabled] ?? "false"),
                new SettingDefinition(AppSettingNames.OAuth.Weibo.AppID, ConfigurationManager.AppSettings[AppSettingNames.OAuth.Weibo.AppID] ?? ""),
                new SettingDefinition(AppSettingNames.OAuth.Weibo.AppSecret, ConfigurationManager.AppSettings[AppSettingNames.OAuth.Weibo.AppSecret] ?? ""),

                new SettingDefinition(AppSettingNames.Captcha.Geetest.PublicKey, ConfigurationManager.AppSettings[AppSettingNames.Captcha.Geetest.PublicKey] ?? "7a444abca9308c3d4ff09593553b6783"),
                new SettingDefinition(AppSettingNames.Captcha.Geetest.PrivateKey, ConfigurationManager.AppSettings[AppSettingNames.Captcha.Geetest.PrivateKey] ?? "8de52190dec3ae0841b398bf683302d8"),


                new SettingDefinition(AppSettingNames.System.ImageUploadPath, ConfigurationManager.AppSettings[AppSettingNames.System.ImageUploadPath] ?? "~/Common/Images/UserPics"),
                new SettingDefinition(AppSettingNames.System.SystemName, ConfigurationManager.AppSettings[AppSettingNames.System.SystemName] ?? "后台管理系统",scopes: SettingScopes.Application | SettingScopes.Tenant),

                new SettingDefinition(AppSettingNames.Push.JPush.JPushAppKey, ConfigurationManager.AppSettings[AppSettingNames.Push.JPush.JPushAppKey] ?? ""),
                new SettingDefinition(AppSettingNames.Push.JPush.JPushMasterSecret, ConfigurationManager.AppSettings[AppSettingNames.Push.JPush.JPushMasterSecret] ?? ""),

                new SettingDefinition(AppSettingNames.Push.Getui.AppID, ConfigurationManager.AppSettings[AppSettingNames.Push.Getui.AppID] ?? ""),
                new SettingDefinition(AppSettingNames.Push.Getui.AppKey, ConfigurationManager.AppSettings[AppSettingNames.Push.Getui.AppKey] ?? ""),
                new SettingDefinition(AppSettingNames.Push.Getui.MasterSecret, ConfigurationManager.AppSettings[AppSettingNames.Push.Getui.MasterSecret] ?? ""),

                // 阿里oss
                new SettingDefinition(AppSettingNames.OSS.Aliyun.Endpoint, ConfigurationManager.AppSettings[AppSettingNames.OSS.Aliyun.Endpoint] ?? "http://oss-cn-hangzhou.aliyuncs.com"),
                new SettingDefinition(AppSettingNames.OSS.Aliyun.AccessKeyId, ConfigurationManager.AppSettings[AppSettingNames.OSS.Aliyun.AccessKeyId] ?? "LTAIzSvLObgWldCj"),
                new SettingDefinition(AppSettingNames.OSS.Aliyun.AccessKeySecret, ConfigurationManager.AppSettings[AppSettingNames.OSS.Aliyun.AccessKeySecret] ?? "c4sdyaEQkQFDCWmDRQwDmaVvhgtLqM"),


                // oss基础设置
                new SettingDefinition(AppSettingNames.OSS.OfficeOnlineServerUrl, ConfigurationManager.AppSettings[AppSettingNames.OSS.OfficeOnlineServerUrl] ?? "http://220.165.143.91"),
                new SettingDefinition(AppSettingNames.OSS.BucketCounct, ConfigurationManager.AppSettings[AppSettingNames.OSS.BucketCounct] ?? "10"),
                new SettingDefinition(AppSettingNames.OSS.BucketPrefix, ConfigurationManager.AppSettings[AppSettingNames.OSS.BucketPrefix] ?? ""),
                new SettingDefinition(AppSettingNames.OSS.ProhibitedFileType, ConfigurationManager.AppSettings[AppSettingNames.OSS.ProhibitedFileType] ?? ""),
                new SettingDefinition(AppSettingNames.OSS.ContextStore, ConfigurationManager.AppSettings[AppSettingNames.OSS.ContextStore] ?? "430schoolcommon"),

                // oss跨域规则
                new SettingDefinition(AppSettingNames.OSS.AllowedOrigins, ConfigurationManager.AppSettings[AppSettingNames.OSS.AllowedOrigins] ?? ""),
                new SettingDefinition(AppSettingNames.OSS.AllowedMethods, ConfigurationManager.AppSettings[AppSettingNames.OSS.AllowedMethods] ?? ""),
                new SettingDefinition(AppSettingNames.OSS.AllowedHeaders, ConfigurationManager.AppSettings[AppSettingNames.OSS.AllowedHeaders] ?? ""),
                new SettingDefinition(AppSettingNames.OSS.ExposedHeaders, ConfigurationManager.AppSettings[AppSettingNames.OSS.ExposedHeaders] ?? ""),
                new SettingDefinition(AppSettingNames.OSS.MaxAgeSeconds, ConfigurationManager.AppSettings[AppSettingNames.OSS.MaxAgeSeconds] ?? "10"),

                // oss防盗链
                new SettingDefinition(AppSettingNames.OSS.AllowEmptyReferer, ConfigurationManager.AppSettings[AppSettingNames.OSS.AllowEmptyReferer] ?? "true"),
                new SettingDefinition(AppSettingNames.OSS.Referer, ConfigurationManager.AppSettings[AppSettingNames.OSS.Referer] ?? ""),

                // 430文件系统
                new SettingDefinition(AppSettingNames.OSS.FS430.Endpoint, ConfigurationManager.AppSettings[AppSettingNames.OSS.FS430.Endpoint] ?? "http://220.165.143.84:8081"),
                new SettingDefinition(AppSettingNames.OSS.FS430.AccessKey, ConfigurationManager.AppSettings[AppSettingNames.OSS.FS430.AccessKey] ?? "FS430EduV2"),
                new SettingDefinition(AppSettingNames.OSS.FS430.FileType, ConfigurationManager.AppSettings[AppSettingNames.OSS.FS430.FileType] ?? ".mp4 .rmvb .avi"),


                new SettingDefinition(AppSettingNames.Collections.DefalutUnlockValue, ConfigurationManager.AppSettings[AppSettingNames.Collections.DefalutUnlockValue] ?? "300"),
                new SettingDefinition(AppSettingNames.Collections.DefalutFrameDelay, ConfigurationManager.AppSettings[AppSettingNames.Collections.DefalutFrameDelay] ?? "1000"),


            };
        }
    }
}
