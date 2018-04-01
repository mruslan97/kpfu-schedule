using System;
using System.Configuration;
using System.Data.Entity;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using kpfu_schedule.Models;
using NLog;
using SelectPdf;
using Telegram.Bot;
using Telegram.Bot.Types;
using File = System.IO.File;

namespace kpfu_schedule.Tools
{
    public class ImageGenerator
    {
        private static readonly TelegramBotClient Bot =
            new TelegramBotClient(ConfigurationManager.AppSettings["BotToken"]);

        private readonly HtmlToImage _converterHtmlToImage;
        private readonly HtmlParser _htmlParser;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public ImageGenerator()
        {
            _converterHtmlToImage = new HtmlToImage();
            _htmlParser = new HtmlParser();
        }

        public async void GetDay(long chatId, bool isToday)
        {
            try
            {
                var day = Convert.ToInt32(DateTime.Today.DayOfWeek);
                if (day == 6 && !isToday || day == 0 && isToday)
                {
                    await Bot.SendTextMessageAsync(chatId, "Выходной день");
                    return;
                }

                day = isToday ? day : day + 1;
                var group = "";
                using (var db = new TgUsersContext())
                {
                    var user = await db.Users.SingleOrDefaultAsync(u => u.ChatId == chatId);
                    group = user.Group;
                }

                if (!File.Exists($"tmpPng/{group}{isToday}.png"))
                {
                    var httpClient = new HttpClient();
                    var htmlPage =
                        await httpClient.GetStringAsync($"https://kpfu.ru/week_sheadule_print?p_group_name={group}");
                    var htmlDocument = _htmlParser.ParseDay(htmlPage, day);
                    _converterHtmlToImage.WebPageWidth = 600;
                    var image = _converterHtmlToImage.ConvertHtmlString(htmlDocument);
                    image.Save($"tmpPng/{group}{isToday}.png", ImageFormat.Png);
                    image.Dispose();
                }

                var fs = new MemoryStream(File.ReadAllBytes($"tmpPng/{group}{isToday}.png"));
                var fileToSend = new FileToSend($"Расписание.png", fs);
                await Bot.SendPhotoAsync(chatId, fileToSend);
            }
            catch (Exception e)
            {
                _logger.Error($"{e.Message} / chatId: {chatId}");
            }
        }

        public async void GetWeek(long chatId)
        {
            try
            {
                var currentConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var group = "";
                using (var db = new TgUsersContext())
                {
                    var user = await db.Users.SingleOrDefaultAsync(u => u.ChatId == chatId);
                    group = user.Group;
                }

                if (!File.Exists($"tmpPng/{group}week.png"))
                {
                    var image = _converterHtmlToImage.ConvertUrl(
                        $"https://kpfu.ru/week_sheadule_print?p_group_name={group}");
                    image.Save($"tmpPng/{group}week.png", ImageFormat.Png);
                    image.Dispose();
                }

                var fs = new MemoryStream(File.ReadAllBytes($"tmpPng/{group}week.png"));
                var fileToSend = new FileToSend($"Расписание.png", fs);
                await Bot.SendPhotoAsync(chatId, fileToSend,
                    $"Номер недели: {currentConfig.AppSettings.Settings["WeekNumber"].Value}, тип: {currentConfig.AppSettings.Settings["WeekType"].Value}");
            }
            catch (Exception e)
            {
                _logger.Error($"{e.Message} / chatId: {chatId}");
            }
        }
    }
}