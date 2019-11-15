using Abp.Domain.Repositories;
using Abp.Application.Services.Dto;
using System.Collections.Generic;
using MF.CommonDto;
using System.Linq;
using Aliyun.OSS;
using Abp.AutoMapper;
using MF.Authorization.Users;

namespace MF.Enums
{
    /// <summary>
    /// 枚举下拉选择
    /// </summary>
    public class EnumAppService : MFAppServiceBase
    {



        public IEnumerable<NameValueDto> GetProvinces()
        {
            return new List<NameValueDto>()
            {
                new NameValueDto("北京市"              ,"北京市"               ),
                new NameValueDto("天津市"              ,"天津市"               ),
                new NameValueDto("河北省"              ,"河北省"               ),
                new NameValueDto("山西省"              ,"山西省"               ),
                new NameValueDto("内蒙古自治区"        ,"内蒙古自治区"         ),
                new NameValueDto("辽宁省"              ,"辽宁省"               ),
                new NameValueDto("吉林省"              ,"吉林省"               ),
                new NameValueDto("黑龙江省"            ,"黑龙江省"             ),
                new NameValueDto("上海市"              ,"上海市"               ),
                new NameValueDto("江苏省"              ,"江苏省"               ),
                new NameValueDto("浙江省"              ,"浙江省"               ),
                new NameValueDto("安徽省"              ,"安徽省"               ),
                new NameValueDto("福建省"              ,"福建省"               ),
                new NameValueDto("江西省"              ,"江西省"               ),
                new NameValueDto("山东省"              ,"山东省"               ),
                new NameValueDto("河南省"              ,"河南省"               ),
                new NameValueDto("湖北省"              ,"湖北省"               ),
                new NameValueDto("湖南省"              ,"湖南省"               ),
                new NameValueDto("广东省"              ,"广东省"               ),
                new NameValueDto("广西壮族自治区"      ,"广西壮族自治区"       ),
                new NameValueDto("海南省"              ,"海南省"               ),
                new NameValueDto("重庆市"              ,"重庆市"               ),
                new NameValueDto("四川省"              ,"四川省"               ),
                new NameValueDto("贵州省"              ,"贵州省"               ),
                new NameValueDto("云南省"              ,"云南省"               ),
                new NameValueDto("西藏自治区"          ,"西藏自治区"           ),
                new NameValueDto("陕西省"              ,"陕西省"               ),
                new NameValueDto("甘肃省"              ,"甘肃省"               ),
                new NameValueDto("青海省"              ,"青海省"               ),
                new NameValueDto("宁夏回族自治区"      ,"宁夏回族自治区"       ),
                new NameValueDto("新疆维吾尔自治区"    ,"新疆维吾尔自治区"     ),
                new NameValueDto("台湾省"              ,"台湾省"               ),
                new NameValueDto("香港特别行政区"      ,"香港特别行政区"       ),
                new NameValueDto("澳门特别行政区"      ,"澳门特别行政区"       ),
            };
        }
        public IEnumerable<NameValueDto<int>> GetOSSStorageClass()
        {
            yield return new NameValueDto<int>("标准存储", (int)StorageClass.Standard);
            yield return new NameValueDto<int>("低频访问", (int)StorageClass.IA);
            yield return new NameValueDto<int>("归档存储", (int)StorageClass.Archive);
        }
        public IEnumerable<NameValueDto<int>> GetOSSCannedAccessControlList()
        {
            yield return new NameValueDto<int>("私有", (int)CannedAccessControlList.Private);
            yield return new NameValueDto<int>("公共读", (int)CannedAccessControlList.PublicRead);
            yield return new NameValueDto<int>("公共读写", (int)CannedAccessControlList.PublicReadWrite);
        }


        /// <summary>
        /// 用户类型
        /// </summary>
        /// <returns></returns>
        public IEnumerable<NameValueDto<int>> GetUserType()
        {
            return EnumToNameValue.EnumToNameValueDto<RoleType>();
        }

    }
}

