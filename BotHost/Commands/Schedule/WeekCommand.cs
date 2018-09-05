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
    public class WeekCommand : CommandBase<DefaultCommandArgs>
    {
        private ImageGenerator _imageGenerator;
        private UsersContext _usersContext;
        public WeekCommand(ImageGenerator imageGenerator, UsersContext usersContext) : base(name: "week")
        {
            _imageGenerator = imageGenerator;
            _usersContext = usersContext;
        }
        protected override bool CanHandleCommand(Update update)
        {
            if (!base.CanHandleCommand(update))
            {
                return update.Message.Text.ToLowerInvariant().Equals("на неделю");
            }
            return true;
        }

        public override async Task<UpdateHandlingResult> HandleCommand(Update update, DefaultCommandArgs args)
        {
            var weekNumber = WeeksCalculator.GetCurrentWeek();
            var messageText = weekNumber % 2 == 0
                ? $"Это {weekNumber}-я неделя - четная."
                : $"Это {weekNumber}-я неделя - нечетная.";
            var user = await _usersContext.TgUsers.SingleOrDefaultAsync(u => u.ChatId == update.Message.Chat.Id);
            if (user == null)
            {
                await Bot.Client.SendTextMessageAsync(update.Message.Chat.Id, "Нет данных в базе, напиши /start");
                return UpdateHandlingResult.Handled;
            }
            var image = await _imageGenerator.GetWeek(user.Group);
            await Bot.Client.SendPhotoAsync(update.Message.Chat.Id, image, messageText);
            return UpdateHandlingResult.Handled;
        }
    }
}
