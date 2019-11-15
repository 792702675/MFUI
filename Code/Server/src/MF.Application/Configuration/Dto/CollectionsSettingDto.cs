using Abp.Zero.Configuration;
using MF.OSSObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF.Configuration.Dto
{
    [Tab("收藏品基础配置", 5)]
    public class CollectionsSettingDto //: ISettingDto
    {

        [DisplayName("默认的解锁值")]
        [Key(AppSettingNames.Collections.DefalutUnlockValue)]
        public int DefalutUnlockValue { get; set; }

        [DisplayName("默认的帧延时（ms）")]
        [Key(AppSettingNames.Collections.DefalutFrameDelay)]
        public int DefalutFrameDelay { get; set; }

    }
}
