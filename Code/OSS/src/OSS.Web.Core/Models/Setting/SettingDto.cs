using System;
using System.Collections.Generic;
using System.Text;

namespace OSS.Models.Setting
{
    public class SettingDto
    {
        public RefererRule RefererRule { get; set; } = new RefererRule();
        public CORSRule CORSRule { get; set; } = new CORSRule();
    }


    public class CORSRule
    {

        //
        // 摘要:
        //     Allowed origins. One origin could contain at most one wildcard (*).
        public IList<string> AllowedOrigins { get; set; }
        //
        // 摘要:
        //     Allowed HTTP Method. Valid values are GET,PUT,DELETE,POST,HEAD. This property
        //     is to specify the value of Access-Control-Allow-Methods header in the preflight
        //     response. It means the allowed methods in the actual CORS request.
        public IList<string> AllowedMethods { get; set; }
        //
        // 摘要:
        //     Get or set Allowed Headers. This property is to specify the value of Access-Control-Allowed-Headers
        //     in the preflight response. It defines the allowed headers in the actual CORS
        //     request. Each allowed header can have up to one wildcard (*).
        public IList<string> AllowedHeaders { get; set; }
        //
        // 摘要:
        //     Get or set exposed headers in the CORS response. Wildcard(*) is not allowed.
        //     This property is to specify the value of Access-Control-Expose-Headers in the
        //     preflight response.
        public IList<string> ExposeHeaders { get; set; }
        //
        // 摘要:
        //     HTTP Access-Control-Max-Age's getter and setter, in seconds. The Access-Control-Max-Age
        //     header indicates how long the results of a preflight request (OPTIONS) can be
        //     cached in a preflight result cache. The max value is 999999999.
        public int MaxAgeSeconds { get; set; }
    }

    public class RefererRule
    {
        //
        // 摘要:
        //     Gets the flag of allowing empty referer.
        public bool AllowEmptyReferer { get; set; }
        //
        // 摘要:
        //     Gets the referer list.
        public IList<string> RefererList { get; set; }
    }

}
