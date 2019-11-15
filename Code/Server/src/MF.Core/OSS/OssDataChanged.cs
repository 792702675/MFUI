using Abp.Events.Bus;
using System;
using System.Collections.Generic;
using System.Text;

namespace MF.OSS
{
    public class OssDataChanged:EventData
    {
        public string BucketName { get; }
        public OssDataChanged(string bucketName)
        {
            BucketName = bucketName;
        }
    }
}
