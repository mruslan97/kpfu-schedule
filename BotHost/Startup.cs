using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotHost.Commands;
using BotHost.Commands.Schedule;
using BotHost.Models;
using BotHost.Quartz;
using BotHost.Tools;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using SelectPdf;
using Telegram.Bot.Framework;
using Telegram.Bot.Framework.Abstractions;

namespace BotHost
{
    public class Startup
    {
        private IConfigurationRoot _configuration;
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            _configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<UsersContext>(options =>
                options.UseNpgsql(connectionString));
            services.AddHttpClient();
            services.AddTransient<HtmlParser>();
            services.AddTransient<HtmlToImage>();
            services.AddTransient<HtmlToPdf>();
            services.AddTransient<ImageGenerator>();
            services.AddTransient<PdfGenerator>();
            services.AddTransient<Cache>();
            services.AddQuartz(typeof(ScheduledJob));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddTelegramBot<KpfuScheduleBot>(_configuration.GetSection("ScheduleBot"))
                .AddUpdateHandler<SetupGroupCommand>()
                .AddUpdateHandler<StartCommand>()
                .AddUpdateHandler<TodayCommand>()
                .AddUpdateHandler<TomorrowCommand>()
                .AddUpdateHandler<WeekCommand>()
                .AddUpdateHandler<PdfCommand>()
                .AddUpdateHandler<HelpCommand>()
                .Configure();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            ILogger logger = loggerFactory.CreateLogger<Startup>();
            logger.LogInformation("Configuration started");
            logger.LogInformation($"Env: {env.EnvironmentName}");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            logger.LogInformation("Bot up");

            if (_configuration.GetSection("UseWebHook").Get<bool>())
            {
                app.UseTelegramBotWebhook<KpfuScheduleBot>();
            }
            else
            {
                Task.Factory.StartNew(async () =>
                {
                    var botManager = app.ApplicationServices.GetRequiredService<IBotManager<KpfuScheduleBot>>();
                    await botManager.SetWebhookStateAsync(false);
                    while (true)
                    {
                        try
                        {
                            await botManager.GetAndHandleNewUpdatesAsync();
                        }
                        catch (Exception e)
                        {
                            logger.LogError($"Exception: {e}");
                        }
                    }
                }).ContinueWith(t =>
                {
                    if (t.IsFaulted) throw t.Exception;
                });
            }
            logger.LogInformation("Set up bot to notifier");
            //set up bot for notifier to fix DI-loop
            //var notifier = (Notificator)app.ApplicationServices.GetRequiredService<INotifiactionSender>();
            //notifier.Bot = app.ApplicationServices.GetRequiredService<ItisScheduleBot>();
            //run scheduled updates
            logger.LogInformation("Run schedules updating");
            //updates = new UpdatesScheduler(app.ApplicationServices);
            //updates.Start();

            //app.Run(async (context) => { });
            app.UseQuartz();
            app.UseMvc();
            logger.LogInformation("Configuration ended");
        }
    }
}
