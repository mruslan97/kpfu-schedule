using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
        private static readonly TelegramBotClient Bot = new TelegramBotClient("349393552:AAGKVaWpgZb_Zyjfbz5wfrcxj3QbuIaEvRw");
        private HtmlToPdf _converterHtmlToPdf;
        private HtmlToImage _converterHtmlToImage;
        private Dictionary<long,string> _users;

        public MessageHandler()
        {
            _converterHtmlToPdf = new HtmlToPdf();
            _converterHtmlToImage = new HtmlToImage();
            _users = new Dictionary<long, string>();
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
                case "на сегодня.":
                    TodayAnswer();
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
                    if(message.Text.Length == 6 || message.Text.Length == 8)
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
        }

        private async void VerificationAnswer(string group, long chatId)
        {
            if(!_users.ContainsKey(chatId))
            _users.Add(chatId,group);
            else
            {
                _users[chatId] = group;
            }
            var keyboard = new ReplyKeyboardMarkup(new[]
            {
                new[] {new KeyboardButton("На сегодня")},
                new[] {new KeyboardButton("Получить в png")},
                new[] {new KeyboardButton("Получить в pdf")},
                new[] {new KeyboardButton("Смена группы")}});
            await Bot.SendTextMessageAsync(chatId, $"Группа сохранена.", replyMarkup: keyboard);
        }

        private async void GetPdf(long chatId)
        {
            var group = _users[chatId];
            var doc = _converterHtmlToPdf.ConvertUrl($"https://kpfu.ru/week_sheadule_print?p_group_name={group}");
            doc.Save($"tmpPdf/{chatId}.pdf");
            var fs = new MemoryStream(File.ReadAllBytes($"tmpPdf/{chatId}.pdf"));
            var fileToSend = new FileToSend($"Расписание.pdf", fs);
            await Bot.SendDocumentAsync(chatId, fileToSend, "Лови расписание ;)");
            var file = new FileInfo($"tmpPdf/{chatId}.pdf");
            file.Delete();
        }

        private async void GetPng(long chatId)
        {
            var group = _users[chatId];
            var image = _converterHtmlToImage.ConvertUrl($"https://kpfu.ru/week_sheadule_print?p_group_name={group}");
            var quantizer = new WuQuantizer();
            using (var quantized = quantizer.QuantizeImage(new Bitmap(image)))
            {
                quantized.Save($"tmpPng/{chatId}.png", ImageFormat.Png);
            }
            var fs = new MemoryStream(File.ReadAllBytes($"tmpPng/{chatId}.png"));
            var fileToSend = new FileToSend($"Расписание.png", fs);
            await Bot.SendPhotoAsync(chatId, fileToSend, "Лови расписание ;)");
            var file = new FileInfo($"tmpPng/{chatId}.png");
            file.Delete();
        }

        private async void ChangeGroupAnswer(long chatId)
        {
            await Bot.SendTextMessageAsync(chatId, "Отправь номер новой группы");
        }

        private void TodayAnswer()
        {
            
        }
    }
}
