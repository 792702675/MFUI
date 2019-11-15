using Abp.Configuration;
using MF.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF.Notifications
{
    public class TemplateMessageNotification : MFDomainServiceBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="openId"></param>
        /// <param name="first">模板消息的标题</param>
        /// <param name="content">预警内容</param>
        /// <param name="target">预警目标</param>
        /// <param name="remark">模板消息的备注</param>
        /// <param name="time">预警时间</param>
        /// <param name="url"></param>
        public void SendWarningTemplateMessage(string openId, string first, string content, string target, string remark, DateTime? time = null, string url = "")
        {
            //if (time == null)
            //{
            //    time = DateTime.Now;
            //}
            //if (!string.IsNullOrWhiteSpace(openId))
            //{
            //    try
            //    {
            //        TemplateApi.SendTemplateMessage(
            //            SettingManager.GetSettingValueForApplication(AppSettingNames.WechatMP.AppId),
            //            openId,
            //            SettingManager.GetSettingValueForApplication(AppSettingNames.WechatMP.WarningTemplateMessage),
            //            url,
            //            new
            //            {
            //                first = new TemplateDataItem(first, "#586c94"),
            //                keyword1 = new TemplateDataItem(content, "#000000"),
            //                keyword2 = new TemplateDataItem($"{time.Value.ToString("yyyy-MM-dd HH:mm:ss")}", "#000000"),
            //                keyword3 = new TemplateDataItem(target, "#000000"),
            //                remark = new TemplateDataItem(remark, "#000000")
            //            });
            //    }
            //    catch (Exception e)
            //    {
            //        Logger.Error("微信模板消息发送失败，openid：" + openId, e);
            //    }
            //}
        }
    }
}
