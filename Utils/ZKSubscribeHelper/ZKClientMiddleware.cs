using Mango.Wcf.Util.ConfigCenter;
using Mango.Wcf.Util.Configs;
using Mango.Wcf.Util.Ioc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZKSubscribeHelper
{
    public static class ZKClientMiddleware
    {
        private static ZKSetting _zkConfig = new ZKSetting();
        public static void AddZKClient(this IServiceCollection services, IConfigurationSection ccConfig = null, IConfigurationSection zkConfig = null)
        {
            zkConfig.Bind(_zkConfig);
            ccConfig.Bind(ConfigManager.CCConfig);
            services.Configure<Config>(ccConfig);
            services.AutoRegisterService();
            services.Configure<ZKSetting>(zkConfig);
        }

        public static IApplicationBuilder UseZKClient(this IApplicationBuilder app)
        {
            ZKClient zkclient = new ZKClient(_zkConfig);
            zkclient.Init();
            return app.Use(async (context, next) =>
            {
                await next();
            });

            //return app.Use(async (context, next) =>
            //{
            //   await next();
            //});

            //return app.Use(next =>
            //{
            //    return context =>
            //    {
            //        return next.Invoke(context);
            //    };
            //});
        }
    }
}
