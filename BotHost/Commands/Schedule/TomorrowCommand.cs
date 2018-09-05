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

namespace BotHost.Commands.Schedule
{
    public class TomorrowCommand : CommandBase<DefaultCommandArgs>
    {
        private ImageGenerator _imageGenerator;
        private UsersContext _usersContext;
        public TomorrowCommand(ImageGenerator imageGenerator, UsersContext usersContext) : base(name: "tomorrow")
        {
            _imageGenerator = imageGenerator;
            _usersContext = usersContext;
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
            var user = await _usersContext.TgUsers.SingleOrDefaultAsync(u => u.ChatId == update.Message.Chat.Id);
            if (user == null)
            {
                await Bot.Client.SendTextMessageAsync(update.Message.Chat.Id, "Нет данных в базе, напиши /start");
                return UpdateHandlingResult.Handled;
            }
            var image = await _imageGenerator.GetDay(user.Group, false);
            if (image == null)
            {
                await Bot.Client.SendTextMessageAsync(update.Message.Chat.Id, "Выходной день");
                return UpdateHandlingResult.Handled;
            }
            await Bot.Client.SendPhotoAsync(update.Message.Chat.Id, image);
            return UpdateHandlingResult.Handled;
        }
    }
}
