
using log4net;
using Mango.NodisClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ZKSubscribeHelper
{
    public class ZKClient
    {
        //ILog _logger = LogManager.GetLogger(typeof(ZKClient));
        ILogger<ZKClient> _logger;
        public ZKSetting _zksetting;
        public ZooKeeperHelper zkhelper;

        public ZKClient(IOptions<ZKSetting> zksetting, ILogger<ZKClient> logger)
        {
            _logger = logger;
            if (zksetting != null)
            {
                _zksetting = zksetting.Value;
            }
        }
        public void Init()
        {
            _logger.LogInformation($"开始监听zk<jodis/{_zksetting.zkProxyDir}>节点");
            zkhelper = new ZooKeeperHelper(_zksetting.zkAddr, _zksetting.zkProxyDir, _zksetting.zkSessionTimeout,
               (nodes) =>
               {
                   foreach (var item in nodes)
                   {
                       _logger.LogInformation($"新增节点：{item.Addr}");
                   }
               },
               (nodes) =>
               {
                   foreach (var item in nodes)
                   {
                       _logger.LogInformation($"删除节点：{item.Addr}");
                   }
               });
        }
    }
}
