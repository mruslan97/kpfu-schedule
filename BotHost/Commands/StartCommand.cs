using System.Threading.Tasks;
using BotHost.Commands.CommandsArgs;
using BotHost.Models;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Framework;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;

namespace BotHost.Commands
{
    public class StartCommand : CommandBase<DefaultCommandArgs>
    {
        private UsersContext _usersContext;
        private ILogger<StartCommand> _logger;
        public StartCommand(UsersContext usersContext, ILogger<StartCommand> logger) : base("start")
        {
            _usersContext = usersContext;
            _logger = logger;
        }

        public override async Task<UpdateHandlingResult> HandleCommand(Update update, DefaultCommandArgs args)
        {
            await Bot.Client.SendTextMessageAsync(update.Message.Chat.Id,
                $"Привет, {update.Message.Chat.FirstName}! Введи номер своей группы в формате **-***");
            //using (_usersContext)
            //{
                if (_usersContext.TgUsers.Find(update.Message.Chat.Id) != null)
                    return UpdateHandlingResult.Handled;
                _usersContext.TgUsers.Add(new TgUser
                {
                    ChatId = update.Message.Chat.Id,
                    Username = update.Message.Chat.Username,
                    FirstName = update.Message.Chat.FirstName,
                    LastName = update.Message.Chat.LastName
                });
                await _usersContext.SaveChangesAsync();
            //}
            _logger.LogTrace($"user {update.Message.Chat.Id} saved in db");
            return UpdateHandlingResult.Handled;
        }
    }
}