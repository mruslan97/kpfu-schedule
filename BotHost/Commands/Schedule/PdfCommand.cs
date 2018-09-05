using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotHost.Commands.CommandsArgs;
using BotHost.Tools;
using Telegram.Bot.Framework;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace BotHost.Commands.Schedule
{
    public class PdfCommand : CommandBase<DefaultCommandArgs>
    {
        private PdfGenerator _pdfGenerator;
        public PdfCommand(PdfGenerator pdfGenerator) : base(name: "pdf")
        {
            _pdfGenerator = pdfGenerator;
        }
        protected override bool CanHandleCommand(Update update)
        {
            if (!base.CanHandleCommand(update))
            {
                return update.Message.Text.ToLowerInvariant().Contains("pdf");
            }
            return true;
        }

        public override async Task<UpdateHandlingResult> HandleCommand(Update update, DefaultCommandArgs args)
        {
            var pdfStream = await _pdfGenerator.GetPdf(update.Message.Chat.Id);                      
            var pdfDoc = new InputOnlineFile(pdfStream, "Расписание.pdf");
            await Bot.Client.SendDocumentAsync(update.Message.Chat.Id, pdfDoc);
            return UpdateHandlingResult.Handled;
        }
    }
    
}
