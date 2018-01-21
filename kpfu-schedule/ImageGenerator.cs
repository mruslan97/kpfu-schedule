using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kpfu_schedule.Models;
using SelectPdf;
using Telegram.Bot;
using Telegram.Bot.Types;
using File = System.IO.File;

namespace kpfu_schedule
{
    public class ImageGenerator
    {
        private static readonly TelegramBotClient Bot =
            new TelegramBotClient("444905366:AAG9PlFd" +
                                  "6ZusE3hPO_sGETGPhzgM_e7roZg");
        private HtmlToImage _converterHtmlToImage;
        private HtmlParser _htmlParser;

        public ImageGenerator()
        {
            _converterHtmlToImage = new HtmlToImage();
            _htmlParser = new HtmlParser();
        }

        public async void GetDay(long chatId,bool isToday)
        {
            var day = isToday
                ? Convert.ToInt32(DateTime.Today.DayOfWeek)
                : Convert.ToInt32(DateTime.Today.DayOfWeek) + 1;
            if (day == 6 && !isToday || day == 7)
            {
                await Bot.SendTextMessageAsync(chatId, "Выходной день");
                return;
            }
            string group = "";
            using (var db = new TgUsersContext())
            {
                var user = await db.Users.SingleOrDefaultAsync(u => u.ChatId == chatId);
                group = user.Group;
            }
            var htmlDocument = _htmlParser.GetDay(group, day);
            _converterHtmlToImage.WebPageWidth = 400;
            var image = _converterHtmlToImage.ConvertHtmlString(htmlDocument);
            var tmp = DateTime.Now.Millisecond;
            image.Save($"tmpPng/{chatId}Day{isToday}{tmp}.png", ImageFormat.Png);
            image.Dispose();
            var fs = new MemoryStream(File.ReadAllBytes($"tmpPng/{chatId}Day{isToday}{tmp}.png"));
            var fileToSend = new FileToSend($"Расписание на {DateTimeFormatInfo.CurrentInfo.DayNames[day]}.png", fs);
            await Bot.SendPhotoAsync(chatId, fileToSend);
            var file = new FileInfo($"tmpPng/{chatId}Day{isToday}.png");
            file.Delete();
        }

        public async void GetWeek(long chatId)
        {
            string group = "";
            using (var db = new TgUsersContext())
            {
                var user = await db.Users.SingleOrDefaultAsync(u => u.ChatId == chatId);
                group = user.Group;
            }
            var image = _converterHtmlToImage.ConvertUrl($"https://kpfu.ru/week_sheadule_print?p_group_name={group}");
            var tmp = DateTime.Now.Millisecond;
            image.Save($"tmpPng/{chatId}{tmp}.png", ImageFormat.Png);
            image.Dispose();
            var fs = new MemoryStream(File.ReadAllBytes($"tmpPng/{chatId}{tmp}.png"));
            var fileToSend = new FileToSend($"Расписание.png", fs);
            await Bot.SendPhotoAsync(chatId, fileToSend);
            var file = new FileInfo($"tmpPng/{chatId}.png");
            file.Delete();
        }
    }
}
