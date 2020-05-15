using Mango.Wcf.Util.ConfigCenter;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ZKSubscribeHelper
{
    /// <summary>
    /// CC帮助类
    /// </summary>
    public class CCClientHelper
    {
        ICCHelper _ccHelper;
        ILogger<CCClientHelper> _logger;
        ZKSetting _zksetting;
        public CCClientHelper(ICCHelper ccHelper, IOptions<ZKSetting> zksetting, ILogger<CCClientHelper> logger)
        {
            _ccHelper = ccHelper;
            _logger = logger;
            if (zksetting != null)
            {
                _zksetting = zksetting.Value;
            }
        }

        public string GetCodisProxyValue()
        {
            string key = "Codis_codis-mango_proxy_addr";
            var config = _ccHelper.GetConfig();
            string values = _ccHelper.GetConfig(key, config.SingleKey);
            return values;
        }

        public async Task<string> SetCodisProxyValueAsync(string value)
        {
            var result = "";
            var postUrl = new Uri(_zksetting.ccPushHost + "/mangoapi/a9085_ConfigCenter.Data/CodisProxyHelper/ChangeCodisValue");
            var postData = new
            {
                Environment = _zksetting.environment,
                ConfigValue = value
            };
            _logger.LogInformation($"更新CC请求内容:{Newtonsoft.Json.JsonConvert.SerializeObject(postData)}");
            //post请求
            result = await Mango.Wcf.Util.Utilitys.HttpClientHelper.HttpPostAsync(postUrl.ToString(), Newtonsoft.Json.JsonConvert.SerializeObject(postData), "application/json");
            var resModel = new { RequestId = "", ResultCode = 0, ResultMsg = "" };
            resModel = Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(result, resModel);
            _logger.LogInformation($"更新CC响应结果:{Newtonsoft.Json.JsonConvert.SerializeObject(resModel)}");
            return result;
        }


    }
}
