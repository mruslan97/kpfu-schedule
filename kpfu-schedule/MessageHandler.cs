using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using kpfu_schedule.Models;
using nQuant;
using SelectPdf;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using File = System.IO.File;

namespace kpfu_schedule
{
    public class MessageHandler
    {
        private static readonly TelegramBotClient Bot = new TelegramBotClient("444905366:AAG9PlFd6ZusE3hPO_sGETGPhzgM_e7roZg");
        private HtmlToPdf _converterHtmlToPdf;
        private HtmlToImage _converterHtmlToImage;
        private HtmlParser _htmlParser;

        public MessageHandler()
        {
            _converterHtmlToPdf = new HtmlToPdf();
            _converterHtmlToImage = new HtmlToImage();
            _htmlParser = new HtmlParser();
        }
        public async void SortInputMessage(Message message)
        {
            switch (message.Text.ToLower())
            {
                case "/start":
                    HelloAnswer(message.Chat);
                    break;
                case "смена группы":
                    ChangeGroupAnswer(message.Chat.Id);
                    break;
                case "на сегодня":
                    OneDayAnswer(message.Chat.Id, true);
                    break;
                case "на завтра":
                    OneDayAnswer(message.Chat.Id, false);
                    break;
                case "получить в pdf":
                    GetPdf(message.Chat.Id);
                    break;
                case "получить в png":
                    GetPng(message.Chat.Id);
                    break;
                default:
                    //var regex = new Regex(@"\d{2}-\d{3}");
                    //if (regex.IsMatch(message.Text) && message.Text.Length == 6)
                    if (message.Text.Length == 6 || message.Text.Length == 8)
                        VerificationAnswer(message.Text, message.Chat.Id);
                    else
                        await Bot.SendTextMessageAsync(message.Chat.Id,
                            "Неверный формат или данная команда отсутствует");
                    break;
            }
            // return "smth answer";
        }

        private async void HelloAnswer(Chat chat)
        {
            var sticker = new FileToSend("CAADAgADnwIAApkvSwpAf6KT3FrhngI");
            await Bot.SendStickerAsync(chat.Id, sticker);
            await Bot.SendTextMessageAsync(chat.Id,
                $"Привет, {chat.FirstName}! Введи номер своей группы в формате **-***");
            using (var db = new TgUsersContext())
            {
                if (db.Users.Find(chat.Id) != null) return;
                db.Users.Add(new TgUser
                {
                    ChatId = chat.Id,
                    Username = chat.Username,
                    FirstName = chat.FirstName,
                    LastName = chat.LastName
                });
                await db.SaveChangesAsync();
            }
        }

        private async void VerificationAnswer(string group, long chatId)
        {
            using (var db = new TgUsersContext())
            {
                var user = await db.Users.SingleOrDefaultAsync(u => u.ChatId == chatId);
                user.Group = group;
                await db.SaveChangesAsync();
            }
            var keyboard = new ReplyKeyboardMarkup(new[]
            {
                new[] {new KeyboardButton("На сегодня")},
                new[] {new KeyboardButton("На завтра")},
                new[] {new KeyboardButton("Получить в pdf")},
                new[] {new KeyboardButton("Получить в png") }});
            await Bot.SendTextMessageAsync(chatId, $"Группа сохранена.", replyMarkup: keyboard);
        }

        private async void GetPdf(long chatId)
        {
            var stopWatch = new Stopwatch();
            string group = "";
            using (var db = new TgUsersContext())
            {
                var user = await db.Users.SingleOrDefaultAsync(u => u.ChatId == chatId);
                group = user.Group;
            }
            stopWatch.Start();
            var doc = _converterHtmlToPdf.ConvertUrl($"https://kpfu.ru/week_sheadule_print?p_group_name={group}");
            Console.WriteLine($"Converted to pdf elapsed {stopWatch.Elapsed}");
            doc.Save($"tmpPdf/{chatId}.pdf");
            var fs = new MemoryStream(File.ReadAllBytes($"tmpPdf/{chatId}.pdf"));
            var fileToSend = new FileToSend($"Расписание.pdf", fs);
            await Bot.SendDocumentAsync(chatId, fileToSend);
            Console.WriteLine($"sended, elapsed {stopWatch.Elapsed}");
            var file = new FileInfo($"tmpPdf/{chatId}.pdf");
            file.Delete();
        }

        private async void GetPng(long chatId)
        {
            var stopWatch = new Stopwatch();
            string group = "";
            using (var db = new TgUsersContext())
            {
                var user = await db.Users.SingleOrDefaultAsync(u => u.ChatId == chatId);
                group = user.Group;
            }
            stopWatch.Start();
            var image = _converterHtmlToImage.ConvertUrl($"https://kpfu.ru/week_sheadule_print?p_group_name={group}");
            image.Save($"tmpPng/{chatId}.png",ImageFormat.Png);
            Console.WriteLine($"Converted to image elapsed {stopWatch.Elapsed}");
            var fs = new MemoryStream(File.ReadAllBytes($"tmpPng/{chatId}.png"));
            var fileToSend = new FileToSend($"Расписание.png", fs);
            await Bot.SendPhotoAsync(chatId, fileToSend);
            Console.WriteLine($"sended, elapsed {stopWatch.Elapsed}");
            var file = new FileInfo($"tmpPng/{chatId}.png");
            file.Delete();
        }

        private async void ChangeGroupAnswer(long chatId)
        {
            await Bot.SendTextMessageAsync(chatId, "Отправь номер новой группы");
        }

        private async void OneDayAnswer(long chatId, bool isToday)
        {
            var day = isToday 
                ? Convert.ToInt32(DateTime.Today.DayOfWeek)
                : Convert.ToInt32(DateTime.Today.DayOfWeek) + 1;
            string group = "";
            using (var db = new TgUsersContext())
            {
                var user = await db.Users.SingleOrDefaultAsync(u => u.ChatId == chatId);
                group = user.Group;
            }
            var htmlDocument = _htmlParser.GetDay(group, day);
            _converterHtmlToImage.WebPageWidth = 400;
            var image = _converterHtmlToImage.ConvertHtmlString(htmlDocument);
            image.Save($"tmpPng/{chatId}Day{isToday}.png", ImageFormat.Png);
            var fs = new MemoryStream(File.ReadAllBytes($"tmpPng/{chatId}Day{isToday}.png"));
            var fileToSend = new FileToSend($"Расписание на {DateTimeFormatInfo.CurrentInfo.DayNames[day]}.png", fs);
            await Bot.SendPhotoAsync(chatId, fileToSend);
            var file = new FileInfo($"tmpPng/{chatId}Day{isToday}.png");
            file.Delete();
        }
    }
}
