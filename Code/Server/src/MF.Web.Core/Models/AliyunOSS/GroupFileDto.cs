using MF.OSS;
using System;
using System.Collections.Generic;
using System.Text;

namespace MF.Models.AliyunOSS
{
    public class GroupFileDto
    {
        public Guid Group { get; set; }
        public List<OssETagFileDto> FileInfo { get; set; }
    }
}
