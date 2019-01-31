using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace BotHost
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Directory.CreateDirectory(Directory.GetParent(Assembly.GetExecutingAssembly().Location) + "/logs");
            Directory.CreateDirectory(Directory.GetParent(Assembly.GetExecutingAssembly().Location) + "/logs/messages");
            Directory.CreateDirectory(Directory.GetParent(Assembly.GetExecutingAssembly().Location) + "/tmpPng");
            Directory.CreateDirectory(Directory.GetParent(Assembly.GetExecutingAssembly().Location) + "/tmpPdf");
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseNLog()
                .UseStartup<Startup>();
        }
    }
}
