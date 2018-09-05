using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotHost.Commands.CommandsArgs;
using BotHost.Models;
using BotHost.Tools;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Framework;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace BotHost.Commands.Schedule
{
    public class PdfCommand : CommandBase<DefaultCommandArgs>
    {
        private PdfGenerator _pdfGenerator;
        private UsersContext _usersContext;
        public PdfCommand(PdfGenerator pdfGenerator, UsersContext usersContext) : base(name: "pdf")
        {
            _pdfGenerator = pdfGenerator;
            _usersContext = usersContext;
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
            var user = await _usersContext.TgUsers.SingleOrDefaultAsync(u => u.ChatId == update.Message.Chat.Id);
            if (user == null)
            {
                await Bot.Client.SendTextMessageAsync(update.Message.Chat.Id, "Нет данных в базе, напиши /start");
                return UpdateHandlingResult.Handled;
            }
            var pdfStream = await _pdfGenerator.GetPdf(user.Group);                      
            var pdfDoc = new InputOnlineFile(pdfStream, "Расписание.pdf");
            await Bot.Client.SendDocumentAsync(update.Message.Chat.Id, pdfDoc);
            return UpdateHandlingResult.Handled;
        }
    }
    
}
