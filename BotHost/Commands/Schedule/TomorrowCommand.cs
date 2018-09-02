using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotHost.Commands.CommandsArgs;
using BotHost.Tools;
using Telegram.Bot.Framework;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;

namespace BotHost.Commands.Schedule
{
    public class TomorrowCommand : CommandBase<DefaultCommandArgs>
    {
        private ImageGenerator _imageGenerator;
        public TomorrowCommand(ImageGenerator imageGenerator) : base(name: "tomorrow")
        {
            _imageGenerator = imageGenerator;
        }
        protected override bool CanHandleCommand(Update update)
        {
            if (!base.CanHandleCommand(update))
            {
                return update.Message.Text.ToLowerInvariant().Contains("завтра");
            }
            return true;
        }

        public override async Task<UpdateHandlingResult> HandleCommand(Update update, DefaultCommandArgs args)
        {
            var image = await _imageGenerator.GetDay(update.Message.Chat.Id, false);
            if (image == null)
            {
                await Bot.Client.SendTextMessageAsync(update.Message.Chat.Id, "Выходной день");
                return UpdateHandlingResult.Handled;
            }
            //var pdfDoc = new InputOnlineFile(pdfStream, "Расписание.pdf");
            await Bot.Client.SendPhotoAsync(update.Message.Chat.Id, image);
            //await Bot.Client.SendDocumentAsync(update.Message.Chat.Id, pdfDoc);
            return UpdateHandlingResult.Handled;
        }
    }
}
