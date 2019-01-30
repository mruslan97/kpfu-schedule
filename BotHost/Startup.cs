using System;
using System.Threading.Tasks;
using BotHost.Commands;
using BotHost.Commands.Schedule;
using BotHost.Models;
using BotHost.Quartz;
using BotHost.Services;
using BotHost.Services.Impl;
using BotHost.Tools;
using BotHost.Vk;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SelectPdf;
using Telegram.Bot.Framework;
using Telegram.Bot.Framework.Abstractions;
using VkNet;
using VkNet.Abstractions;
using VkNet.Model;

namespace BotHost
{
    public class Startup
    {
        private readonly IConfigurationRoot _configuration;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();

            _configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            services.AddSingleton<IVkApi>(sp =>
            {
                var api = new VkApi(services);

                api.Authorize(new ApiAuthParams
                {
                    AccessToken = _configuration.GetSection("VkToken").Get<string>()
                });

                return api;
            });
            services.Configure<VkOptions>(Configuration.GetSection(nameof(VkOptions)));
            services.AddDbContext<UsersContext>(options =>
                options.UseNpgsql(connectionString));
            services.AddHttpClient();
            services.AddTransient<IHtmlParser, HtmlParser>();
            services.AddTransient<IImageGenerator,ImageGenerator>();
            services.AddTransient<IPdfGenerator, PdfGenerator>();
            services.AddTransient<HtmlToImage>();
            services.AddTransient<HtmlToPdf>();
            services.AddTransient<Cache>();
            services.AddTransient<MessageKeyboardBuilder>();
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
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            logger.LogInformation("Bot up");
            if (_configuration.GetSection("UseTelegram").Get<bool>())
                if (_configuration.GetSection("UseWebHook").Get<bool>())
                    app.UseTelegramBotWebhook<KpfuScheduleBot>();
                else
                    Task.Factory.StartNew(async () =>
                    {
                        var botManager = app.ApplicationServices.GetRequiredService<IBotManager<KpfuScheduleBot>>();
                        await botManager.SetWebhookStateAsync(false);
                        while (true)
                            try
                            {
                                await botManager.GetAndHandleNewUpdatesAsync();
                            }
                            catch (Exception e)
                            {
                                logger.LogError($"Exception: {e}");
                            }
                    }).ContinueWith(t =>
                    {
                        if (t.IsFaulted) throw t.Exception;
                    });
            app.UseQuartz();
            if (_configuration.GetSection("UseVk").Get<bool>())
                app.UseMvc();
            logger.LogInformation("Configuration ended");
        }
    }
}