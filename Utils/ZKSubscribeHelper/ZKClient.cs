
using log4net;
using Mango.NodisClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZKSubscribeHelper
{
    public class ZKClient
    {
        //ILog _logger = LogManager.GetLogger(typeof(ZKClient));
        ILogger<ZKClient> _logger;
        public ZKSetting _zksetting;
        public ZooKeeperHelper zkhelper;
        CCClientHelper _ccGHelper;
        public ZKClient(IOptions<ZKSetting> zksetting, ILogger<ZKClient> logger, CCClientHelper ccGHelper)
        {
            _logger = logger;
            if (zksetting != null)
            {
                _zksetting = zksetting.Value;
            }
            _ccGHelper = ccGHelper;
        }
        public void Init()
        {
            _logger.LogInformation($"开始监听zk<jodis/{_zksetting.zkProxyDir}>节点");
            zkhelper = new ZooKeeperHelper(_zksetting.zkAddr, _zksetting.zkProxyDir, _zksetting.zkSessionTimeout,
               (nodes) =>
               {
                   _logger.LogInformation($"新增节点");
                   foreach (var item in nodes)
                   {
                       _logger.LogInformation($"新增节点：{item.Addr}");
                   }

                   _ = CheckCCAsync();
               },
               (nodes) =>
               {
                   _logger.LogInformation($"删除节点");
                   foreach (var item in nodes)
                   {
                       _logger.LogInformation($"删除节点：{item.Addr}");
                   }

                   _ = CheckCCAsync();
               });
        }

        public async Task CheckCCAsync()
        {
            bool chageTag = false;
            var oldVal = _ccGHelper.GetCodisProxyValue().Split(',').ToList();
            var newVal = zkhelper.pools.Select(a => a.Addr).ToList();
            _logger.LogInformation($"CC的的原有值:{string.Join(",", oldVal)};zk中最新的值:{string.Join(",", newVal)}");

            var listUnion = oldVal.Union(newVal);
            var listExp1 = listUnion.Except(oldVal).ToList();
            if (listExp1.Count > 0)
            {
                //增加了节点
                _logger.LogInformation($"zk中新增了节点:{string.Join(",", listExp1)}");
                chageTag = true;
            }
            var listExp2 = listUnion.Except(newVal).ToList();
            if (listExp2.Count > 0)
            {
                //删除了节点
                _logger.LogInformation($"zk中删除了节点:{string.Join(",", listExp2)}");
                chageTag = true;
            }

            if (chageTag)
            {
                //更新内容
                var newValStr = string.Join(",", newVal);
                _logger.LogInformation($"更新CC中的值为：{newValStr}");
                await _ccGHelper.SetCodisProxyValueAsync(newValStr);
            }
            else
            {
                _logger.LogInformation("CC的值与zk中的节点一致，无需更新");
            }

        }
    }
}
