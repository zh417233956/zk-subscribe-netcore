
using log4net;
using Mango.NodisClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ZKSubscribeHelper
{
    public class ZKClient
    {
        public ZooKeeperHelper zkhelper;
        ILog log = LogManager.GetLogger(typeof(ZKClient));
        ZKSetting _zksetting;
        public ZKClient(ZKSetting zksetting)
        {
            _zksetting = zksetting;
        }
        public void Init()
        {
            log.Info($"开始监听zk<jodis/{_zksetting.zkProxyDir}>节点");
            zkhelper = new ZooKeeperHelper(_zksetting.zkAddr, _zksetting.zkProxyDir, _zksetting.zkSessionTimeout,
               (nodes) =>
               {
                   foreach (var item in nodes)
                   {
                       log.Info($"新增节点：{item.Addr}");
                   }
               },
               (nodes) =>
               {
                   foreach (var item in nodes)
                   {
                       log.Info($"删除节点：{item.Addr}");
                   }
               });


            var pools = zkhelper.pools;

            log.Info($"jodis/{_zksetting.zkProxyDir}节点：{ string.Join(",", pools.Select(s => s.Addr).ToList()) }");
        }
    }
}
