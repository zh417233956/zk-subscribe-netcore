using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging((context, loggingBuilder) =>
                {
                    loggingBuilder.AddFilter("System", LogLevel.Warning);
                    loggingBuilder.AddFilter("Microsoft", LogLevel.Warning);
                    var path = System.IO.Directory.GetCurrentDirectory();
                    loggingBuilder.AddLog4Net($"{path}/Configs/log4net.config");
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
