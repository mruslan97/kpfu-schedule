﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotHost.Commands.CommandsArgs;
using BotHost.Models;
using BotHost.Tools;
using Telegram.Bot.Framework;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace BotHost.Commands.Schedule
{
    public class TodayCommand : CommandBase<DefaultCommandArgs>
    {
        private ImageGenerator _imageGenerator;
        public TodayCommand(ImageGenerator imageGenerator) : base(name: "today")
        {
            _imageGenerator = imageGenerator;
        }
        protected override bool CanHandleCommand(Update update)
        {
            if (!base.CanHandleCommand(update))
            {
                return update.Message.Text.ToLowerInvariant().Contains("сегодня");
            }
            return true;
        }

        public override async Task<UpdateHandlingResult> HandleCommand(Update update, DefaultCommandArgs args)
        {
            var image = await _imageGenerator.GetDay(update.Message.Chat.Id, true);
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
