using System;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using kpfu_schedule.Models;
using NLog;
using SelectPdf;
using Telegram.Bot;
using Telegram.Bot.Types;
using File = System.IO.File;

namespace kpfu_schedule.Tools
{
    public class PdfGenerator
    {
        private static readonly TelegramBotClient Bot =
            new TelegramBotClient(ConfigurationManager.AppSettings["BotToken"]);

        private readonly HtmlToPdf _converterHtmlToPdf;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public PdfGenerator()
        {
            _converterHtmlToPdf = new HtmlToPdf();
        }

        public async void GetPdf(long chatId)
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
                var doc = _converterHtmlToPdf.ConvertUrl($"https://kpfu.ru/week_sheadule_print?p_group_name={group}");
                doc.Save($"tmpPdf/{chatId}.pdf");
                var fs = new MemoryStream(File.ReadAllBytes($"tmpPdf/{chatId}.pdf"));
                var fileToSend = new FileToSend($"Расписание.pdf", fs);
                await Bot.SendDocumentAsync(chatId, fileToSend, $"Номер недели: {currentConfig.AppSettings.Settings["WeekNumber"].Value}, тип: {currentConfig.AppSettings.Settings["WeekType"].Value}");
                var file = new FileInfo($"tmpPdf/{chatId}.pdf");
                file.Delete();
            }
            catch (Exception e)
            {
                _logger.Error($"{e.Message} / chatId : {chatId}");
            }
        }
    }
}