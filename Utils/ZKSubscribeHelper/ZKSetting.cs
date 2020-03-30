using System;
using System.Collections.Generic;
using System.Text;

namespace ZKSubscribeHelper
{
    public class ZKSetting
    {
        public string zkAddr { get; set; }
        public int zkSessionTimeout { get; set; }

        public string zkProxyDir { get; set; }
    }
}
