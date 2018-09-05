using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotHost.Commands.CommandsArgs;
using Telegram.Bot.Framework;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace BotHost.Commands
{
    public class HelpCommand : CommandBase<DefaultCommandArgs>
    {
        public HelpCommand() : base(name: "help")
        {
        }

        public override async Task<UpdateHandlingResult> HandleCommand(Update update, DefaultCommandArgs args)
        {
            await Bot.Client.SendTextMessageAsync(update.Message.Chat.Id,
                "Для смены группы просто отправь новый номер");
            return UpdateHandlingResult.Handled;
        }
    }
}
