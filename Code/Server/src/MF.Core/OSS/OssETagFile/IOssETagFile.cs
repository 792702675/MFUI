using System;
using System.Collections.Generic;
using System.Text;

namespace MF.OSS
{
    public interface IOssETagFile
    {
        string BucketName { get; set; }
        string ETag { get; set; }
        string Url { get; }
    }
}
