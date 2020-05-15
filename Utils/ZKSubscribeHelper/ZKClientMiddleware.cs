using Mango.Wcf.Util.ConfigCenter;
using Mango.Wcf.Util.Configs;
using Mango.Wcf.Util.Ioc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace ZKSubscribeHelper
{
    public class ZKClientMiddleware : IMiddleware
    {
        ILogger<ZKClientMiddleware> _logger;
        ZKClient _zkclient;
        CCClientHelper _ccGHelper;
        public ZKClientMiddleware(ZKClient zkclient, ILogger<ZKClientMiddleware> logger, CCClientHelper ccGHelper)
        {
            _logger = logger;
            _zkclient = zkclient;
            _zkclient.Init();
            _ = _zkclient.CheckCCAsync();
            _ccGHelper = ccGHelper;
        }

        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.Request.Path.Value.Equals("/"))
            {
                var message = $"监听jodis/{_zkclient._zksetting.zkProxyDir}节点<br />当前节点值：{ string.Join(",", _zkclient.zkhelper.pools.Select(s => s.Addr).ToList()) }";

                message += $"<br />CC的当前值为：{_ccGHelper.GetCodisProxyValue()}";

                _logger.LogInformation(message.Replace("<br />",";"));
                context.Response.ContentType = "text/html;charset=utf-8";
                return context.Response.WriteAsync(message);
            }
            else if (context.Request.Path.Value.Equals("/getpools"))
            {
                var message = $"当前jodis/{_zkclient._zksetting.zkProxyDir}节点：{ string.Join(",", _zkclient.zkhelper.pools.Select(s => s.Addr).ToList()) }";
                message += $";CC的当前值为：{_ccGHelper.GetCodisProxyValue()}";
                _logger.LogInformation(message);
                context.Response.ContentType = "text/plain;charset=utf-8";
                return context.Response.WriteAsync(message);
            }
            else if (context.Request.Path.Value.Equals("/update"))
            {
                _ = _zkclient.CheckCCAsync();
                var message = $"手动更新CC成功;当前jodis/{_zkclient._zksetting.zkProxyDir}节点：{ string.Join(",", _zkclient.zkhelper.pools.Select(s => s.Addr).ToList()) }";
                message += $";CC的当前值为：{_ccGHelper.GetCodisProxyValue()}";
                _logger.LogInformation(message);
                context.Response.ContentType = "text/plain;charset=utf-8";
                return context.Response.WriteAsync(message);
            }
            else if (context.Request.Path.Value.ToLower().Equals("/mangoapi/configcentercall"))
            {
                string key = context.Request.Query["key"];
                string value = context.Request.Query["value"];
                var message = $"CC回调通知,key={key},vaule={value};";
                _logger.LogInformation(message);
                context.Response.ContentType = "text/plain;charset=utf-8";
                return context.Response.WriteAsync("100");
            }
            else
            {
                return next.Invoke(context);
            }
        }

        #region 多种实现形式
        //private readonly Microsoft.AspNetCore.Http.RequestDelegate _next;

        ////在应用程序的生命周期中，中间件的构造函数只会被调用一次
        //public XfhMiddleware(Microsoft.AspNetCore.Http.RequestDelegate next)
        ////{
        //    this._next = next;
        //}

        //public async System.Threading.Tasks.Task InvokeAsync(Microsoft.AspNetCore.Http.HttpContext context)
        //{
        //    // Do something...
        //    await _next(context);
        //}

        //public static IApplicationBuilder UseZKClient(this IApplicationBuilder app)
        //{           
        //    return app.Use(async (context, next) =>
        //    {
        //        await next();
        //    });

        //    //return app.Use(async (context, next) =>
        //    //{
        //    //   await next();
        //    //});

        //    //return app.Use(next =>
        //    //{
        //    //    return context =>
        //    //    {
        //    //        return next.Invoke(context);
        //    //    };
        //    //});
        //}
        #endregion 多种实现形式
    }
    public static class ZKMiddlewareExtension
    {
        private static ZKSetting _zkConfig = new ZKSetting();
        public static void AddZKMiddleware(this IServiceCollection services, IConfigurationSection ccConfig = null, IConfigurationSection zkConfig = null)
        {
            // 使用UseMiddleware将自定义中间件添加到请求处理管道中
            services.AddSingleton<ZKClientMiddleware>();
            services.AddSingleton<ZKClient>();
            services.AddTransient<CCClientHelper>();
            zkConfig.Bind(_zkConfig);
            ccConfig.Bind(ConfigManager.CCConfig);
            services.Configure<Config>(ccConfig);
            services.AutoRegisterService();
            services.Configure<ZKSetting>(zkConfig);
        }
        public static IApplicationBuilder UseZKMiddleware(this IApplicationBuilder builder)
        {
            // 使用UseMiddleware将自定义中间件添加到请求处理管道中
            return builder.UseMiddleware<ZKClientMiddleware>();
        }
    }
}
