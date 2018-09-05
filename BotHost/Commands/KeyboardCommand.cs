using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotHost.Commands.CommandsArgs;
using Telegram.Bot.Framework;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotHost.Commands
{
    public class KeyboardCommand : CommandBase<DefaultCommandArgs>
    {
        public KeyboardCommand() : base("keyboard")
        {

        }
        public override async Task<UpdateHandlingResult> HandleCommand(Update update, DefaultCommandArgs args)
        {
            var keyboard = new ReplyKeyboardMarkup(new[]
            {
                new[] {new KeyboardButton("На сегодня")},
                new[] {new KeyboardButton("На завтра")},
                new[] {new KeyboardButton("На неделю")},
                new[] {new KeyboardButton("На неделю(pdf)")}
            });
            await Bot.Client.SendTextMessageAsync(update.Message.Chat.Id, $"Обновление клавиатуры", replyMarkup: keyboard);
            return UpdateHandlingResult.Handled;
        }
    }
}
