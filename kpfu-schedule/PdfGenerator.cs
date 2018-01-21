using System.Data.Entity;
using System.IO;
using kpfu_schedule.Models;
using SelectPdf;
using Telegram.Bot;
using Telegram.Bot.Types;
using File = System.IO.File;

namespace kpfu_schedule
{
    public class PdfGenerator
    {
        private static readonly TelegramBotClient Bot =
            new TelegramBotClient("444905366:AAG9PlFd" +
                                  "6ZusE3hPO_sGETGPhzgM_e7roZg");

        private readonly HtmlToPdf _converterHtmlToPdf;

        public PdfGenerator()
        {
            _converterHtmlToPdf = new HtmlToPdf();
        }

        public async void GetPdf(long chatId)
        {
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
            await Bot.SendDocumentAsync(chatId, fileToSend);
            var file = new FileInfo($"tmpPdf/{chatId}.pdf");
            file.Delete();
        }
    }
}